#if ENABLED_PURCHASING

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils.Attributes;

namespace ServicesUtils.Iap
{
    public class IapProductTextUpdater : MonoBehaviour
    {
        [SerializeField, Dropdown(nameof(LoadCatalogProducts))] private string _productId;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _price;

        [SerializeField] private string _priceCurrencySymbol;

        private bool _observeInitialization;
        
        void OnEnable()
        {
            if (CodelessIAPStoreListener.initializationComplete)
            {
                UpdateText();
            }
        }

        private void Start()
        {
            _observeInitialization = !CodelessIAPStoreListener.initializationComplete;
        }

        private void Update()
        {
            if (_observeInitialization && CodelessIAPStoreListener.initializationComplete)
            {
                _observeInitialization = false;
                UpdateText();
            }
        }

        private void UpdateText()
        {
            var product = CodelessIAPStoreListener.Instance.GetProduct(_productId);
            if (product != null)
            {
                if (_title != null)
                {
                    _title.text = product.metadata.localizedTitle;
                }

                if (_description != null)
                {
                    _description.text = product.metadata.localizedDescription;
                }

                if (_price != null)
                {
                    var price = product.metadata.localizedPriceString;
                    price = DecoratePriceTextWithCurrencySymbol(price);
                    _price.text = price;
                }
            }
        }

        private string DecoratePriceTextWithCurrencySymbol(string price)
        {
            if (!price.StartsWith(_priceCurrencySymbol))
            {
                price = $"{_priceCurrencySymbol}{price}";
            }

            return price;
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