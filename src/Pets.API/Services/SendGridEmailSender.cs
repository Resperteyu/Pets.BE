using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Pets.API.Services;

public class SendGridEmailSender : IEmailSender
{
    private readonly ISendGridClient sendGridClient;
    private readonly ILogger logger;

    public SendGridEmailSender(ISendGridClient sendGridClient, ILogger<SendGridEmailSender> logger)
    {
        this.sendGridClient = sendGridClient;
        this.logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("mggprom@gmail.com", "www.petshub.com"),
            Subject = subject,
            PlainTextContent = htmlMessage,
            HtmlContent = htmlMessage
        };
        msg.AddTo(new EmailAddress(email));

        var response = await sendGridClient.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to send email");
        }
    }
}