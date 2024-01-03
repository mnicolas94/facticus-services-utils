#if ENABLED_SERVICESCORE

using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Events;
using SerializableCallback;

namespace ServicesUtils
{
    public class ServicesInitialization : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onInitialized;
        [SerializeField] private SerializableValueCallback<string> _environmentName;

        private async void Start()
        {
            try
            {
                var environmentName = _environmentName.Value;
                var options = new InitializationOptions();
                options.SetEnvironmentName(environmentName);
                
                await UnityServices.InitializeAsync(options);
                _onInitialized.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}

#endif