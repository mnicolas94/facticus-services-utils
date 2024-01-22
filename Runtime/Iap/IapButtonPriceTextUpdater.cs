using System;
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
            
            _iapButton.onProductFetched.AddListener(OnProductFetched);
        }

        private void OnProductFetched(Product product)
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