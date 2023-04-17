using System.IO;
using System.Threading.Tasks;

namespace Pets.API.Email.Service
{
  public interface IEmailService
  {
    Task SendEmailAsync(Message message, string fileName, Stream attachment);
  }
}