using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMainPlayerItem : InGameBasePlayerItem
{
    private BaseInGameMainPlayerDataModel _mainDataModel;
    public BaseInGameMainPlayerDataModel MainDataModel
    {
        get
        {
            if(_mainDataModel == null)
                _mainDataModel = this.PlayerModel != null ? (this.PlayerModel as BaseInGameMainPlayerDataModel) : null;
            return _mainDataModel;
        }
    }

    #region Prop on editor
    public Button _btnEndTurn;
    #endregion Prop on editor

    #region Data
    public int AmountCardInBag => this.MainDataModel?.AmountCardInBag ?? 0;
    public bool IsHasCardIsBag => this.MainDataModel?.IsHasCardIsBag ?? false;
    #endregion Data


    #region Init Action
    public override InGameBasePlayerItem InitPlayerItSelf()
    {
        return base.InitPlayerItSelf();
    }

    #endregion InitAction

    #region Turn Action
    public override void BeginTurn()
    {
        base.BeginTurn();
        _btnEndTurn.interactable = true;
    }
    public override void ContinueTurn()
    {
        base.ContinueTurn();
        _btnEndTurn.interactable = true;
    }
    public override void OnACardGoingBeDrawed()
    {
        base.OnACardGoingBeDrawed();
        _btnEndTurn.interactable = false;
    }
    public override void Action_ACardPutToPallet(int cardID)
    {
        base.Action_ACardPutToPallet(cardID);

    }
    public override void Action_DecideAndUseCoin(int amountCoinNeeding, int pointAdding)
    {
        base.Action_DecideAndUseCoin(amountCoinNeeding, pointAdding);

        InGameManager.Instance.ShowConfirmUsingCoin(amountCoinNeeding, pointAdding);
    }
    public override void EndTurn()
    {
        base.EndTurn();
        _btnEndTurn.interactable = false;
    }
    public override void PullCardToBag(List<InGame_CardDataModel> cardReceive)
    {
        base.PullCardToBag(cardReceive);
        if (cardReceive != null && cardReceive.Count > 0)
        {
            this.MainDataModel.AddCardsToPallet(cardReceive);
        }
        this.BagVisual?.RefreshPlayerBag(this.MainDataModel._dictionaryBags);

        _tmpCoinValue.SetText($"{(PlayerModel.CurrentCoinPoint).ToString("D2")}");

        Debug.Log($"PLAYER {this.ID} bag: {this.MainDataModel._dictionaryBags.DebugDicCardInGame()}");
    }
    #endregion Turn Action

    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }
}
