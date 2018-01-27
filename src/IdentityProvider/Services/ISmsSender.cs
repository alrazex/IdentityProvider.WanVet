using System.Threading.Tasks;

namespace IdentityProvider.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
