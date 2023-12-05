using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListingUpgradedCardsDialog : BaseDialog
{
    public Transform _tfPanel;
    public CardUpgradedInfoUIItem _prefab;
    public List<CardUpgradedInfoUIItem> _items;
    private System.Action cbClose;

    public void ParseData(List<InGame_CardDataModelLevels> levelupCards, System.Action cbClose)
    {
        this.cbClose = cbClose;
        if (levelupCards != null && levelupCards.Count > 0)
        {
            _items ??= new List<CardUpgradedInfoUIItem>();
            foreach (var item in _items)
            {
                item.gameObject.SetActive(false);
            }

            if (levelupCards.Count > _items.Count)
            {
                int exceed = levelupCards.Count - _items.Count;
                for (int i = 0; i < exceed; i++)
                {
                    CardUpgradedInfoUIItem item = Instantiate(_prefab, this._tfPanel);
                    _items.Add(item);
                }
            }

            for (int i = 0; i < levelupCards.Count; i++)
            {
                CardUpgradedInfoUIItem item = this._items[i];
                item.gameObject.SetActive(true);
                item.ParseData(levelupCards[i]);
            }
        }

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_tfPanel as RectTransform);
    }

    protected override void OnCompleteHide()
    {
        base.OnCompleteHide();
        cbClose?.Invoke();
    }
    public static ListingUpgradedCardsDialog ShowDialog()
    {
        return GameManager.Instance.OnShowDialog<ListingUpgradedCardsDialog>("Dialogs/ListingUpgradedCardsDialog");
    }

}
