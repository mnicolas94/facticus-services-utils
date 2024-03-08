using System.Threading;
using System.Threading.Tasks;

namespace ServicesUtils.AdsCommon
{
    public interface IAdsDisplayer
    {
        void Initialize();
        bool IsReady();
        Task<bool> WaitToBeReadyAsync(CancellationToken ct);
        Task<AdResult> DisplayAdAsync(CancellationToken ct);
    }

    public enum AdResult
    {
        Completed,
        Failed,
        Skipped,
    }
}