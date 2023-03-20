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

        [SerializeField] private RGNCoinItem _rgnCoinItemPrefab;

        private bool _triedToLoad;
        private List<RGNCoinItem> _rgnCoinItems;

        public override void PreInit(IRGNFrame rgnFrame)
        {
            base.PreInit(rgnFrame);
            _rgnCoinItems = new List<RGNCoinItem>();
        }
        protected override void OnShow()
        {
            if (_triedToLoad)
            {
                return;
            }
            ReloadRGNCoinOffersAsync();
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
            _rgnCoinItemsContent.sizeDelta = new Vector2(_rgnCoinItems.Count * (_rgnCoinItemPrefab.GetWidth() + RGNCoinItem.GAB_BETWEEN_ITEMS), 0);
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
    }
}
