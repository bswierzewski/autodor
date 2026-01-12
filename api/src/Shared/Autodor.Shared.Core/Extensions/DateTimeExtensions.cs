namespace Autodor.Shared.Core.Extensions;

/// <summary>
/// Extension methods for DateTime operations
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>Generates all days between two dates (inclusive)</summary>
    /// <param name="from">The start date</param>
    /// <param name="to">The end date</param>
    /// <returns>An enumerable sequence of each day from start to end</returns>
    public static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
    {
        for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
        {
            yield return day;
        }
    }
}