#if ENABLED_AUTHENTICATION

using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace ServicesUtils.Authentication
{
    public class AnonymousAuthenticator : MonoBehaviour, IAuthenticator
    {
        public async Task Authenticate()
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}

#endif