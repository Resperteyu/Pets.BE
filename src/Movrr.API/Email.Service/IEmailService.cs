using System.IO;
using System.Threading.Tasks;

namespace Movrr.API.Email.Service
{
  public interface IEmailService
  {
    Task SendEmailAsync(Message message, string fileName, Stream attachment);
  }
}