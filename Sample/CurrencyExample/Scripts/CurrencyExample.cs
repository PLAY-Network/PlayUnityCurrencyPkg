using System.Collections.Generic;
using System.Threading.Tasks;
using RGN.Impl.Firebase;
using RGN.Modules.Currency;
using RGN.UI;
using UnityEngine;

namespace RGN.Samples
{
    public sealed class CurrencyExample : IUIScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private LoadingIndicator _fullScreenLoadingIndicator;
        [SerializeField] private RectTransform _rgnCoinItemsContent;
        [SerializeField] private RectTransform _customCoinItemsContent;

        [Header("Prefabs")]
        [SerializeField] private RGNCoinItem _rgnCoinItemPrefab;
        [SerializeField] private CustomCoinItem _customCoinItemPrefab;

        private bool _triedToLoad;
        private List<RGNCoinItem> _rgnCoinItems;
        private List<CustomCoinItem> _customCoinItems;

        public override void PreInit(IRGNFrame rgnFrame)
        {
            base.PreInit(rgnFrame);
            _rgnCoinItems = new List<RGNCoinItem>();
            _customCoinItems = new List<CustomCoinItem>();
        }
        protected override async void OnShow()
        {
            if (_triedToLoad)
            {
                return;
            }
            await ReloadRGNCoinOffersAsync();
            await ReloadCustomCoinOffersAsync();
        }

        internal void SetUIInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
            _fullScreenLoadingIndicator.SetEnabled(!interactable);
        }

        private Task ReloadRGNCoinOffersAsync()
        {
            DisposeRGNCoinOffers();
            return LoadRGNCoinOffersAsync();
        }
        private async Task LoadRGNCoinOffersAsync()
        {
            SetUIInteractable(false);
            var offers = await CurrencyModule.I.GetRGNCoinEconomyAsync();
            for (int i = 0; i < offers.products.Count; i++)
            {
                var product = offers.products[i];
                RGNCoinItem item = Instantiate(_rgnCoinItemPrefab, _rgnCoinItemsContent);
                item.Init(this, i, product);
                _rgnCoinItems.Add(item);
            }
            _rgnCoinItemsContent.sizeDelta = 
                new Vector2(
                    _rgnCoinItems.Count * (_rgnCoinItemPrefab.GetWidth() + RGNCoinItem.GAB_BETWEEN_ITEMS),
                    0);
            SetUIInteractable(true);
        }

        private void DisposeRGNCoinOffers()
        {
            for (int i = 0; i < _rgnCoinItems.Count; i++)
            {
                _rgnCoinItems[i].Dispose();
            }
            _rgnCoinItems.Clear();
        }
        private Task ReloadCustomCoinOffersAsync()
        {
            DisposeCustomCoinOffers();
            return LoadCustomCoinOffersAsync();
        }
        private async Task LoadCustomCoinOffersAsync()
        {
            SetUIInteractable(false);
            var offers = await CurrencyModule.I.GetInAppPurchaseCurrencyDataAsync();
            for (int i = 0; i < offers.products.Count; i++)
            {
                var product = offers.products[i];
                CustomCoinItem item = Instantiate(_customCoinItemPrefab, _customCoinItemsContent);
                item.Init(this, i, product);
                _customCoinItems.Add(item);
            }
            _customCoinItemsContent.sizeDelta =
                new Vector2(
                    _customCoinItems.Count * (_rgnCoinItemPrefab.GetWidth() + CustomCoinItem.GAB_BETWEEN_ITEMS),
                    0);
            SetUIInteractable(true);
        }

        private void DisposeCustomCoinOffers()
        {
            for (int i = 0; i < _customCoinItems.Count; i++)
            {
                _customCoinItems[i].Dispose();
            }
            _customCoinItems.Clear();
        }
    }
}
