using System.Threading.Tasks;

namespace ServicesUtils.Authentication
{
    public interface IAuthenticator
    {
        Task Authenticate();
    }
}