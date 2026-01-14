using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Autodor.Shared.Infrastructure.Extensions;

public static class OpenApiProblemDetailsExtensions
{
    public static OpenApiOptions AddProblemDetailsResponses(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();

            if (!document.Components.Schemas.ContainsKey("ProblemDetails"))            
                document.Components.Schemas["ProblemDetails"] = CreateProblemDetailsSchema();            

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (!operation.Responses.ContainsKey("default"))
            {
                operation.Responses.Add("default", new OpenApiResponse
                {
                    Description = "An error occurred (4xx/5xx)",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/problem+json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "ProblemDetails"
                                }
                            }
                        }
                    }
                });
            }

            return Task.CompletedTask;
        });

        return options;
    }

    private static OpenApiSchema CreateProblemDetailsSchema() => new()
    {
        Type = "object",
        Description = "Standard error response.",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["type"] = new() { Type = "string" },
            ["title"] = new() { Type = "string" },
            ["status"] = new() { Type = "integer", Format = "int32" },
            ["detail"] = new() { Type = "string" },
            ["instance"] = new() { Type = "string" },
            ["traceId"] = new() { Type = "string" },
            ["timestamp"] = new() { Type = "string", Format = "date-time" },
            ["errors"] = new()
            {
                Type = "object",
                Nullable = true,
                Description = "Validation errors (grouped by field name)",
                AdditionalProperties = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema { Type = "string" }
                }
            }
        }
    };
}
