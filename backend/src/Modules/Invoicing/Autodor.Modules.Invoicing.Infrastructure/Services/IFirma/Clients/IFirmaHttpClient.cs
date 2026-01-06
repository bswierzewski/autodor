using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;
using ErrorOr;
using System.Net.Http.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

public class IFirmaHttpClient(HttpClient httpClient)
{
    private static class ErrorCodes
    {
        public const string CreateInvoiceFailed = "IFirma.CreateInvoiceFailed";
        public const string EmptyResponse = "IFirma.EmptyResponse";
        public const string BusinessValidationError = "IFirma.BusinessValidationError";
    }

    public async Task<ErrorOr<ResponseRoot>> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        using var request = new HttpRequestMessage(HttpMethod.Post, "fakturakraj.json");
        request.SetApiKey(IFirmaKeyType.Invoice);
        request.Content = JsonContent.Create(invoice);

        return await SendRequestAsync<ResponseRoot>(
            request,
            ErrorCodes.CreateInvoiceFailed,
            ValidateBusinessResponse,
            cancellationToken);
    }

    private async Task<ErrorOr<TResponse>> SendRequestAsync<TResponse>(
        HttpRequestMessage request,
        string errorCode,
        Func<TResponse, ErrorOr<TResponse>>? businessValidator,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return Error.Failure(
                errorCode,
                $"Status: {response.StatusCode}, Error: {errorContent}");
        }

        var responseData = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken);

        if (responseData is null)
            return Error.Failure(ErrorCodes.EmptyResponse, "Empty response from IFirma API.");

        return businessValidator?.Invoke(responseData) ?? responseData;
    }

    private static ErrorOr<ResponseRoot> ValidateBusinessResponse(ResponseRoot response)
    {
        if (!response.Response.IsSuccess)
        {
            return Error.Validation(
                ErrorCodes.BusinessValidationError,
                response.Response.Message ?? $"Business error occurred. Status code: {response.Response.StatusCode}");
        }

        return response;
    }
}
