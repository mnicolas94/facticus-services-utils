using System.Threading;
using System.Threading.Tasks;
using SerializableCallback;
using UnityEngine;
using UnityEngine.Events;

namespace ServicesUtils.AdsCommon
{
    public class AdDisplayerCallbacks : MonoBehaviour, IAdsDisplayer
    {
        [SerializeField] private SerializableEvent _initialize;
        [SerializeField] private SerializableValueCallback<bool> _isReady;
        [SerializeField] private SerializableCallback<CancellationToken, Task<bool>> _waitToBeReady;
        [SerializeField] private SerializableEvent _displayAd;
        [SerializeField] private SerializableCallback<UnityEvent> _adCompletedEventGetter;
        [SerializeField] private SerializableCallback<UnityEvent> _adSkippedEventGetter;
        [SerializeField] private SerializableCallback<UnityEvent> _adFailedEventGetter;

        public void Initialize()
        {
            _initialize?.Invoke();
        }

        public bool IsReady()
        {
            return _isReady.Value;
        }

        public async Task<bool> WaitToBeReadyAsync(CancellationToken ct)
        {
            var waitToBeReadyTask = _waitToBeReady.Invoke(ct);
            if (waitToBeReadyTask == null)
            {
                return true;
            }

            return await waitToBeReadyTask;
        }

        public async Task<AdResult> DisplayAdAsync(CancellationToken ct)
        {
            _displayAd?.Invoke();
            
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
            
            var adCompletedEvent = _adCompletedEventGetter.Invoke();
            var adSkippedEvent = _adSkippedEventGetter.Invoke();
            var adFailedEvent = _adFailedEventGetter.Invoke();
            
            adCompletedEvent?.AddListener(Local_OnShowComplete);
            adSkippedEvent?.AddListener(Local_OnShowSkipped);
            adFailedEvent?.AddListener(Local_OnShowFailure);
            
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
                adCompletedEvent?.RemoveListener(Local_OnShowComplete);
                adSkippedEvent?.RemoveListener(Local_OnShowSkipped);
                adFailedEvent?.RemoveListener(Local_OnShowFailure);
            }
        }
    }
}