using Application.Common.Interfaces;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Web.Infrastructure;
using Web.Services;

namespace Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddFluentValidationRulesToSwagger();

        // Customise default API behaviour
        //services.Configure<ApiBehaviorOptions>(options =>
        //    options.SuppressModelStateInvalidFilter = true);
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHttpContextAccessor();

        // Authentication and Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Auth0:Authority"];
                options.Audience = configuration["Auth0:Audience"];

                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    ValidIssuer = configuration["Auth0:Authority"],
                //    ValidAudience = configuration["Auth0:Audience"],
                //    ValidateIssuer = true,
                //    ValidateAudience = true,
                //    ValidateLifetime = true,
                //    ValidateIssuerSigningKey = true,
                //    ClockSkew = TimeSpan.Zero
                //};
            });
        services.AddAuthorization();

        // Add CORS services only in development mode
        if (env.IsDevelopment())
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }

        // DI services
        services.AddScoped<IUser, CurrentUser>();

        return services;
    }
}