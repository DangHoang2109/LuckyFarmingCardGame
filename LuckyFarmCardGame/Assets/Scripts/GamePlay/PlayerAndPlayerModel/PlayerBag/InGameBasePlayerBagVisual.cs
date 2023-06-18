using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBasePlayerBagVisual : MonoBehaviour
{
    #region Prop on Editor
    [SerializeField] protected List<InGameBagCardTypeUIItem> _uiItems;

    #endregion Prop on editor

    #region Data
    protected InGameBasePlayerItem _hostPlayer;

    [SerializeField] protected Dictionary<int,InGameBagCardTypeUIItem> _dicItems;

    protected System.Action<int,bool> _onItemChosedWhileEffect;
    #endregion Data


    public InGameBasePlayerBagVisual SetHostPlayer(InGameBasePlayerItem p)
    {
        this._hostPlayer = p;
        return this;
    }
    /// <summary>
    /// Parse goal card
    /// </summary>
    public void ParseGoalCard(InGameMissionGoalCardConfig goalCardConfig)
    {
        _dicItems ??= new Dictionary<int, InGameBagCardTypeUIItem>();
        for (int i = 0; i < _uiItems.Count; i++)
        {
            _dicItems.Add(i, _uiItems[i]);

            _uiItems[i]
                .SetHostPlayer(this._hostPlayer)
                .SetCardType(i)
                .EnableToggleForEffectStage(false);
        }

        foreach (InGame_CardDataModelWithAmount inGame_CardDataModelWithAmount in goalCardConfig._requirement)
        {
            _dicItems[inGame_CardDataModelWithAmount._cardID]
                .SetMaxValue(inGame_CardDataModelWithAmount._amountCard);
        }

        foreach (InGameBagCardTypeUIItem item in _uiItems)
        {
            item.UpdateValue(0);
        }
    }
    public InGameBasePlayerBagVisual RefreshPlayerBag(Dictionary<int, InGame_CardDataModelWithAmount> playerBag)
    {
        foreach (KeyValuePair< int,InGameBagCardTypeUIItem> item in _dicItems)
        {
            int id = item.Value.CardID;
            int amount = playerBag.TryGetValue(id, out InGame_CardDataModelWithAmount c) ? c._amountCard : 0 ;
            _dicItems[id].UpdateValue(amount);
        }
        return this;
    }

    public InGameBasePlayerBagVisual EnableToggleForEffectStage(bool isOn, System.Action<int, bool> onItemChosedWhileEffect)
    {
        this._onItemChosedWhileEffect = onItemChosedWhileEffect;
        for (int i = 0; i < _uiItems.Count; i++)
        {
            _uiItems[i]
                .EnableToggleForEffectStage(isOn);
        }
        return this;
    }

    public bool TryFindUIItem(int cardID, out InGameBagCardTypeUIItem item)
    {
        return this._dicItems.TryGetValue(cardID, out item);
    }
    /// <summary>
    /// Called by toggle event 
    /// </summary>
    /// <param name="isOn"></param>
    public void ToggleEvent_OnChangeChosingValue(int cardItemID,bool isOn)
    {
        _onItemChosedWhileEffect?.Invoke(cardItemID,isOn);
    }
}
