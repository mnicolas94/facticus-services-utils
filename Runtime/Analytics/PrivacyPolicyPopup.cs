using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using UnityEngine;
using UnityEngine.UI;

namespace ServicesUtils.Analytics
{
    public class PrivacyPolicyPopup : AsyncPopupReturnable<bool>
    {
        private const string PlayerPrefsKeyDisplayed = "privacypolicy_displayed";
        private const string PlayerPrefsKeyGaveConsent = "privacypolicy_gaveconsent";

        [SerializeField] private GameObject _popupObject;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _dontAcceptButton;

        private bool AlreadyDisplayed => PlayerPrefs.GetInt(PlayerPrefsKeyDisplayed, 0) == 1;
        private bool UserGaveConsent => PlayerPrefs.GetInt(PlayerPrefsKeyGaveConsent, 0) == 1;

        public override void Initialize()
        {
            _popupObject.SetActive(!AlreadyDisplayed);
        }
        
        public override async Task<bool> Show(CancellationToken ct)
        {
            var isConsentRequested = PlayerPrefs.HasKey(PlayerPrefsKeyGaveConsent);
            if (!AlreadyDisplayed || !isConsentRequested)
            {
                var pressedButton = await AsyncUtils.Utils.WaitFirstButtonPressedAsync(
                    ct, _acceptButton, _dontAcceptButton);
                PlayerPrefs.SetInt(PlayerPrefsKeyDisplayed, 1);

                var gaveConsent = pressedButton == _acceptButton;
                PlayerPrefs.SetInt(PlayerPrefsKeyGaveConsent, gaveConsent ? 1 : 0);

                return gaveConsent;
            }

            return UserGaveConsent;
        }

#if UNITY_EDITOR
        [ContextMenu("Remove player prefs data")]
        public void EditorOnly_RemovePlayerPrefsData()
        {
            
        }
#endif
    }
}