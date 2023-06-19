using ServicesUtils.AdsLegacy;
using ServicesUtils.Iap;
using UnityEngine;

namespace ServicesUtils
{
    public class NoAdsButtonStateController : MonoBehaviour
    {
        [SerializeField] private NoAdsIapReceiptChecker _checker;
        [SerializeField] private GameObject _target;
        
        public void UpdateState()
        {
            var hasReceipt = _checker.HasReceipt();
            var isMobile = AdsInitializer.IsMobile;
            var active = !hasReceipt && isMobile;
            _target.SetActive(active);
        }
    }
}