using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Pets.API.Services;

public class SendGridEmailSender : IEmailSender
{
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger _logger;

    public SendGridEmailSender(ISendGridClient sendGridClient, ILogger<SendGridEmailSender> logger)
    {
        _sendGridClient = sendGridClient;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("mggprom@gmail.com", "www.petshub.com"),
            Subject = subject,
            PlainTextContent = textMessage,
            HtmlContent = htmlMessage
        };
        msg.AddTo(new EmailAddress(email));

        var response = await _sendGridClient.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email");
        }
    }
}