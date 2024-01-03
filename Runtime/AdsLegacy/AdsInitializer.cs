#if ENABLED_ADSLEGACY

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using SerializableCallback;

namespace ServicesUtils.AdsLegacy
{
    [Serializable]
    public class BoolCallback : SerializableCallback<bool>{}
    
    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField] private string _androidGameId;
        [SerializeField] private string _iOSGameId;
        [SerializeField] private BoolCallback _testModeCallback;

        [SerializeField] private UnityEvent _onInitializationComplete;
        [SerializeField] private UnityEvent _onInitializationFailed;
        
        private string _gameId;
        private bool _initialized;
        private bool _failed;

        public static bool IsMobile => Application.platform is RuntimePlatform.Android or RuntimePlatform.IPhonePlayer
                                 or RuntimePlatform.WindowsEditor or RuntimePlatform.LinuxEditor
                                 or RuntimePlatform.OSXEditor;
 
        public void Initialize()
        {
            if (!IsMobile)  // initialize Ads only on mobile devices or in editor
            {
                return;
            }
            
            _initialized = false;
            _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? _iOSGameId
                : _androidGameId;
            var testMode = _testModeCallback.Invoke();
            Advertisement.Initialize(_gameId, testMode, this);
        }

        public async Task<bool> WaitForInitialization(CancellationToken ct)
        {
            while (!_initialized && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }

            return _failed;
        }
 
        public void OnInitializationComplete()
        {
            Debug.Log("Ads service initialized");
            
            _initialized = true;
            _failed = false;
            _onInitializationComplete.Invoke();
        }
 
        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Error initializing Ads service: {error.ToString()} - {message}");

            _initialized = true;
            _failed = true;
            _onInitializationFailed.Invoke();
        }
    }
}

#endif