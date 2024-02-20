#if ENABLED_PURCHASING

using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ServicesUtils.Iap
{
    public class IapButtonPriceTextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private CodelessIAPButton _iapButton;
        [SerializeField] private GameObject _loadingObject;

        private void Start()
        {
            if (_loadingObject != null)
            {
                _loadingObject.SetActive(true);
                _priceText.gameObject.SetActive(false);
            }
            
            var productId = _iapButton.productId;
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product == null)
            {
                _iapButton.onProductFetched.AddListener(UpdateProductPrice);
            }
            else
            {
                UpdateProductPrice(product);
            }
        }

        private void UpdateProductPrice(Product product)
        {
            if (_loadingObject != null)
            {
                _loadingObject.SetActive(false);
                _priceText.gameObject.SetActive(true);
            }
            
            var price = product.metadata.localizedPriceString;
            var currency = product.metadata.isoCurrencyCode;

            _priceText.text = $"{price} {currency}";
        }
    }
}

#endif