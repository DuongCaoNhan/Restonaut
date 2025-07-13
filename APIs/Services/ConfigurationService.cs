using Microsoft.Extensions.Configuration;

namespace APIs.Services
{
    public interface IConfigurationService
    {
        string GetJwtKey();
        string GetJwtIssuer();
        string GetJwtAudience();
        int GetJwtExpiryMinutes();
        string GetConnectionString(string name = "DefaultConnection");
        string GetApiKey(string serviceName);
        string GetAzureBlobStorageConnectionString();
        string GetSendGridApiKey();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetJwtKey()
        {
            return _configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key is not configured in user secrets or environment variables");
        }

        public string GetJwtIssuer()
        {
            return _configuration["Jwt:Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer is not configured in user secrets or environment variables");
        }

        public string GetJwtAudience()
        {
            return _configuration["Jwt:Audience"] 
                ?? throw new InvalidOperationException("JWT Audience is not configured in user secrets or environment variables");
        }

        public int GetJwtExpiryMinutes()
        {
            var expiryMinutes = _configuration["Jwt:ExpiryMinutes"];
            if (int.TryParse(expiryMinutes, out var minutes))
            {
                return minutes;
            }
            return 60; // Default to 60 minutes
        }

        public string GetConnectionString(string name = "DefaultConnection")
        {
            return _configuration.GetConnectionString(name) 
                ?? throw new InvalidOperationException($"Connection string '{name}' is not configured in user secrets or environment variables");
        }

        public string GetApiKey(string serviceName)
        {
            return _configuration[$"ApiKeys:{serviceName}"] 
                ?? throw new InvalidOperationException($"API key for '{serviceName}' is not configured in user secrets or environment variables");
        }

        public string GetAzureBlobStorageConnectionString()
        {
            return _configuration["Azure:BlobStorage:ConnectionString"] 
                ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured in user secrets or environment variables");
        }

        public string GetSendGridApiKey()
        {
            return _configuration["SendGrid:ApiKey"] 
                ?? throw new InvalidOperationException("SendGrid API key is not configured in user secrets or environment variables");
        }
    }
}
