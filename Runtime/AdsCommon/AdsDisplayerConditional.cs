using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SerializableCallback;
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
            if (IsLoaded())
            {
                await DisplayAdAsync(destroyCancellationToken);
            }
        }
        
        public async void LoadAndDisplayAd()
        {
            var displayer = GetDisplayer();
            var isLoaded = await displayer.LoadAsync(destroyCancellationToken);
            if (isLoaded)
            {
                await DisplayAdAsync(destroyCancellationToken);
            }
        }
        
        public async void Load()
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                await displayer.LoadAsync(destroyCancellationToken);
            }
        }

        public bool IsLoaded()
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                return displayer.IsLoaded();
            }

            return false;
        }

        public bool IsLoading()
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                return displayer.IsLoading();
            }

            return false;
        }

        public async Task<bool> LoadAsync(CancellationToken ct)
        {
            var displayer = GetDisplayer();
            if (displayer != null)
            {
                return await displayer.LoadAsync(ct);
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