using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlightBooking.Utils;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                               .OfType<AuthorizeAttribute>().Any() 
                           || context.MethodInfo.GetCustomAttributes(true)
                               .OfType<AuthorizeAttribute>().Any();

        if (!hasAuthorize) return;

        var securityRequirement = new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }
            ] = Array.Empty<string>()
        };

        operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };;
    }
}