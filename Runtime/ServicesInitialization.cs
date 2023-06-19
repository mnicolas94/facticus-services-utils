#if ENABLED_SERVICESCORE

using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Events;

namespace ServicesUtils
{
    [Serializable]
    public class StringCallback : SerializableCallback<string>{}

    public class ServicesInitialization : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onInitialized;
        [SerializeField] private StringCallback _getEnvironmentNameCallback;

        private async void Start()
        {
            try
            {
                var environmentName = _getEnvironmentNameCallback.Invoke();
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