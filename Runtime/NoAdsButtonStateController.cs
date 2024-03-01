#if ENABLED_ADSLEGACY && ENABLED_PURCHASING

using System.Threading;
using System.Threading.Tasks;
using ServicesUtils.AdsLegacy;
using ServicesUtils.Iap;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;

namespace ServicesUtils
{
    public class NoAdsButtonStateController : MonoBehaviour
    {
        [SerializeField] private NoAdsIapReceiptChecker _checker;
        [SerializeField] private GameObject _target;
        
        private async void Start()
        {
            var initialized = await WaitForeServicesToBeInitialized(destroyCancellationToken);
            if (initialized)
            {
                UpdateState();
            }
        }

        private async Task<bool> WaitForeServicesToBeInitialized(CancellationToken ct)
        {
            var bothServicesInitialized = CodelessIAPStoreListener.initializationComplete && Advertisement.isInitialized;
            while (!bothServicesInitialized && !ct.IsCancellationRequested)
            {
                await Task.Yield();
                bothServicesInitialized = CodelessIAPStoreListener.initializationComplete && Advertisement.isInitialized;
            }

            return bothServicesInitialized;
        }
        
        public void UpdateState()
        {
            var hasReceipt = _checker.HasReceipt();
            var isMobile = AdsInitializer.IsMobile;
            var active = !hasReceipt && isMobile;
            _target.SetActive(active);
        }
    }
}

#endif