#if ENABLED_AUTHENTICATION

using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

namespace ServicesUtils.Authentication
{
    public class DebugAuthentication : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _debugText;

        public async void UpdateDebugText()
        {
            var Profile = AuthenticationService.Instance.Profile;
            var AccessToken = AuthenticationService.Instance.AccessToken;
            var IsAuthorized = AuthenticationService.Instance.IsAuthorized;
            var IsExpired = AuthenticationService.Instance.IsExpired;
            var PlayerId = AuthenticationService.Instance.PlayerId;
            var PlayerName = AuthenticationService.Instance.PlayerName;
            var PlayerNameAsync = await AuthenticationService.Instance.GetPlayerNameAsync();
            var IsSignedIn = AuthenticationService.Instance.IsSignedIn;
            var SessionTokenExists = AuthenticationService.Instance.SessionTokenExists;
            
            var debugText = $"Profile: {Profile}\n" +
                            $"AccessToken: {AccessToken}\n" +
                            $"IsAuthorized: {IsAuthorized}\n" +
                            $"IsExpired: {IsExpired}\n" +
                            $"PlayerId: {PlayerId}\n" +
                            $"PlayerNameAsync: {PlayerNameAsync}\n" +
                            $"PlayerName: {PlayerName}\n" +
                            $"IsSignedIn: {IsSignedIn}\n" +
                            $"SessionTokenExists: {SessionTokenExists}";
            _debugText.text = debugText;
        }
    }
}

#endif