using SerializableCallback;
#if ENABLED_ADSLEGACY
using ServicesUtils.AdsLegacy;
#endif
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace ServicesUtils.AdsCommon
{
    [MovedFrom(sourceNamespace: "ServicesUtils.AdsLegacy")]
    [CreateAssetMenu(fileName = "AdTimer", menuName = "ServicesUtils/Ads/AdTimer")]
    public class AdTimer : ScriptableObject
    {
        [SerializeField] private float _firstTimePeriod;
        [SerializeField] private float _adTimePeriod;
        [SerializeField] private SerializableValueCallback<bool> _additionalConditions;
        [SerializeField] private SerializableValueCallback<bool> _forceAdsCallback;

        private float _lasTimeDisplayedAd;
        private bool _displayedAtLeastOnce;
        
        private float TimePeriod => _displayedAtLeastOnce ? _adTimePeriod : _firstTimePeriod;
        
        public bool ItsTimeToDisplay => Time.time - _lasTimeDisplayedAd >= TimePeriod;

        private void OnEnable()
        {
            UpdateTimer();
            _displayedAtLeastOnce = false;
        }
        
        public void UpdateTimer()
        {
            _displayedAtLeastOnce = true;
            _lasTimeDisplayedAd = Time.time;
        }

#if ENABLED_ADSLEGACY
        public void ShowAdIfShould(AdUnit adUnit)
        {
            if (ShouldShow() && adUnit.IsReady())
            {
                adUnit.ShowAd();
                UpdateTimer();
            }
        }
        
        public void ShowAdIfShould(AdUnitData adUnit)
        {
            if (ShouldShow() && adUnit.IsReady())
            {
                adUnit.ShowAd();
                UpdateTimer();
            }
        }
#endif
        
        public bool ShouldShow()
        {
            var forceAds = _forceAdsCallback.Value;
            var meetsAdditionalConditions = _additionalConditions.Value;
            return (ItsTimeToDisplay && meetsAdditionalConditions) || forceAds;
        }
    }
}