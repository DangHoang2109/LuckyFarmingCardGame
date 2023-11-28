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

    public Button _btnDeck;

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
        //Debug.LogError("NOT FOUND CARD " + cardID);
        return 1; //nếu chưa có thu về bao giờ thì card ở default level là 1
    }
    public override void PullCardToBag(List<InGame_CardDataModel> cardReceive)
    {
        base.PullCardToBag(cardReceive);
        if (cardReceive != null && cardReceive.Count > 0)
        {
            List < InGame_CardDataModelLevels > levelUp = this.MainDataModel.AddCardsToPallet(cardReceive);
            if(levelUp!=null && levelUp.Count > 0)
            {
                ListingUpgradedCardsDialog d = ListingUpgradedCardsDialog.ShowDialog();
                d.ParseData(levelUp, null);
            }
        }
        _tmpCoinValue.SetText($"{(PlayerModel.CurrentCoinPoint).ToString("D2")}");
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
            dmg = BaseDamagePerTurn; //replace with this host info

        InGameBasePlayerItem frontEnemy = InGameManager.Instance.FrontEnemy;
        if(frontEnemy != null)
        {
            //create attack vfx
            VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.AttackSword, amount: 1, desPos: frontEnemy.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

            void OnCallbackProjectileHit(VFXBaseObject _)
            {
                InGameManager.Instance.OnPlayerAttacking(frontEnemy.SeatID, dmg);
                base.AttackSingleUnit(dmg);
            }
        }
    }
    public override void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = BaseDamagePerTurn; //replace with this host info

        if (InGameManager.Instance.IsHaveEnemy)
        {
            List<InGameBasePlayerItem> EnemysAlive = InGameManager.Instance.EnemysAlive;
            List<Transform> enemyPos = new List<Transform>();
            foreach (var item in EnemysAlive)
            {
                enemyPos.Add(item.transform);
            }
            //create attack vfx
            VFXActionManager.Instance.ShowMultiVFxXByCard(vfxId: VFXGameID.AttackSword, amount: EnemysAlive.Count, desPoss: enemyPos, delay: 0.25f, cbOnFirst: OnCallbackProjectileHit);

            void OnCallbackProjectileHit(VFXBaseObject _)
            {
                InGameManager.Instance.OnPlayerAttackingAllUnit(isEnemySide: true, dmg);
                base.AttackAllUnit(dmg);
            }
        }
    }
    public override void AddShield(int shieldUnit = -1)
    {
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.DefendShield, amount: 1, desPos: ShieldUI.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.AddShield(shieldUnit);
        }
    }
    public override void AddHP(int heal)
    {
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.HealFlower, amount: 1, desPos: _hpBar.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.AddHP(heal);
        }
    }
    public override void RevealCard(int reveal = -1)
    {
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.CardFly, amount: 1, desPos: _btnDeck.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.RevealCard(reveal);
        }
    }
    public override void ForceDrawCard(int draw = -1)
    {        
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.CardFly, amount: 1, desPos: _btnDeck.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.ForceDrawCard(draw);
        }
    }
    public override void DrainHP(int stat = -1)
    {
        if (stat <= 0)
            stat = BaseDamagePerTurn; //replace with this host info

        InGameBasePlayerItem frontEnemy = InGameManager.Instance.FrontEnemy;
        if (frontEnemy != null)
        {
            //create attack vfx , startPos: frontEnemy.transform
            VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.orbHPRed, amount: 1, desPos: _hpBar.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

            void OnCallbackProjectileHit(VFXBaseObject _)
            {
                InGameManager.Instance.OnPlayerAttacking(frontEnemy.SeatID, stat);
                InGameManager.Instance.OnPlayerHeal(this.SeatID, stat);
            }
        }
    }

    public void AttackAllUnitAndStunFront(int dmg, int turnStun)
    {
        if (dmg <= 0)
            dmg = BaseDamagePerTurn;
        if (turnStun <= 0)
            turnStun = 1;

        if (InGameManager.Instance.IsHaveEnemy)
        {
            List<InGameBasePlayerItem> EnemysAlive = InGameManager.Instance.EnemysAlive;
            List<Transform> enemyPos = new List<Transform>();
            foreach (var item in EnemysAlive)
            {
                enemyPos.Add(item.transform);
            }
            //create attack vfx
            VFXActionManager.Instance.ShowMultiVFxXByCard(vfxId: VFXGameID.AttackSword, amount: EnemysAlive.Count, desPoss: enemyPos, delay: 0.25f, cbOnFirst: OnCallbackProjectileHit);

            void OnCallbackProjectileHit(VFXBaseObject _)
            {
                InGameManager.Instance.OnPlayerAttackingAllUnit(isEnemySide: true, dmg);
                base.AttackAllUnit(dmg);

                //set stun
                InGameBasePlayerItem frontEnemy = InGameManager.Instance.FrontEnemy;
                if (frontEnemy != null)
                    InGameManager.Instance.SetStun(frontEnemy, turnStun);

                InGameManager.Instance.OnTellControllerContinueTurn();
            }
        }
    }
    public override void SetMultiplierDamage(float damageMultiplier)
    {
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.BuffGreen, amount: 1, desPos: _playerAttributePallet.Panel, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.SetMultiplierDamage(damageMultiplier);
            InGameManager.Instance.OnTellControllerContinueTurn();
        }
    }
    public override void SetVulnerable(int amountTurn)
    {        
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.BuffGreen, amount: 1, desPos: _playerAttributePallet.Panel, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.SetVulnerable(amountTurn);
            InGameManager.Instance.OnTellControllerContinueTurn();
        }
    }
    #endregion Turn Action

    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }
}
