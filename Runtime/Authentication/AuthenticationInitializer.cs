using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNRD;
using UnityEngine;
using UnityEngine.Events;

namespace ServicesUtils.Authentication
{
    public class AuthenticationInitializer : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IAuthenticator> _fallback;
        [SerializeField] private List<RuntimePlatformAuthenticator> _authenticators;

        [SerializeField] private UnityEvent _onInitialized;

        public async void Initialize()
        {
            var authenticator = GetAuthenticator();

            bool success = false;
            if (authenticator != null)
            {
                success = await AuthenticateWithAuthenticator(authenticator);
            }

            if (!success)
            {
                success = await AuthenticateWithAuthenticator(_fallback.Value);
            }

            if (success)
            {
                _onInitialized.Invoke();
            }
        }

        private async Task<bool> AuthenticateWithAuthenticator(IAuthenticator authenticator)
        {
            try
            {
                await authenticator.Authenticate();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                return false;
            }
        }

        private IAuthenticator GetAuthenticator()
        {
            bool Predicate(RuntimePlatformAuthenticator platAuth)
            {
                return platAuth.Platform == Application.platform;
            }

            var exists = _authenticators.Exists(Predicate);
            if (exists)
            {
                return _authenticators.First(Predicate).Authenticator.Value;
            }

            return null;
        }
    }

    [Serializable]
    public class RuntimePlatformAuthenticator
    {
        [SerializeField] private RuntimePlatform _platform;
        [SerializeField] private SerializableInterface<IAuthenticator> _authenticator;

        public RuntimePlatform Platform => _platform;

        public SerializableInterface<IAuthenticator> Authenticator => _authenticator;
    }
}