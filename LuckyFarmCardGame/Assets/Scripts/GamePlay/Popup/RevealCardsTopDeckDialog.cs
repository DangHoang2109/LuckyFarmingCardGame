using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealCardsTopDeckDialog : BaseDialog
{
    public Transform _tfPanel;
    public CardRevealUIItem _prefab;
    public List<CardRevealUIItem> _items;
    private System.Action cbClose;
    public void ParseData(List<InGame_CardDataModel> topCards, System.Action cbClose)
    {
        this.cbClose = cbClose;
        if (topCards != null && topCards.Count > 0)
        {
            _items ??= new List<CardRevealUIItem>();
            foreach (var item in _items)
            {
                item.gameObject.SetActive(false);
            }

            if(topCards.Count > _items.Count)
            {
                int exceed = topCards.Count - _items.Count;
                for (int i = 0; i < exceed; i++)
                {
                    CardRevealUIItem item = Instantiate(_prefab, this._tfPanel);
                    _items.Add(item);
                }
            }

            for (int i = 0; i < topCards.Count; i++)
            {
                CardRevealUIItem item = this._items[i];
                item.gameObject.SetActive(true);
                item.ParseData(topCards[i]._id);
            }
        }
    }

    protected override void OnCompleteHide()
    {
        base.OnCompleteHide();
        cbClose?.Invoke();
    }
    public static RevealCardsTopDeckDialog ShowDialog()
    {
        return GameManager.Instance.OnShowDialog<RevealCardsTopDeckDialog>("Dialogs/RevealTopCardDialog");
    }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Cosinas/Game/RevealCardsTopDeckDialog")]
    public static void Test()
    {
        List<InGame_CardDataModel> topCards = InGameManager.Instance.GameController.GetDeckTopCards(2, isWillPopThatCardOut: false);
        RevealCardsTopDeckDialog d = RevealCardsTopDeckDialog.ShowDialog();
        d.ParseData(topCards, cbClose: null);
    }

#endif

}
