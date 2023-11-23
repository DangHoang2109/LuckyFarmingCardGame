using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RevealCardsTopDeckDialog : BaseDialog
{
    public RectTransform _tfPanel;
    public CardRevealUIItem _prefab;
    public List<CardRevealUIItem> _items;
    private System.Action cbClose;

    [Header("UI Grid")]
    public GridLayoutGroup _gridLayoutGroup;

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
                item.SetIndex(i + 1);
            }
        }

        StartCoroutine(SetUIGridView(topCards.Count));
    }
    private IEnumerator SetUIGridView(int itemCount)
    {
        yield return new WaitForEndOfFrame();
        //set scale of item
        if(itemCount <= 4)
        {
            _gridLayoutGroup.cellSize = new Vector2(288f, 380f);
        }
        else
            _gridLayoutGroup.cellSize = new Vector2(220f, 290f);

        //set pivot of transform
        if(itemCount <= 2)
        {
            _tfPanel.pivot = new Vector2(0f, 0.5f);
            _tfPanel.anchorMin = new Vector2(0, 0.5f);
            _tfPanel.anchorMax = new Vector2(1, 0.5f);
        }
        else
        {
            _tfPanel.pivot = new Vector2(0f, 1);
            _tfPanel.anchorMin = new Vector2(0, 1f);
            _tfPanel.anchorMax = new Vector2(1, 1f);
        }
        //_tfPanel.localPosition = Vector3.zero;
        _tfPanel.anchoredPosition = Vector3.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tfPanel);
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
    [UnityEditor.MenuItem("Cosinas/Game/Reveal8CardsTopDeckDialog")]
    public static void Test8()
    {
        List<InGame_CardDataModel> topCards = InGameManager.Instance.GameController.GetDeckTopCards(8, isWillPopThatCardOut: false);
        RevealCardsTopDeckDialog d = RevealCardsTopDeckDialog.ShowDialog();
        d.ParseData(topCards, cbClose: null);
    }
#endif

}
