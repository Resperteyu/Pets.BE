using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pets.API.Settings;

namespace Pets.API.Services;

public class EmailService
{
    private readonly IEmailSender _emailSender;
    private readonly WebSiteSettings _webSiteSettings;
    private readonly Dictionary<string, string> _emailTemplates = new();

    public EmailService(IEmailSender emailSender, IOptions<WebSiteSettings> webSiteSettings)
    {
        _emailSender = emailSender;
        _webSiteSettings = webSiteSettings.Value;
    }

    public async Task SendConfirmationEmailAsync(string email, string token)
    {
        var textTemplate = await GetEmailTemplateAsync("ConfirmationEmail.txt");
        var htmlTemplate = await GetEmailTemplateAsync("ConfirmationEmail.html");

        var link = BuildLink("Verify", email, token);

        textTemplate = textTemplate.Replace("[Link]", link);
        htmlTemplate = htmlTemplate.Replace("[Link]", link);

        await _emailSender.SendEmailAsync(email, "Confirm Your Email Address", htmlTemplate, textTemplate);
    }

    public async Task SendForgotPasswordEmailAsync(string email, string token)
    {
        var textTemplate = await GetEmailTemplateAsync("ForgotPasswordEmail.txt");
        var htmlTemplate = await GetEmailTemplateAsync("ForgotPasswordEmail.html");

        var link = BuildLink("ResetPassword", email, token);

        textTemplate = textTemplate.Replace("[Link]", link);
        htmlTemplate = htmlTemplate.Replace("[Link]", link);

        await _emailSender.SendEmailAsync(email, "Password Reset Request", htmlTemplate, textTemplate);
    }

    private async Task<string> GetEmailTemplateAsync(string templateName)
    {
        if (_emailTemplates.TryGetValue(templateName, out var template))
        {
            return template;
        }

        var runningPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(EmailService))!.Location)!;
        template = await File.ReadAllTextAsync(Path.Combine(runningPath, "Templates", templateName));
        _emailTemplates[templateName] = template;

        return template;
    }

    private string BuildLink(string path, string email, string token)
    {
        var encodedEmail = WebUtility.UrlEncode(email);
        var encodedToken = WebUtility.UrlEncode(token);

        return $"{_webSiteSettings.RootUrl}/{path}?email={encodedEmail}&token={encodedToken}";
    }
}