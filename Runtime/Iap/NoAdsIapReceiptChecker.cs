#if ENABLED_PURCHASING

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils.Attributes;

namespace ServicesUtils.Iap
{
    [CreateAssetMenu(fileName = "NoAdsIapReceiptChecker", menuName = "Services/NoAdsIapReceiptChecker")]
    public class NoAdsIapReceiptChecker : ScriptableObject
    {
        [SerializeField, Dropdown(nameof(LoadCatalogProducts))] private string _product;
        
        public bool HasReceipt()
        {
            var storeController = CodelessIAPStoreListener.Instance.StoreController;
            if (storeController == null) return false;
            
            var hasReceipt = storeController.products.WithID(_product).hasReceipt;
            return hasReceipt;
        }
        
        [ContextMenu("HR")]
        public void DebugHasReceipt()
        {
            var hasReceipt = HasReceipt();
            Debug.Log($"hasReceipt: {hasReceipt}");
        }
        
        private List<string> LoadCatalogProducts()
        {
            var catalog = ProductCatalog.LoadDefaultCatalog();

            var products = catalog.allProducts.Select(product => product.id).ToList();
            return products;
        }
    }
}

#endif