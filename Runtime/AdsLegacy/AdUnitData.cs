﻿#if ENABLED_ADSLEGACY

using System;
using System.Threading;
using System.Threading.Tasks;
using ServicesUtils.AdsCommon;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace ServicesUtils.AdsLegacy
{
    [CreateAssetMenu(fileName = "AdUnit", menuName = "ServicesUtils/Ads/AdUnit")]
    public class AdUnitData : ScriptableObject, IUnityAdsLoadListener, IUnityAdsShowListener, IAdsDisplayer
    {
        [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
        [SerializeField] private string _iOsAdUnitId = "Interstitial_iOS";

        [SerializeField] private bool _reloadAfterShow;
        [SerializeField] private bool _reloadAfterFailure;
        [SerializeField] private float _reloadAfterFailureTime = 5;

        [SerializeField] private UnityEvent _onAdLoaded;
        [SerializeField] private UnityEvent<string> _onFailedToLoad;
        [SerializeField] private UnityEvent<string> _onShowFailure;
        [SerializeField] private UnityEvent _onShowStart;
        [SerializeField] private UnityEvent _onShowClick;
        [SerializeField] private UnityEvent _onShowComplete;
        [SerializeField] private UnityEvent _onShowSkipped;
        
        private string _adUnitId;
        private bool _loaded;
        private bool _loading;

        public UnityEvent OnAdLoaded => _onAdLoaded;

        public UnityEvent<string> OnFailedToLoad => _onFailedToLoad;

        public UnityEvent<string> OnShowFailure => _onShowFailure;

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
            
            var errorMessage = $"Error loading Ad Unit {adUnitId}: {error} - {message}";
            _onFailedToLoad.Invoke(errorMessage);

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
            var errorMessage = $"Error showing Ad Unit {adUnitId}: {error} - {message}";
            _onShowFailure.Invoke(errorMessage);
            
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
            var ct = Application.exitCancellationToken;
            await AsyncUtils.Utils.Delay(_reloadAfterFailureTime, ct);
            if (!ct.IsCancellationRequested)
            {
                LoadAd();
            }
        }

        [ContextMenu("Load")]
        public async void LoadAd()
        {
            _loading = true;

            while (!Advertisement.isInitialized && !Application.exitCancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
            }

            if (Application.exitCancellationToken.IsCancellationRequested) return;
            
            Advertisement.Load(_adUnitId, this);
        }
        
        [Obsolete("Use LoadAd() instead")]
        public void Load()
        {
            LoadAd();
        }

        public bool IsLoaded()
        {
            return _loaded;
        }

        public bool IsLoading()
        {
            return _loading;
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

            void OnLoadFailed(string _)
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

            void Local_OnShowFailure(string _)
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