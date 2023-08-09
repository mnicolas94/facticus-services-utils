#if ENABLED_ADSLEGACY

using UnityEngine;

namespace ServicesUtils.AdsLegacy
{
    [CreateAssetMenu(fileName = "AdTimer", menuName = "ServicesUtils/Ads/AdTimer")]
    public class AdTimer : ScriptableObject
    {
        [SerializeField] private float _firstTimePeriod;
        [SerializeField] private float _adTimePeriod;
        [SerializeField] private BoolCallback _additionalConditions;
        [SerializeField] private BoolCallback _forceAdsCallback;

        private float _lasTimeDisplayedAd;
        private bool _displayedAtLeastOnce;
        
        private float TimePeriod => _displayedAtLeastOnce ? _adTimePeriod : _firstTimePeriod;
        
        public bool ItsTimeToDisplay => Time.time - _lasTimeDisplayedAd >= TimePeriod;

        private void OnEnable()
        {
            _displayedAtLeastOnce = false;
            UpdateTimer();
        }
        
        public void UpdateTimer()
        {
            _lasTimeDisplayedAd = Time.time;
        }

        public void ShowAdIfShould(AdUnit adUnit)
        {
            if (ShouldShow())
            {
                adUnit.ShowAd();
                _displayedAtLeastOnce = true;
                UpdateTimer();
            }
        }
        
        public void ShowAdIfShould(AdUnitData adUnit)
        {
            if (ShouldShow())
            {
                adUnit.ShowAd();
                _displayedAtLeastOnce = true;
                UpdateTimer();
            }
        }

        private bool ShouldShow()
        {
            var forceAds = _forceAdsCallback.Invoke();
            var meetsAdditionalConditions = _additionalConditions.target == null || _additionalConditions.Invoke();
            return (ItsTimeToDisplay && meetsAdditionalConditions) || forceAds;
        }
    }
}

#endif