using System;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Pets.API.Services;

public class SendGridEmailSender(ISendGridClient sendGridClient, ILogger<SendGridEmailSender> logger)
    : IEmailSender
{
    private readonly ILogger _logger = logger;

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

        var response = await sendGridClient.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email");
        }
    }
}

public class ConsoleEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage)
    {
        Console.WriteLine($"[Email to: {email}] Subject: {subject}\n{textMessage}\n{htmlMessage}");
        return Task.CompletedTask;
    }
}