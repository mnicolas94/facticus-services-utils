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

        private bool AlreadyDisplayed
        {
            get => PlayerPrefs.GetInt(PlayerPrefsKeyDisplayed, 0) == 1;
            set => PlayerPrefs.SetInt(PlayerPrefsKeyDisplayed, value ? 1 : 0);
        }

        private bool UserGaveConsent
        {
            get => PlayerPrefs.GetInt(PlayerPrefsKeyGaveConsent, 0) == 1;
            set => PlayerPrefs.SetInt(PlayerPrefsKeyGaveConsent, value ? 1 : 0);
        }

        public override void Initialize()
        {
            // backward compatibility
            var isConsentStored = PlayerPrefs.HasKey(PlayerPrefsKeyGaveConsent);
            if (AlreadyDisplayed && !isConsentStored)
            {
                AlreadyDisplayed = false;
            }
            
            _popupObject.SetActive(!AlreadyDisplayed);
        }
        
        public override async Task<bool> Show(CancellationToken ct)
        {
            if (!AlreadyDisplayed)
            {
                var pressedButton = await AsyncUtils.Utils.WaitFirstButtonPressedAsync(
                    ct, _acceptButton, _dontAcceptButton);
                AlreadyDisplayed = true;

                var gaveConsent = pressedButton == _acceptButton;
                UserGaveConsent = gaveConsent;

                return gaveConsent;
            }

            return UserGaveConsent;
        }

#if UNITY_EDITOR
        [ContextMenu("Remove player prefs data")]
        private void EditorOnly_RemovePlayerPrefsData()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeyDisplayed);
            PlayerPrefs.DeleteKey(PlayerPrefsKeyGaveConsent);
        }
#endif
    }
}