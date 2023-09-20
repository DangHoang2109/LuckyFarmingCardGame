using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMainPlayerItem : InGameBasePlayerItem
{
    #region Prop on editor
    public Button _btnEndTurn;
    #endregion Prop on editor

    #region Data
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
    #endregion Turn Action

    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }
}
