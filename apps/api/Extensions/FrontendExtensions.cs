using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace Autodor.API.Extensions;

internal static class FrontendExtensions
{
    public static IServiceCollection AddFrontend(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
        services.Configure<BrotliCompressionProviderOptions>(options =>
            options.Level = CompressionLevel.Fastest);
        services.Configure<GzipCompressionProviderOptions>(options =>
            options.Level = CompressionLevel.Fastest);

        return services;
    }

    public static WebApplication UseFrontend(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers.XFrameOptions = "SAMEORIGIN";

            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.Headers.CacheControl =
                    context.Request.Path.StartsWithSegments("/assets")
                        ? "public,max-age=31536000,immutable"
                        : "no-cache";
            }

            await next(context);
        });

        app.UseResponseCompression();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Unknown API routes must remain 404s instead of falling through to the SPA.
        app.MapFallback("/api/{**path}", () => Results.NotFound());
        app.MapFallbackToFile("index.html");

        return app;
    }
}
