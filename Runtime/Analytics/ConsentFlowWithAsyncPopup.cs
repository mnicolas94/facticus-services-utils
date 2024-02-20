using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using UnityEngine;

namespace ServicesUtils.Analytics
{
    public class ConsentFlowWithAsyncPopup : MonoBehaviour
    {
        [SerializeField] private AsyncPopupReturnable<bool> _privacyPolicyPopup;

        public async Task<bool> DisplayConsentPopup(CancellationToken ct)
        {
            var userGaveConsent = await Popups.ShowPopup(_privacyPolicyPopup, ct);
            return userGaveConsent;
        }
    }
}