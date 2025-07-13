using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APIs.Services;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configService;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(IConfigurationService configService, ILogger<ConfigurationController> logger)
        {
            _configService = configService;
            _logger = logger;
        }

        /// <summary>
        /// Get configuration status (for testing purposes - remove in production)
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetConfigurationStatus()
        {
            try
            {
                var status = new
                {
                    JwtConfigured = !string.IsNullOrEmpty(_configService.GetJwtKey()),
                    JwtExpiryMinutes = _configService.GetJwtExpiryMinutes(),
                    HasDefaultConnection = HasConnectionString("DefaultConnection"),
                    HasSqliteConnection = HasConnectionString("SqliteConnection"),
                    HasProductionConnection = HasConnectionString("ProductionConnection"),
                    HasExternalApiKey = HasApiKey("ExternalService"),
                    HasSendGridKey = HasSendGridApiKey(),
                    HasAzureBlobStorage = HasAzureBlobStorageConnection(),
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking configuration status");
                return StatusCode(500, "Error checking configuration");
            }
        }

        /// <summary>
        /// Example of using external API with stored API key
        /// </summary>
        [HttpGet("external-service-example")]
        public IActionResult CallExternalService()
        {
            try
            {
                var apiKey = _configService.GetApiKey("ExternalService");
                
                // In a real scenario, you would use this API key to call an external service
                // Example: var client = new HttpClient();
                // client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
                // var response = await client.GetAsync("https://api.external.com/data");
                
                return Ok(new { 
                    Message = "External service called successfully", 
                    ApiKeyConfigured = !string.IsNullOrEmpty(apiKey),
                    Timestamp = DateTime.UtcNow 
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("External API key not configured: {Message}", ex.Message);
                return BadRequest("External API key not configured");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling external service");
                return StatusCode(500, "Error calling external service");
            }
        }

        private bool HasConnectionString(string name)
        {
            try
            {
                var connectionString = _configService.GetConnectionString(name);
                return !string.IsNullOrEmpty(connectionString);
            }
            catch
            {
                return false;
            }
        }

        private bool HasApiKey(string serviceName)
        {
            try
            {
                var apiKey = _configService.GetApiKey(serviceName);
                return !string.IsNullOrEmpty(apiKey);
            }
            catch
            {
                return false;
            }
        }

        private bool HasSendGridApiKey()
        {
            try
            {
                var apiKey = _configService.GetSendGridApiKey();
                return !string.IsNullOrEmpty(apiKey);
            }
            catch
            {
                return false;
            }
        }

        private bool HasAzureBlobStorageConnection()
        {
            try
            {
                var connectionString = _configService.GetAzureBlobStorageConnectionString();
                return !string.IsNullOrEmpty(connectionString);
            }
            catch
            {
                return false;
            }
        }
    }
}
