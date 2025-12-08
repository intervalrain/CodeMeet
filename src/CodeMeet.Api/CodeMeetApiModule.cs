using Microsoft.OpenApi.Models;
using CodeMeet.Application;
using CodeMeet.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Asp.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using CodeMeet.Application.Common.Security;
using System.Text;
using CodeMeet.Api.Middlewares;

namespace CodeMeet.Api;

public static class CodeMeetApiModule
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(opts => opts.Conventions.Add(new RouteTokenTransformerConvention(LowercaseRouteTransformer.Default)));
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true; 
        });
        services.AddSwagger();
        services.AddApplication(configuration);
        services.AddInfrastructure(configuration);

        var sp = services.BuildServiceProvider();
        var jwtSettings = sp.GetService<IOptions<JwtSettings>>()?.Value!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });
        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AuditContextMiddleware>();
        app.MapControllers();
        
        return app;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CodeMeet API",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token." 
            });

            c.OperationFilter<SecurityRequirementOperationFilter>();
        });

        return services;
    }

    public static WebApplication UseSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("swagger/v1/swagger.json", "CodeMeet API v1");
            options.RoutePrefix = "";
        });

        return app;
    }
}

internal class SecurityRequirementOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAllowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any() ||
            (context.MethodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any() ?? false);

        if (hasAllowAnonymous) return;

        operation.Security = [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            }  
        ];
    }
}

internal class LowercaseRouteTransformer : IOutboundParameterTransformer
{
    public static readonly LowercaseRouteTransformer Default = new();
    
    public string? TransformOutbound(object? value)
    {
        return value?.ToString()?.ToLower();
    }
}