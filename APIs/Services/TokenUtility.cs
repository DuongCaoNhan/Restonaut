using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace APIs.Services
{
    public static class TokenUtility
    {
        /// <summary>
        /// Extracts the token from the Authorization header
        /// </summary>
        public static string? ExtractTokenFromHeader(this HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeader))
                return null;

            var authHeaderValue = authHeader.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeaderValue) || !authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            return authHeaderValue.Substring("Bearer ".Length).Trim();
        }

        /// <summary>
        /// Gets the user ID from a validated ClaimsPrincipal
        /// </summary>
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Gets the email from a validated ClaimsPrincipal
        /// </summary>
        public static string? GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Gets the token expiration time from a validated ClaimsPrincipal
        /// </summary>
        public static DateTime? GetExpirationTime(this ClaimsPrincipal principal)
        {
            var expClaim = principal.FindFirst("exp");
            if (expClaim != null && long.TryParse(expClaim.Value, out long expirationSeconds))
            {
                // Convert Unix timestamp to DateTime
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(expirationSeconds);
            }
            
            return null;
        }
    }
}