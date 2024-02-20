#if ENABLED_ANALYTICS

using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using SerializableCallback;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Serialization;

namespace ServicesUtils.Analytics
{
    public class AnalyticsInitializer : MonoBehaviour
    {
        [SerializeField] private SerializableCallback<CancellationToken, Task<bool>> _consentFlow;
        
        [Obsolete("Use _consentFlow instead")]
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
                var userGaveConsent = await _consentFlow.Invoke(ct);

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