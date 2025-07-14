using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ServicesUtils.AdsCommon
{
    public interface IAdsDisplayer
    {
        bool IsLoaded();
        bool IsLoading();
        Task<bool> LoadAsync(CancellationToken ct);
        Task<AdResult> DisplayAdAsync(CancellationToken ct);
    }
    
    public static class AdsDisplayerExtensions
    {
        /// <summary>
        /// Loads the ad, and keeps trying with a limited number of attempts
        /// </summary>
        /// <param name="displayer"></param>
        /// <param name="reloadAttempts"></param> number of attempts
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<bool> LoadAsync(this IAdsDisplayer displayer, int reloadAttempts, CancellationToken ct)
        {
            var isLoaded = false;
            for (var i = 0; i < reloadAttempts; i++)
            {
                isLoaded = await displayer.LoadAsync(ct);
                if (isLoaded)
                {
                    break;
                }
            }
            return isLoaded;
        }
        
        /// <summary>
        /// Loads the ad, and keeps trying with a limited number of attempts. If it is still not loaded after those
        /// attempts, it will try again after a delay.
        /// Notice it will keep trying forever as long as the Ad is not loaded,
        /// or the cancellation token is not cancelled.
        /// </summary>
        /// <param name="displayer"></param>
        /// <param name="reloadAttempts"></param>
        /// <param name="reloadTimeAfterFailure"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<bool> LoadAsync(this IAdsDisplayer displayer, int reloadAttempts, float reloadTimeAfterFailure,
            CancellationToken ct)
        {
            var isLoaded = false;
            while (!isLoaded && !ct.IsCancellationRequested)
            {
                isLoaded = await displayer.LoadAsync(reloadAttempts, ct);
                if (!isLoaded)
                {
                    await Awaitable.WaitForSecondsAsync(reloadTimeAfterFailure, ct);
                }
            }
            return isLoaded;
        }

        public static async Task WaitToBeReadyAsync(this IAdsDisplayer displayer, CancellationToken ct)
        {
            while (!displayer.IsLoaded() && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
    }

    public enum AdResult
    {
        Completed,
        Failed,
        Skipped,
    }
}