#if ENABLED_PURCHASING

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace ServicesUtils.Iap
{
    public class IapInitializer : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onIAPInitialized;
        
        
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
        
        public void Initialize()
        {
            // this will initialize the service if it is not initialized yet
            var instance = CodelessIAPStoreListener.Instance;
            NotifyInitialization();
        }

        private async void NotifyInitialization()
        {
            var ct = _cts.Token;
            while (!CodelessIAPStoreListener.initializationComplete && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }

            if (CodelessIAPStoreListener.initializationComplete)
            {
                _onIAPInitialized.Invoke();
            }
        }
    }
}

#endif