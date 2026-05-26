using Autodor.Modules.Invoicing.Features.SendInvoiceSummaryEmail;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wolverine;

namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

/// <summary>
/// Runs bulk invoice creation automatically on the 14th and on the last day of each month.
/// Trigger days and time-of-day are configured via <see cref="SchedulingOptions"/>.
/// </summary>
internal sealed class CreateInvoicesScheduler(
    IServiceScopeFactory scopeFactory,
    IHostApplicationLifetime lifetime,
    IOptions<InvoicingOptions> options,
    ILogger<CreateInvoicesScheduler> logger) : BackgroundService
{
    private readonly SchedulingOptions _scheduling = options.Value.Scheduling;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_scheduling.Enabled)
        {
            logger.LogDebug("Invoice scheduler is disabled (Modules:Invoicing:Scheduling:Enabled = false)");
            return;
        }

        if (!await lifetime.WaitForAppStartupAsync(stoppingToken))
            return;

        logger.LogInformation(
            "Invoice scheduler started. Trigger days: 14th and last day of month at {RunTime}",
            _scheduling.RunTime);

        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = CalculateNextRunTime(_scheduling.RunTime);
            var delay = nextRun - DateTimeOffset.Now;

            logger.LogInformation("Next invoice run scheduled for {NextRun}", nextRun);

            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                break;

            await RunJobAsync(stoppingToken);
        }
    }

    private async Task RunJobAsync(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var (dateFrom, dateTo) = GetDateRangeForTriggerDay(today);

        logger.LogInformation("Invoice scheduler firing. Range: {DateFrom}–{DateTo}", dateFrom, dateTo);

        await using var scope = scopeFactory.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        CreateInvoicesResult result;
        try
        {
            result = await bus.InvokeAsync<CreateInvoicesResult>(
                new CreateInvoicesCommand(dateFrom.ToDateTime(TimeOnly.MinValue), dateTo.ToDateTime(TimeOnly.MinValue)),
                ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return;
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning("No orders found during scheduled invoice run: {Message}", ex.Message);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Scheduled invoice run failed");
            return;
        }

        if (_scheduling.SummaryRecipients.Count > 0)
        {
            try
            {
                await bus.InvokeAsync(new SendInvoiceSummaryEmailCommand(dateFrom, dateTo, result), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to dispatch invoice summary email");
            }
        }
    }

    // ---------- scheduling helpers ----------

    internal static DateTimeOffset CalculateNextRunTime(TimeOnly runTime)
    {
        var now = DateTimeOffset.Now;
        var today = DateOnly.FromDateTime(now.DateTime);
        var todayTriggerTime = new DateTimeOffset(today.ToDateTime(runTime), now.Offset);

        // If today is a trigger day and the run time hasn't passed yet, return today's slot.
        if (IsTriggerDay(today) && now < todayTriggerTime)
            return todayTriggerTime;

        // Otherwise walk forward to the next trigger day.
        var candidate = today.AddDays(1);
        while (!IsTriggerDay(candidate))
            candidate = candidate.AddDays(1);

        return new DateTimeOffset(candidate.ToDateTime(runTime), now.Offset);
    }

    internal static bool IsTriggerDay(DateOnly date)
    {
        var lastDay = DateTime.DaysInMonth(date.Year, date.Month);
        return date.Day == 14 || date.Day == lastDay;
    }

    /// <summary>
    /// Returns the billing date range that corresponds to the given trigger day:
    /// <list type="bullet">
    ///   <item>14th → 1st–14th of the same month (first half)</item>
    ///   <item>last day → 15th–last day of the same month (second half)</item>
    /// </list>
    /// </summary>
    internal static (DateOnly DateFrom, DateOnly DateTo) GetDateRangeForTriggerDay(DateOnly triggerDay)
    {
        var lastDay = DateTime.DaysInMonth(triggerDay.Year, triggerDay.Month);
        if (triggerDay.Day == 14)
            return (new DateOnly(triggerDay.Year, triggerDay.Month, 1), triggerDay);

        // Last day of month (handles edge case: months where last day == 14 are handled by the 14th branch above)
        return (new DateOnly(triggerDay.Year, triggerDay.Month, 15),
                new DateOnly(triggerDay.Year, triggerDay.Month, lastDay));
    }
}
