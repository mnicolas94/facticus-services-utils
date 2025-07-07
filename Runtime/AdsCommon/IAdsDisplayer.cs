using System.Threading;
using System.Threading.Tasks;

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