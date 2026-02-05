using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Pets.API.ComponentTest;

public interface IEmailInbox
{
    ConcurrentBag<EmailMessage> Sent { get; }
}

public record EmailMessage(string To, string Subject, string HtmlBody);

public class FakeEmailSender : IEmailSender, IEmailInbox
{
    public ConcurrentBag<EmailMessage> Sent { get; } = new();

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Sent.Add(new EmailMessage(email, subject, htmlMessage));
        return Task.CompletedTask;
    }
}

