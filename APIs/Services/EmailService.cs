namespace APIs.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfigurationService _configService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfigurationService configService, ILogger<EmailService> logger)
        {
            _configService = configService;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Get SendGrid API key from user secrets
                var sendGridApiKey = _configService.GetSendGridApiKey();
                
                // In a real implementation, you would use the SendGrid SDK here
                // Example:
                // var client = new SendGridClient(sendGridApiKey);
                // var msg = new SendGridMessage();
                // msg.SetFrom(new EmailAddress("noreply@yourapp.com", "Your App"));
                // msg.AddTo(new EmailAddress(to));
                // msg.SetSubject(subject);
                // msg.AddContent(MimeType.Html, body);
                // var response = await client.SendEmailAsync(msg);
                
                _logger.LogInformation("Email would be sent to {To} with subject {Subject}", to, subject);
                
                // Simulate async operation
                await Task.Delay(100);
                
                return true;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("SendGrid API key not configured: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                return false;
            }
        }
    }
}
