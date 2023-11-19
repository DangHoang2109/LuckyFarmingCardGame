using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Instruction_CardEffectQuickthroughDialog : BaseDialog
{
    #region Prop on Editor
    public List<Instruction_CardEffectQuickthroughItem> _items;

    public Instruction_CardEffectQuickthroughItem _itemPrefab;
    public RectTransform _tfPanel;
    #endregion  Prop on Editor


    public override void OnShow(object data = null, UnityAction callback = null, bool isSkipAnimationShow = false)
    {
        base.OnShow(data, callback, isSkipAnimationShow);
        base.OnShow(data, callback);
        ParseData();
    }

    public void ParseData()
    {
        List<InGameCardConfig> allCards = InGameCardConfigs.Instance._configs;
        if(allCards != null && allCards.Count > 0)
        {
            int amountToSpawn = allCards.Count - _items.Count;
            if(amountToSpawn > 0)
            {
                for (int i = 0; i < amountToSpawn; i++)
                {
                    _items.Add(Instantiate(this._itemPrefab, this._tfPanel));
                }
            }

            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < allCards.Count; i++)
            {
                _items[i].gameObject.SetActive(true);
                _items[i].ParseCard(allCards[i]);
            }
        }
    }
}
