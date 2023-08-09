using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace ServicesUtils.AdsLegacy
{
    [CreateAssetMenu(fileName = "AdUnit", menuName = "Facticus/ServicesUtils/AdUnit")]
    public class AdUnitData : ScriptableObject, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
        [SerializeField] private string _iOsAdUnitId = "Interstitial_iOS";

        [SerializeField] private bool _reloadAfterShow;
        [SerializeField] private bool _reloadAfterFailure;
        [SerializeField] private float _reloadAfterFailureTime = 5;

        [SerializeField] private UnityEvent _onAdLoaded;
        [SerializeField] private UnityEvent _onFailedToLoad;
        [SerializeField] private UnityEvent _onShowFailure;
        [SerializeField] private UnityEvent _onShowStart;
        [SerializeField] private UnityEvent _onShowClick;
        [SerializeField] private UnityEvent _onShowComplete;
        [SerializeField] private UnityEvent _onShowSkipped;
        
        private string _adUnitId;
        private bool _loaded;
        private bool _loading;

        public UnityEvent OnAdLoaded => _onAdLoaded;

        public UnityEvent OnFailedToLoad => _onFailedToLoad;

        public UnityEvent OnShowFailure => _onShowFailure;

        public UnityEvent OnShowStart => _onShowStart;

        public UnityEvent OnShowClick => _onShowClick;

        public UnityEvent OnShowComplete => _onShowComplete;

        public UnityEvent OnShowSkipped => _onShowSkipped;

        public bool Loaded => _loaded;

        public bool Loading => _loading;

        private void OnEnable()
        {
            _adUnitId = GetAddUnit();
            _loaded = false;
            _loading = false;
        }

        private string GetAddUnit()
        {
            return Application.platform == RuntimePlatform.IPhonePlayer
                ? _iOsAdUnitId
                : _androidAdUnitId;
        }

        [ContextMenu("Load")]
        public void LoadAd()
        {
            Advertisement.Load(_adUnitId, this);
        }
 
        // Show the loaded content in the Ad Unit:
        [ContextMenu("Show")]
        public void ShowAd()
        {
            Advertisement.Show(_adUnitId, this);
        }
 
        // Implement Load Listener and Show Listener interface methods: 
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            _loaded = true;
            _loading = false;
            _onAdLoaded.Invoke();
        }
 
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            _loaded = false;
            _loading = false;
            _onFailedToLoad.Invoke();

            if (_reloadAfterFailure)
            {
                ReloadAfterTime();
            }
        }

        public void OnUnityAdsShowStart(string adUnitId)
        {
            _onShowStart.Invoke();
        }

        public void OnUnityAdsShowClick(string adUnitId)
        {
            _onShowClick.Invoke();
        }
 
        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            _onShowFailure.Invoke();
            
            if (_reloadAfterShow)
            {
                LoadAd();
            }
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                _onShowComplete.Invoke();
            }
            else
            {
                _onShowSkipped.Invoke();
            }
            
            if (_reloadAfterShow)
            {
                LoadAd();
            }
        }

        private async void ReloadAfterTime()
        {
            await Task.Delay((int)(_reloadAfterFailureTime * 1000));
            LoadAd();
        }
    }
}