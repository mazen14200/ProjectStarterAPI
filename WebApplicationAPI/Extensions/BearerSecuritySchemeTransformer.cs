using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebApplicationAPI.Extensions;

/// <summary>
/// Registers the "Bearer" security scheme on the generated OpenAPI document
/// so Scalar (and any generated client) shows an Authorize / "Bearer token"
/// field instead of assuming the API is unauthenticated.
/// </summary>

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter the access token returned by /api/v1/auth/login."
        };

        document.Components.SecuritySchemes["Bearer"] = scheme;

        document.Security ??= [];

        document.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

        return Task.CompletedTask;
    }
}