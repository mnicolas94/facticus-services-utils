#if ENABLED_ANALYTICS

using System.Collections.Generic;
using System.Threading;
using AsyncUtils;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Serialization;

namespace ServicesUtils.Analytics
{
    public class AnalyticsInitializer : MonoBehaviour
    {
        [FormerlySerializedAs("_popup")]
        [SerializeField] private AsyncPopupReturnable<bool> _privacyPolicyPopup;
        
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }
        
        public async void Initialize()
        {
            try
            {
                var ct = _cts.Token;
                var userGaveConsent = await Popups.ShowPopup(_privacyPolicyPopup, ct);

                if (userGaveConsent)
                {
                    AnalyticsService.Instance.StartDataCollection();
                }
            }
            catch (ConsentCheckException e)
            {
                // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
                Debug.LogError(e.Message);
            }
        }
    }
}

#endif