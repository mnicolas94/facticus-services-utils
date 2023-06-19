using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using UnityEngine;
using UnityEngine.UI;

namespace ServicesUtils.Analytics
{
    public class PrivacyPolicyPopup : AsyncPopup
    {
        private const string PlayerPrefsKey = "privacypolicy_displayed";

        [SerializeField] private GameObject _popupObject;
        [SerializeField] private Button _acceptButton;

        private bool AlreadyDisplayed => PlayerPrefs.GetInt(PlayerPrefsKey, 0) == 1;

        public override void Initialize()
        {
            _popupObject.SetActive(!AlreadyDisplayed);
        }
        
        public override async Task Show(CancellationToken ct)
        {
            if (!AlreadyDisplayed)
            {
                await AsyncUtils.Utils.WaitPressButtonAsync(_acceptButton, ct);
                PlayerPrefs.SetInt(PlayerPrefsKey, 1);
            }
        }
    }
}