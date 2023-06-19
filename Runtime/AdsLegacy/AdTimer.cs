#if ENABLED_ADSLEGACY && ENABLED_PURCHASING

using ServicesUtils.Iap;
using UnityEngine;

namespace ServicesUtils.AdsLegacy
{
    [CreateAssetMenu(fileName = "AdTimer", menuName = "Services/Ads/AdTimer")]
    public class AdTimer : ScriptableObject
    {
        [SerializeField] private float _adTimePeriod;
        [SerializeField] private NoAdsIapReceiptChecker _receiptChecker;
        [SerializeField] private BoolCallback _forceAdsCallback;

        private float _lasTimeDisplayedAd;
        
        public bool ItsTimeToDisplay => Time.time - _lasTimeDisplayedAd >= _adTimePeriod;

        private void OnEnable()
        {
            UpdateLastTime();
        }
        
        private void UpdateLastTime()
        {
            _lasTimeDisplayedAd = Time.time;
        }

        public void ShowAdIfShould(AdUnit adUnit)
        {
            var forceAds = _forceAdsCallback.Invoke();
            if ((ItsTimeToDisplay && !_receiptChecker.HasReceipt()) || forceAds)
            {
                adUnit.ShowAd();
                UpdateLastTime();
            }
        }
    }
}

#endif