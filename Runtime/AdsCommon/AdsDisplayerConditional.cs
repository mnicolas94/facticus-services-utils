using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SerializableCallback;
using ServicesUtils.AdsLegacy;
using TNRD;
using UnityEngine;

namespace ServicesUtils.AdsCommon
{
    public class AdsDisplayerConditional : MonoBehaviour, IAdsDisplayer
    {
        [SerializeField] private List<ConditionalDisplayer> _displayers;

        private IAdsDisplayer GetDisplayer()
        {
            foreach (var (condition, displayer) in _displayers)
            {
                if (condition.Invoke())
                {
                    return displayer.Value;
                }
            }

            return null;
        }

        public async void DisplayAd()
        {
            await DisplayAdAsync(destroyCancellationToken);
        }
        
        public async void DisplayAd(AdTimer timer)
        {
            if (timer.ShouldShow())
            {
                await DisplayAdAsync(destroyCancellationToken);
                timer.UpdateTimer();
            }
        }
        
        public async void DisplayAdIfReady()
        {
            if (IsReady())
            {
                await DisplayAdAsync(destroyCancellationToken);
            }
        }
        
        public async void LoadAndDisplayAd()
        {
            Initialize();
            await WaitToBeReadyAsync(destroyCancellationToken);
            await DisplayAdAsync(destroyCancellationToken);
        }
        
        public void Initialize()
        {
            var displayer = GetDisplayer();
            displayer?.Initialize();
        }

        public bool IsReady()
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                return displayer.IsReady();
            }

            return false;
        }

        public async Task<bool> WaitToBeReadyAsync(CancellationToken ct)
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                return await displayer.WaitToBeReadyAsync(ct);
            }

            return false;
        }

        public async Task<AdResult> DisplayAdAsync(CancellationToken ct)
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                return await displayer.DisplayAdAsync(ct);
            }

            return AdResult.Failed;
        }
    }

    [Serializable]
    public class ConditionalDisplayer
    {
        [SerializeField] private SerializableCallback<bool> _condition;
        public SerializableCallback<bool> Condition => _condition;
        
        [SerializeField] private SerializableInterface<IAdsDisplayer> _displayer;
        public SerializableInterface<IAdsDisplayer> Displayer => _displayer;

        public void Deconstruct(out SerializableCallback<bool> condition, out SerializableInterface<IAdsDisplayer> displayer)
        {
            condition = _condition;
            displayer = _displayer;
        }
    }
}