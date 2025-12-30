using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CartService.API.Middleware
{
    public class JwtAccessLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAccessLoggingMiddleware> _logger;

        public JwtAccessLoggingMiddleware(
            RequestDelegate next,
            ILogger<JwtAccessLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader["Bearer ".Length..];

                try
                {
                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                    var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
                    var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    var issuer = jwt.Issuer;
                    var audience = jwt.Audiences.FirstOrDefault();
                    var expires = jwt.ValidTo;

                    _logger.LogInformation(
                        "Cart access token received | UserId: {UserId}, Role: {Role}, Issuer: {Issuer}, Audience: {Audience}, Expires: {Expires}",
                        userId, role, issuer, audience, expires);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Invalid JWT token received: {Message}", ex.Message);
                }
            }

            await _next(context);
        }
    }
}
