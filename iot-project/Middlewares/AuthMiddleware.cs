using System.IdentityModel.Tokens.Jwt;
using iot_project.Helpers;
using Microsoft.Extensions.Logging;

namespace iot_project.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            if (path.Contains("/login", StringComparison.OrdinalIgnoreCase) || path.Contains("/check-card", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            _logger.LogInformation(token);
            if (token != null && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }
            if (token == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing or invalid token.");
                return;
            }
            try
            {
                var jwtService = context.RequestServices.GetRequiredService<JwtService>();
                var dataToken = jwtService.verify(token);
                var userId = int.Parse(dataToken.Issuer);
                if (userId == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Invalid token claims.");
                    return;
                }
                context.Items["User"] = userId;
                await _next(context);
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Token validation failed.");
            }
        }
    }
}
