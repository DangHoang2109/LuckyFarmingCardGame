using System;
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
    public InGameDeckConfig DeckConfig => this.MainDataModel?.DeckConfig;
    #region Prop on editor
    public Button _btnEndTurn;

    //[SerializeField]
    //protected InGameBasePlayerBagVisual _bagVisual;
    //public InGameBasePlayerBagVisual BagVisual => _bagVisual;

    public Image _imgTimer;
    public Image _imgAvatar;


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
    public override InGameBasePlayerItem SetAPlayerModel(BaseInGamePlayerDataModel model)
    {
        base.SetAPlayerModel(model);
        InitPlayerItSelf();

        this._imgAvatar.sprite = this.MainDataModel?._statConfig?._icon;

        return this;
    }
    #endregion InitAction

    #region Turn Action
    public override void BeginTurn()
    {
        base.BeginTurn();
        _imgTimer.gameObject.SetActive(true);
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
        _imgTimer.gameObject.SetActive(false);
        _btnEndTurn.interactable = false;
    }
    public int GetCardLevel(int cardID)
    {
        if(this.MainDataModel.TryGetCardInBag(cardID, out InGame_CardDataModelLevels card))
        {
            return card._currentLevel;
        }
        Debug.LogError("NOT FOUND CARD " + cardID);
        return 0;
    }
    public override void PullCardToBag(List<InGame_CardDataModel> cardReceive)
    {
        base.PullCardToBag(cardReceive);
        if (cardReceive != null && cardReceive.Count > 0)
        {
            this.MainDataModel.AddCardsToPallet(cardReceive);
        }
        //this.BagVisual?.RefreshPlayerBag(this.MainDataModel._dictionaryBags);

        _tmpCoinValue.SetText($"{(PlayerModel.CurrentCoinPoint).ToString("D2")}");

        Debug.Log($"PLAYER {this.ID} bag: {this.MainDataModel._dictionaryBags.DebugDicCardInGame()}");
    }
    public override void ParseVisualBagUI()
    {
        base.ParseVisualBagUI();
        ///this.BagVisual?.SetHostPlayer(this);
    }
    public override void ReadyInDestroyCardEffectStage(int amountCardToBeChoseInEffect, Action<int, List<int>> onCompleteBeingChose)
    {
        base.ReadyInDestroyCardEffectStage(amountCardToBeChoseInEffect, onCompleteBeingChose);
        //this.BagVisual?.EnableToggleForEffectStage(true, OnACardItemInBagBeingChose_DestroyingPhase);
    }
    protected override void OnACardItemInBagBeingChose_DestroyingPhase(int cardID, bool isChosed)
    {
        base.OnACardItemInBagBeingChose_DestroyingPhase(cardID, isChosed);
        //if (CardBeingChose.Count == _amountCardToBeChoseInEffect)
        //{
        //    this.BagVisual?.EnableToggleForEffectStage(false, null);
        //}
    }
    public override void ReadyInPullingCardEffectStage(int amountCardToBeChoseInEffect, Action<int, List<int>> onCompleteBeingChose)
    {
        base.ReadyInPullingCardEffectStage(amountCardToBeChoseInEffect, onCompleteBeingChose);
        //this.BagVisual?.EnableToggleForEffectStage(true, OnACardItemInBagBeingChose_PullingCardEffect);
    }
    protected override void OnACardItemInBagBeingChose_PullingCardEffect(int cardID, bool isChosed)
    {
        base.OnACardItemInBagBeingChose_PullingCardEffect(cardID, isChosed);
        //if (CardBeingChose.Count == _amountCardToBeChoseInEffect)
        //{
        //    this.BagVisual?.EnableToggleForEffectStage(false, null);
        //}
    }
    public override void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info
        InGameManager.Instance.OnPlayerAttacking(InGameManager.Instance.FrontEnemy.ID, dmg);
        base.AttackSingleUnit(dmg);
    }
    public override void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info
        InGameManager.Instance.OnPlayerAttackingAllUnit(isEnemySide: true, dmg);
        base.AttackAllUnit(dmg);
    }
    #endregion Turn Action

    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }
}
