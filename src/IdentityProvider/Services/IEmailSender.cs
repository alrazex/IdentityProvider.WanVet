using System.Threading.Tasks;

namespace IdentityProvider.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
