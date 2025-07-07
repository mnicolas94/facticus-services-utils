#if ENABLED_ADSLEGACY

using System;
using System.Threading;
using System.Threading.Tasks;
using ServicesUtils.AdsCommon;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace ServicesUtils.AdsLegacy
{
    [Obsolete("Use AdUnitData instead")]
    public class AdUnit : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IAdsDisplayer
    {
        [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
        [SerializeField] private string _iOsAdUnitId = "Interstitial_iOS";

        [SerializeField] private bool _reloadAfterShow;
        [SerializeField] private bool _reloadAfterFailure;
        [SerializeField] private float _reloadAfterFailureTime = 5;

        [SerializeField] private UnityEvent _onGameObjectStart;
        [SerializeField] private UnityEvent _onAdLoaded;
        [SerializeField] private UnityEvent _onFailedToLoad;
        [SerializeField] private UnityEvent _onShowFailure;
        [SerializeField] private UnityEvent _onShowStart;
        [SerializeField] private UnityEvent _onShowClick;
        [SerializeField] private UnityEvent _onShowComplete;
        [SerializeField] private UnityEvent _onShowSkipped;
 
        private string _adUnitId;
        
        private void Awake()
        {
            _adUnitId = GetAddUnit();
        }

        private void Start()
        {
            _onGameObjectStart.Invoke();
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
            Debug.Log("Loading Ad: " + _adUnitId);
            Advertisement.Load(_adUnitId, this);
        }
 
        // Show the loaded content in the Ad Unit:
        [ContextMenu("Show")]
        public void ShowAd()
        {
            // Note that if the ad content wasn't previously loaded, this method will fail
            Debug.Log("Showing Ad: " + _adUnitId);
            Advertisement.Show(_adUnitId, this);
        }
 
        // Implement Load Listener and Show Listener interface methods: 
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Ad loaded");
            _onAdLoaded.Invoke();
        }
 
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
            _onFailedToLoad.Invoke();

            if (_reloadAfterFailure)
            {
                ReloadAfterTime();
            }
        }

        public void OnUnityAdsShowStart(string adUnitId)
        {
            Debug.Log("Ad show start");
            _onShowStart.Invoke();
        }

        public void OnUnityAdsShowClick(string adUnitId)
        {
            Debug.Log("Ad show click");
            _onShowClick.Invoke();
        }
 
        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
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
                Debug.Log("Ad show complete");
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
            await AsyncUtils.Utils.Delay(_reloadAfterFailureTime, destroyCancellationToken);
            if (!destroyCancellationToken.IsCancellationRequested)
            {
                LoadAd();
            }
        }

        public void Load()
        {
            LoadAd();
        }

        public bool IsLoaded()
        {
            return Advertisement.isInitialized;
        }

        public bool IsLoading()
        {
            return false;
        }

        public async Task<bool> LoadAsync(CancellationToken ct)
        {
            LoadAd();
            
            var loaded = false;
            var success = false;

            void OnLoad()
            {
                loaded = true;
                success = true;
            }

            void OnLoadFailed()
            {
                loaded = true;
                success = false;
            }
            
            _onAdLoaded.AddListener(OnLoad);
            _onFailedToLoad.AddListener(OnLoadFailed);

            try
            {
                while (!loaded && !ct.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                return success;
            }
            finally
            {
                _onAdLoaded.RemoveListener(OnLoad);
                _onFailedToLoad.RemoveListener(OnLoadFailed);
            }
        }

        public async Task<AdResult> DisplayAdAsync(CancellationToken ct)
        {
            ShowAd();
            
            var displayed = false;
            var result = AdResult.Failed;

            void Local_OnShowFailure()
            {
                displayed = true;
                result = AdResult.Failed;
            }

            void Local_OnShowComplete()
            {
                displayed = true;
                result = AdResult.Completed;
            }
            
            void Local_OnShowSkipped()
            {
                displayed = true;
                result = AdResult.Skipped;
            }
            
            _onShowFailure.AddListener(Local_OnShowFailure);
            _onShowComplete.AddListener(Local_OnShowComplete);
            _onShowSkipped.AddListener(Local_OnShowSkipped);
            
            try
            {
                while (!displayed && !ct.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                return result;
            }
            finally
            {
                _onShowFailure.RemoveListener(Local_OnShowFailure);
                _onShowComplete.RemoveListener(Local_OnShowComplete);
                _onShowSkipped.RemoveListener(Local_OnShowSkipped);
            }
        }
    }
}

#endif