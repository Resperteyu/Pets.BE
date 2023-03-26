using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Movrr.API.Email.Service
{
  public class EmailService : IEmailService
  {
    private readonly EmailConfiguration _emailConfig;

    public EmailService(EmailConfiguration emailConfig)
    {
      _emailConfig = emailConfig;
    }

    public async Task SendEmailAsync(Message message, string fileName, Stream attachment)
    {
      var emailMessage = CreateEmailMessage(message, fileName, attachment);

      await SendAsync(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message, string fileName, Stream attachment)
    {
      var emailMessage = new MimeMessage();
      emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
      emailMessage.To.AddRange(message.To);
      emailMessage.Subject = message.Subject;

      var bodyBuilder = new BodyBuilder { HtmlBody = message.Content };
      if (attachment != null)
      {
        bodyBuilder.Attachments.Add(fileName, attachment);
      }

      emailMessage.Body = bodyBuilder.ToMessageBody();

      return emailMessage;
    }

    private async Task SendAsync(MimeMessage mailMessage)
    {
      using (var client = new SmtpClient())
      {
        try
        {
          client.CheckCertificateRevocation = false;
          await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);

          client.AuthenticationMechanisms.Remove("XOAUTH2");
          await client.AuthenticateAsync(_emailConfig.UserName, "hijtacfjkxzjmgun");

          await client.SendAsync(mailMessage);
        }
        catch(Exception ex)
        {
          //log an error message or throw an exception or both.
          throw;
        }
        finally
        {
          await client.DisconnectAsync(true);
          client.Dispose();
        }
      }
    }
  }
}