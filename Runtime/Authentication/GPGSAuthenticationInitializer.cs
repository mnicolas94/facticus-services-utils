#if ENABLED_AUTHENTICATION

using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace ServicesUtils.Authentication
{
    public class GPGSAuthenticationInitializer : MonoBehaviour, IAuthenticator
    {
        
        private string _token;
        private string _error;
        
        private void Awake()
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Activate();
#endif
        }
        
        public async Task Authenticate()
        {
            await LoginGooglePlayGames();
            await SignInWithGooglePlayGamesAsync(_token);
        }
        
        //Fetch the Token / Auth code
        private Task LoginGooglePlayGames()
        {
            var tcs = new TaskCompletionSource<object>();
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate((success) =>
            {
                if (success == SignInStatus.Success)
                {
                    Debug.Log("Login with Google Play games successful.");
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                    {
                        Debug.Log("Authorization code: " + code);
                        _token = code;
                        // This token serves as an example to be used for SignInWithGooglePlayGames
                        tcs.SetResult(null);
                    });
                }
                else
                {
                    _error = "Failed to retrieve Google play games authorization code";
                    Debug.Log("Login Unsuccessful");
                    tcs.SetException(new Exception($"Failed: {success}"));
                }
            });
#else
            tcs.SetResult(null);
#endif
            return tcs.Task;
        }
 
        async Task SignInWithGooglePlayGamesAsync(string authCode)
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
        }
    }
}

#endif