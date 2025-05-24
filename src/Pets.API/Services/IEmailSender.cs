using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage);
    }
}
