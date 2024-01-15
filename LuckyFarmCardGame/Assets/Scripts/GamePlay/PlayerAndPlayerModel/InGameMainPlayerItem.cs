using DG.Tweening;
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
    public Button _btnDeck;
    public Button _btnEndTurn;
    public Transform _tfInteractingInTurn; //transform contain button deck and enturn for animation sliding
    //[SerializeField]
    //protected InGameBasePlayerBagVisual _bagVisual;
    //public InGameBasePlayerBagVisual BagVisual => _bagVisual;

    public Image _imgTimer;
    public Image _imgAvatar;

    public GameObject _gVFXAttacked, _gVFXShieldDefAttack;

    #endregion Prop on editor

    #region Data
    public int AmountCardInBag => this.MainDataModel?.AmountCardInBag ?? 0;
    public bool IsHasCardIsBag => this.MainDataModel?.IsHasCardIsBag ?? false;

    /// <summary>
    /// Use this to quarantee dont be multiple click endturn
    /// </summary>
    private bool _isInTurn;
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
        _isInTurn = true;
        bool isStunning = this.IsStunning;
        base.BeginTurn();
        if (!isStunning)
        {
            _imgTimer.gameObject.SetActive(true);
            _btnEndTurn.interactable = true;

            _tfInteractingInTurn.DOLocalMoveY(0, 0.75f);
        }
        else
        {
            InGameManager.Instance.OnUserEndTurn();
        }
    }
    public override void ContinueTurn()
    {
        base.ContinueTurn();
        _btnEndTurn.interactable = true;
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
    public void OnClickEndTurn()
    {
        if (this._isInTurn)
        {
            _isInTurn = false;
            InGameManager.Instance.OnMainUserEndTurn();
        }
    }

    public override void EndTurn()
    {
        base.EndTurn();
        _imgTimer.gameObject.SetActive(false);
        _btnEndTurn.interactable = false;
        _tfInteractingInTurn.DOLocalMoveY(-200f, 0.75f);
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
            //heal 1HP each card, use this for not showing the animation flower
            this.CurrentHP += cardReceive.Count;
        }
        _tmpCoinValue.SetText($"{(PlayerModel.CurrentCoinPoint).ToString("D2")}");
    }
    public override int Attacked(int dmg, Action<InGameBasePlayerItem> deaded = null)
    {
        bool isShieldUsed = this.CurrentShield > 0;
        dmg = base.Attacked(dmg, deaded);
        //if(dmg > 0)
        StartCoroutine(ieVFXAttacked(isShieldUsed,deaded));
        
        return dmg;
    }
    IEnumerator ieVFXAttacked(bool shieldDef,Action<InGameBasePlayerItem> deaded = null)
    {
        if (shieldDef)
        {
            _gVFXShieldDefAttack.SetActive(false);

            yield return new WaitForEndOfFrame();
            _gVFXShieldDefAttack.SetActive(true);
        }
        else
        {
            _gVFXAttacked.SetActive(false);

            yield return new WaitForEndOfFrame();
            _gVFXAttacked.SetActive(true);
        }


        if (this.isDead())
        {
            yield return new WaitForSeconds(0.2f);
            deaded?.Invoke(this);
        }

    }
    public override void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = BaseDamagePerTurn; //replace with this host info

        float mulDmg = MultiplierDamage;
        if (mulDmg > 0)
            dmg = (int)(dmg * mulDmg);

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
        else
        {
            InGameManager.Instance.OnTellControllerContinueTurn();
        }
    }
    public override void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = BaseDamagePerTurn; //replace with this host info
        float mulDmg = MultiplierDamage;
        if (mulDmg > 0)
            dmg = (int)(dmg * mulDmg);
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
        else
        {
            InGameManager.Instance.OnTellControllerContinueTurn();
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
            //create attack vfx 
            VFXActionManager.Instance.ShowFromToVFxXByCard(vfxId: VFXGameID.orbHPRed, amount: 1, startPos: frontEnemy.transform, desPos: _hpBar.transform, delay: 0.25f, cb: OnCallbackProjectileHit);

            void OnCallbackProjectileHit(VFXBaseObject _)
            {
                InGameManager.Instance.OnPlayerAttacking(frontEnemy.SeatID, stat);
                this.AddHP(stat);

                InGameManager.Instance.OnTellControllerContinueTurn();
            }
        }
        else
        {
            InGameManager.Instance.OnTellControllerContinueTurn();
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
        else
        {
            InGameManager.Instance.OnTellControllerContinueTurn();
        }
    }
    public override void SetMultiplierDamage(float damageMultiplier, int turnActive)
    {
        //create attack vfx
        VFXActionManager.Instance.ShowVFxXBycard(vfxId: VFXGameID.BuffGreen, amount: 1, desPos: _playerAttributePallet.Panel, delay: 0.25f, cb: OnCallbackProjectileHit);

        void OnCallbackProjectileHit(VFXBaseObject _)
        {
            base.SetMultiplierDamage(damageMultiplier, turnActive);
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
    public override void Dead(Action cb)
    {
        base.Dead(cb);
    }
    public override void ClearWhenDead()
    {
        base.ClearWhenDead();

        this._btnDeck.gameObject.SetActive(false);
        this._btnEndTurn.gameObject.SetActive(false);
    }
    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }
    public void OnClickShowQuickTutorial()
    {
        GameManager.Instance.OnShowDialog<BaseDialog>("Dialogs/RuleSummaryDialog");

        //GameManager.Instance.OnShowDialog<Instruction_CardEffectQuickthroughDialog>("Dialogs/Instruction_CardEffectQuickthroughDialog");
    }
}
