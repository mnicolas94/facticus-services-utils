using System.Threading;
using System.Threading.Tasks;
using SerializableCallback;
using UnityEngine;

namespace ServicesUtils.AdsCommon
{
    public class AdDisplayerCallbacks : MonoBehaviour, IAdsDisplayer
    {
        [SerializeField] private SerializableEvent _initialize;
        [SerializeField] private SerializableValueCallback<bool> _isReady;
        [SerializeField] private SerializableCallback<CancellationToken, Task<bool>> _waitToBeReady;
        [SerializeField] private SerializableCallback<CancellationToken, Task<AdResult>> _displayAd;

        public void Initialize()
        {
            _initialize?.Invoke();
        }

        public bool IsReady()
        {
            return _isReady.Value;
        }

        public Task<bool> WaitToBeReadyAsync(CancellationToken ct)
        {
            return _waitToBeReady.Invoke(ct);
        }

        public Task<AdResult> DisplayAdAsync(CancellationToken ct)
        {
            return _displayAd.Invoke(ct);
        }
    }
}