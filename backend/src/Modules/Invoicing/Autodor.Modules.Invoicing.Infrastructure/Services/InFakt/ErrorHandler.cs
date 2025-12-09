using System.Text.Json;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public static class ErrorHandler
{
    public static string ParseError(string errorContent)
    {
        try
        {
            var errorDto = JsonSerializer.Deserialize<ErrorDto>(errorContent);

            if (errorDto?.Errors == null && string.IsNullOrEmpty(errorDto?.Message))
                return errorContent;

            var errors = new List<string>();

            if (!string.IsNullOrEmpty(errorDto.Message))
                errors.Add(errorDto.Message);

            if (errorDto.Errors != null)
            {
                foreach (var error in errorDto.Errors)
                {
                    errors.AddRange(error.Value.Where(e => !string.IsNullOrEmpty(e)));
                }
            }

            return errors.Count > 0 ? string.Join("; ", errors) : errorContent;
        }
        catch
        {
            return errorContent;
        }
    }
}