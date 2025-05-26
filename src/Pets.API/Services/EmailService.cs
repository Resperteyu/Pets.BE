using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pets.API.Settings;

namespace Pets.API.Services;

public class EmailService(IEmailSender emailSender, IOptions<WebSiteSettings> webSiteSettings)
{
    private readonly WebSiteSettings _webSiteSettings = webSiteSettings.Value;
    private readonly Dictionary<string, string> _emailTemplates = new();

    public async Task SendConfirmationEmailAsync(string email, string token)
    {
        var textTemplate = await GetEmailTemplateAsync("ConfirmationEmail.txt");
        var htmlTemplate = await GetEmailTemplateAsync("ConfirmationEmail.html");

        var link = BuildLink("Verify", email, token);

        textTemplate = textTemplate.Replace("[Link]", link);
        htmlTemplate = htmlTemplate.Replace("[Link]", link);

        await emailSender.SendEmailAsync(email, "Confirm Your Email Address", htmlTemplate, textTemplate);
    }

    public async Task SendForgotPasswordEmailAsync(string email, string token)
    {
        var textTemplate = await GetEmailTemplateAsync("ForgotPasswordEmail.txt");
        var htmlTemplate = await GetEmailTemplateAsync("ForgotPasswordEmail.html");

        var link = BuildLink("ResetPassword", email, token);

        textTemplate = textTemplate.Replace("[Link]", link);
        htmlTemplate = htmlTemplate.Replace("[Link]", link);

        await emailSender.SendEmailAsync(email, "Password Reset Request", htmlTemplate, textTemplate);
    }

    public async Task SendMateRequestEmailAsync(string email, Guid mateRequestId)
    {
        var textTemplate = await GetEmailTemplateAsync("MateRequestEmail.txt");
        var htmlTemplate = await GetEmailTemplateAsync("MateRequestEmail.html");

        var link = BuildMateRequestLink(mateRequestId);

        textTemplate = textTemplate.Replace("[Link]", link);
        htmlTemplate = htmlTemplate.Replace("[Link]", link);

        await emailSender.SendEmailAsync(email, "Mate request received", htmlTemplate, textTemplate);
    }

    public async Task SendMateRequestStatusChangeEmailAsync(string email, Guid mateRequestId)
    {
        var textTemplate = await GetEmailTemplateAsync("MateRequestStatusChangeEmail.txt");
        var htmlTemplate = await GetEmailTemplateAsync("MateRequestStatusChangeEmail.html");

        var link = BuildMateRequestLink(mateRequestId);

        textTemplate = textTemplate.Replace("[Link]", link);
        htmlTemplate = htmlTemplate.Replace("[Link]", link);

        await emailSender.SendEmailAsync(email, "Mate request update", htmlTemplate, textTemplate);
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

    private string BuildMateRequestLink(Guid mateRequestId)
    {
        return $"{_webSiteSettings.RootUrl}/MyMateRequest/{mateRequestId}";
    }
}