using UnityEngine;

namespace ServicesUtils.AdsCommon
{
    [CreateAssetMenu(fileName = "AdsLibraryConfiguration", menuName = "ServicesUtils/Ads/AdsLibrary/LibraryConfig", order = 0)]
    public class AdsLibraryConfiguration : ScriptableObject
    {
        [SerializeField] private AdsLibraryEnum _libraryToUse;

        public bool IsLibrary(AdsLibraryEnum library)
        {
            return _libraryToUse == library;
        }
    }
}