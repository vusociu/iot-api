using System.IdentityModel.Tokens.Jwt;
using iot_project.Helpers;

namespace iot_project.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            if (path.Contains("/login", StringComparison.OrdinalIgnoreCase) || path.Contains("/check-card", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var token = context.Request.Headers["token"].FirstOrDefault();
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
