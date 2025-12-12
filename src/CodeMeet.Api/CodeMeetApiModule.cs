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
using System.Reflection;
using CodeMeet.Api.Middlewares;
using CodeMeet.Ddd;
using CodeMeet.Ddd.Application.Cqrs.Validation;
using System.Text.Json.Serialization;

namespace CodeMeet.Api;

public static class CodeMeetApiModule
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(opts => opts.Conventions.Add(new RouteTokenTransformerConvention(LowercaseRouteTransformer.Default)))
            .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var exception = context.HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

                if (exception is ValidationException validationException)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.ProblemDetails.Status = StatusCodes.Status400BadRequest;
                    context.ProblemDetails.Title = "One or more validation errors occurred.";
                    context.ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                    var errors = new Dictionary<string, string[]>();
                    foreach (var error in validationException.Errors)
                    {
                        if (errors.TryGetValue(error.PropertyName, out var existingErrors))
                        {
                            errors[error.PropertyName] = [..existingErrors, error.ErrorMessage];
                        }
                        else
                        {
                            errors[error.PropertyName] = [error.ErrorMessage];
                        }
                    }
                    context.ProblemDetails.Extensions["errors"] = errors;
                }
            };
        });

        return services;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseExceptionHandler();
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