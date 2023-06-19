﻿#if ENABLED_ADSLEGACY

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace ServicesUtils.AdsLegacy
{
    public class AdUnit : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
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
        
        void Awake()
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
            await Task.Delay((int)(_reloadAfterFailureTime * 1000));
            LoadAd();
        }
    }
}

#endif