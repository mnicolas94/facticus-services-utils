#if ENABLED_ANALYTICS

using System.Collections.Generic;
using System.Threading;
using AsyncUtils;
using Unity.Services.Analytics;
using UnityEngine;

namespace ServicesUtils.Analytics
{
    public class AnalyticsInitializer : MonoBehaviour
    {
        [SerializeField] private AsyncPopup _popup;
        
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
                await Popups.ShowPopup(_popup, ct);
                
                List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
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