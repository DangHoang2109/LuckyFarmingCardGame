using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Spine;
using UnityEditor.Experimental.GraphView;
using System;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

public class InGameBasePlayerItem : MonoBehaviour
{
    #region Prop on editor
    public TMPro.TextMeshProUGUI _tmpCoinValue;

    public HPBarUI _hpBar;

    public PlayerAttributeUI _playerAttributePallet;
    public Transform ShieldUI
    {
        get
        {
            if (_playerAttributePallet.TryGetItem(AttributeID.SHIELD, out var item))
                return item.transform;
            else
                return _playerAttributePallet.Panel;
        }
    }

    #endregion Prop on editor

    #region Data
    public virtual int CurrentHP
    {
        get
        {
            return this.PlayerModel?.CurrentHP ?? 0;
        }
        set
        {
            if (this.PlayerModel != null)
            {
                this.PlayerModel.CurrentHP = value;
                this._hpBar.UpdateValue(this.PlayerModel.CurrentHP);
            }
        }
    }
    public virtual int MaxHP
    {
        get
        {
            return this.PlayerModel?.MaxHP ?? 0;
        }
        set
        {
            if (this.PlayerModel != null)
            {
                this.PlayerModel.MaxHP = value;
                this._hpBar.SetMaxValue(this.PlayerModel.MaxHP, isSetCurrentToMax: false);
                this._hpBar.UpdateValue(this.PlayerModel.CurrentHP);
            }
        }
    }
    public int BaseDamagePerTurn => this.PlayerModel?.DamagePerTurn ?? 0;
    public int BaseShieldPerAdd => this.PlayerModel?.ShieldPerAdd ?? 0;
    public int BaseHPPerHeal => this.PlayerModel?.HPPerHeal ?? 0;

    public bool IsPlaying => this._playerModel != null;
    protected BaseInGamePlayerDataModel _playerModel;
    public BaseInGamePlayerDataModel PlayerModel => _playerModel;

    public int SeatID => this.PlayerModel?._seatId ?? -1;
    public bool IsMainPlayer => SeatID == 0;

    //in effect card
    protected List<int> CardBeingChose; protected int _amountCardToBeChoseInEffect; protected System.Action<int, List<int>> _onCompleteBeingChoseInEffect;


    #region InGame skill involve Data : Curse, Stun, Burn, Poision....
    public int AmountTurnStunned
    {
        get
        {
            if (AttributeDatas == null)
                return 0;
            if (AttributeDatas.TryGetData(AttributeID.STUN, out var item))
                return item.GetTurnLeft();
            else
                return 0;
        }
    }
    /// <summary>
    /// 1f is 100% -> normal state
    /// </summary>
    public float MultiplierDamage
    {
        get
        {
            if (AttributeDatas == null)
                return 1f;
            if (AttributeDatas.TryGetData(AttributeID.INCREASE_DMG, out var item))
                return item.GetValue() / 100f;
            else
                return 1f;
        }
    }
    /// <summary>
    /// Số turn miễn đụng
    /// </summary>
    public int AmountTurnInvulnerable
    {
        get
        {
            if (AttributeDatas == null)
                return 0;
            if (AttributeDatas.TryGetData(AttributeID.INVULNERABLE, out var item))
                return item.GetTurnLeft();
            else
                return 0;
        }
    }

    public bool IsStunning
    {
        get
        {
            if (AttributeDatas == null)
                return false;
            if (AttributeDatas.TryGetData(AttributeID.STUN, out var item))
                return item.GetTurnLeft() > 0;
            else
                return false;
        }
    }

    public InGamePlayerAttributeDatas AttributeDatas => this.PlayerModel?.AttributeDatas;

    #endregion InGame skill involve Data

    #endregion Data

    #region Init Action
    /// <summary>
    /// Calling by InGameManager when set up init game
    /// </summary>
    /// <param name="model"></param>
    public virtual InGameBasePlayerItem SetAPlayerModel(BaseInGamePlayerDataModel model)
    {
        this._playerModel = model;
        _tmpCoinValue?.SetText($"{(PlayerModel.CurrentCoinPoint).ToString("D2")}");
        this._hpBar.SetMaxValue(PlayerModel.MaxHP);
        _playerAttributePallet?.JoinGame(this);
        return this;
    }
    public virtual InGameBasePlayerItem InitPlayerItSelf()
    {
        return this;
    }
    public virtual void ParseVisualBagUI()
    {
    }
    #endregion Init Action

    #region Turn Action
    public virtual void BeginTurn()
    {
        AttributeDatas?.StartTurn();
    }
    public virtual void EndTurn()
    {
    }
    public virtual void ContinueTurn()
    {

    }
    /// <summary>
    /// called by ingame controller when a card is drawing
    /// </summary>
    public virtual void OnACardGoingBeDrawed()
    {

    }
    /// <summary>
    /// Called by IngameManager when this user endturn or force endturn success by rolling dice
    /// </summary>
    public virtual void PullCardToBag(List<InGame_CardDataModel> cardReceive)
    {
    }

    /// <summary>
    /// Called by InGame Manager
    /// To let this user ready to be chose by the user who has destroying card
    /// </summary>
    public virtual void ReadyInDestroyCardEffectStage(int amountCardToBeChoseInEffect, System.Action<int, List<int>> onCompleteBeingChose )
    {
        this.CardBeingChose ??= new List<int>();
        CardBeingChose.Clear();

        this._amountCardToBeChoseInEffect = amountCardToBeChoseInEffect;
        _onCompleteBeingChoseInEffect = onCompleteBeingChose;
    }

    protected virtual void OnACardItemInBagBeingChose_DestroyingPhase(int cardID, bool isChosed)
    {
        CardBeingChose ??= new List<int>();
        if (isChosed)
            CardBeingChose.Add(cardID);
        else
            CardBeingChose.Remove(cardID);

        if (CardBeingChose.Count == _amountCardToBeChoseInEffect)
        {
            _onCompleteBeingChoseInEffect?.Invoke(this.SeatID, new List<int>(CardBeingChose));

            this.CardBeingChose.Clear();
            this._amountCardToBeChoseInEffect = 0;
            this._onCompleteBeingChoseInEffect = null;
        }
    }

    /// <summary>
    /// Called by InGame Manager
    /// To let this user ready to be chose by the user who has destroying card
    /// </summary>
    public virtual void ReadyInPullingCardEffectStage(int amountCardToBeChoseInEffect, System.Action<int, List<int>> onCompleteBeingChose)
    {
        this.CardBeingChose ??= new List<int>();
        CardBeingChose.Clear();

        this._amountCardToBeChoseInEffect = amountCardToBeChoseInEffect;
        _onCompleteBeingChoseInEffect = onCompleteBeingChose;

    }
    protected virtual void OnACardItemInBagBeingChose_PullingCardEffect(int cardID, bool isChosed)
    {
        CardBeingChose ??= new List<int>();
        if (isChosed)
            CardBeingChose.Add(cardID);
        else
            CardBeingChose.Remove(cardID);

        if (CardBeingChose.Count == _amountCardToBeChoseInEffect)
        {
            _onCompleteBeingChoseInEffect?.Invoke(this.SeatID, new List<int>(CardBeingChose));

            this.CardBeingChose.Clear();
            this._amountCardToBeChoseInEffect = 0;
            this._onCompleteBeingChoseInEffect = null;
        }
    }

    public virtual void Action_ACardPutToPallet(int cardID)
    {

    }
    public virtual void Action_DecideAndUseCoin(int amountCoinNeeding, int pointAdding)
    {

    }
    public virtual void Attacked(int dmg, System.Action<InGameBasePlayerItem> deaded = null)
    {
        if(this.CurrentShield > 0)
        {
            int cacheShield = CurrentShield;
            this.CurrentShield -= dmg;
            dmg = Mathf.Clamp(dmg - cacheShield, 0 , int.MaxValue);
        }
        SubtractHP(dmg);
    }
    /// <summary>
    /// Only use for subtract directly damage
    /// If we subtract hp through attack, please use Attacked function as it will consider the shield
    /// </summary>
    /// <param name="dmg"></param>
    public virtual void SubtractHP(int dmg)
    {
        this.CurrentHP -= dmg;
        if (isDead())
        {
            InGameManager.Instance.OnAPlayerDie(this.SeatID);
        }
    }
    public virtual void AddHP(int heal)
    {
        this.CurrentHP += heal;
    }
    public virtual void RecoverFullHP()
    {
        this.CurrentHP = this.PlayerModel.MaxHP;
    }
    public virtual void IncreaseMaxHP(int add, bool isAddToCurrentToo)
    {
        this.PlayerModel.MaxHP += add;
        if (isAddToCurrentToo)
        {
            AddHP(add);
        }
    }
    public virtual void AddShield(int shieldUnit = -1)
    {
        if (shieldUnit <= 0)
            shieldUnit = 1; //replace with this host info
        Debug.Log("HOST: CREATE SHIELD" + shieldUnit);
        this.CurrentShield += shieldUnit;
    }
    public virtual void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info
        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info

        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void Heal(int heal = -1)
    {
        if (heal <= 0)
            heal = 1; //replace with this host info
        InGameManager.Instance.OnPlayerHeal(this.SeatID, heal);
        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void DefenseCreateShield(int shieldUnit = -1)
    {
        if (shieldUnit <= 0)
            shieldUnit = 1; //replace with this host info
        InGameManager.Instance.OnPlayerDefense(this.SeatID, shieldUnit);
        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void RevealCard(int reveal = -1)
    {
        if (reveal <= 0)
            reveal = 1; //replace with this host info
        InGameManager.Instance.OnTellControllerToRevealTopCard(reveal);
    }
    public virtual void ForceDrawCard(int draw = -1)
    {
        if (draw <= 0)
            draw = 1; //replace with this host info
        InGameManager.Instance.OnTellControllerToDrawCards(draw);
    }
    public virtual void DrainHP(int stat = -1)
    {
        if (stat <= 0)
            stat = 1; //replace with this host info
        AttackSingleUnit(stat);
        Heal(stat);
    }

    #endregion Turn Action

    public virtual bool isDead()
    {
        return this.PlayerModel?.IsDead() ?? false;
    }
    public virtual bool IsActive()
    {
        return IsPlaying && this.gameObject.activeInHierarchy;
    }
    public virtual void CustomUpdate()
    {
    }
    public virtual void Dead(System.Action cb)
    {
        Debug.Log("DEAD");
    }
    public virtual void ClearWhenDead()
    {
        this.gameObject.SetActive(false);
        this.AttributeDatas?.ClearAll();
        this._playerModel = null;
    }
    #region Skill & Card Affect
    public virtual void AssignCallbackToAttributeData(AttributeID id, System.Action<AttributeID, int, int> cb)
    {
        AttributeDatas.AssignCallback(id, cb);
    }
    public virtual void AddAttribute(AttributeID id, int value, int turnActive, bool isPercent = false)
    {
        int val = AttributeDatas.AddAttribute(id, value, turnActive: turnActive);
        this._playerAttributePallet.AddAttribute(id, val, turnActive, turnActive > 0, isPercent: isPercent);
    }
    public virtual void SetAttribute(AttributeID id, int value, int turnActive, bool isPercent = false)
    {
        AttributeDatas.SetAttribute(id, value, turnActive: turnActive);
        this._playerAttributePallet.AddAttribute(id, value, turnActive, turnActive > 0, isPercent: isPercent);
    }
    public virtual void SetStun(int amountTurnStunnning)
    {
        AddAttribute(AttributeID.STUN, 1, turnActive: amountTurnStunnning);
    }
    public virtual void SetMultiplierDamage(float damageMultiplier, int turnActive)
    {
        AddAttribute(AttributeID.INCREASE_DMG, (int)(damageMultiplier*100), turnActive, isPercent: true);
    }
    public virtual void SetVulnerable(int amountTurn)
    {
        AddAttribute(AttributeID.INVULNERABLE, value: 1, turnActive: amountTurn);
    }
    public virtual int CurrentShield
    {
        get
        {
            if (AttributeDatas.TryGetData(AttributeID.SHIELD, out var item))
                return item.GetValue();
            else
                return 0;
        }
        set
        {
            value = Mathf.Clamp(value, 0, int.MaxValue);
            if (this.PlayerModel != null)
            {
                SetAttribute(AttributeID.SHIELD, value, turnActive: 1);
            }
        }
    }
    #endregion 

}

public class BaseInGamePlayerDataModel
{
    public int _seatId;
    public bool _isMainPlayer;
    protected int _currentCoinPoint = 0;
    public int CurrentCoinPoint
    {
        get => _currentCoinPoint;
        set
        {
            _currentCoinPoint = value;

        }
    }

    public virtual int CurrentHP { get => currentHP; set => currentHP = Mathf.Clamp(value, 0, MaxHP); }
    public virtual int MaxHP { get => maxHP; set => maxHP = value; }
    //public int Shield { get => shield; set => shield = Mathf.Clamp(value, 0, int.MaxValue); }

    public virtual InGamePlayerAttributeDatas AttributeDatas { get; set; } 

    protected int currentHP;
    protected int maxHP;
    //protected int shield;
    protected int baseDamagePerTurn = 0, baseShieldPerAdd = 0, baseHPPerHeal = 0;
    public int DamagePerTurn => baseDamagePerTurn;
    public int ShieldPerAdd => baseShieldPerAdd;
    public int HPPerHeal => baseHPPerHeal;

    public BaseInGamePlayerDataModel()
    {
    }
    public BaseInGamePlayerDataModel SetSeatID(int id, bool isMain)
    {
        this._seatId = id;
        this._isMainPlayer = isMain;
        return this;
    }
    public BaseInGamePlayerDataModel SetHP(int maxHP)
    {
        this.CurrentHP = this.MaxHP = maxHP;
        return this;
    }
    public virtual BaseInGamePlayerDataModel SetBaseStat(int baseDamge, int baseShieldAdd, int baseHealAdd)
    {
        this.baseDamagePerTurn = baseDamge;
        this.baseShieldPerAdd = baseShieldAdd;
        this.baseHPPerHeal = baseHealAdd;
        return this;
    }

    public virtual BaseInGamePlayerDataModel StartGame()
    {
        AttributeDatas ??= new InGamePlayerAttributeDatas();
        AttributeDatas.JoinGame(this._seatId);
        return this;
    }
    public bool IsDead()
    {
        return CurrentHP <= 0;
    }

    public bool IsCanUseGameCoin(int amount)
    {
        return this.CurrentCoinPoint >= amount;
    }
    public bool SubtractGameCoinIfCan(int amount)
    {
        if (IsCanUseGameCoin(amount))
        {
            this.CurrentCoinPoint -= amount;
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class InGame_CardDataModelWithAmount
{
    [ValueDropdown("ListNextStepViews", ExpandAllMenuItems = true)]
    public int _cardID;
    public int _amountCard;

    public InGame_CardDataModelWithAmount()
    {
        _amountCard = 1;
    }
    public InGame_CardDataModelWithAmount(InGame_CardDataModelWithAmount c)
    {
        this._cardID = c._cardID;
        this._amountCard = c._amountCard;
    }
    public InGame_CardDataModelWithAmount(InGame_CardDataModel palletCard)
    {
        this._cardID = palletCard._id;
        this._amountCard = 1;
    }
    public InGame_CardDataModelWithAmount AddACard(InGame_CardDataModel palletCard, int amount = 1)
    {
        if (this._cardID == palletCard._id)
        {
            this._amountCard += amount;
        }
        return this;
    }
    public InGame_CardDataModelWithAmount SubtractCard(int amount = 1)
    {
        this._amountCard = Mathf.Clamp(_amountCard - amount, 0, _amountCard);
        return this;
    }
    public InGame_CardDataModelWithAmount PullSplitCard(out bool splitToTwoSuccess, out InGame_CardDataModelWithAmount splitedCard, int amountToSplit = 1)
    {
        splitedCard = new InGame_CardDataModelWithAmount(this) { _amountCard = amountToSplit };
        splitToTwoSuccess = true;
        if (this._amountCard <= amountToSplit)
        {
            splitToTwoSuccess = false;
            splitedCard._amountCard = this._amountCard;
            this._amountCard = 0;
        }
        else
        {
            splitToTwoSuccess = true;
            splitedCard._amountCard = amountToSplit;
            this._amountCard -= amountToSplit;
        }

        return this;
    }
    public bool CompareEnoughOrHigher(InGame_CardDataModelWithAmount other)
    {
        return this._cardID == other._cardID && this._amountCard >= other._amountCard;
    }
#if UNITY_EDITOR
    private IEnumerable ListNextStepViews()
    {
        var temp = Resources.Load<InGameCardConfigs>("Configs/Games/InGameCardConfigs")._configs
            .Select(x => new ValueDropdownItem($"{x._cardID} - {x._cardName}", x._cardID)).ToList();
        temp.Insert(0, new ValueDropdownItem("None", -1));
        return temp;
    }
#endif
}


public class BaseInGameMainPlayerDataModel : BaseInGamePlayerDataModel
{
    public InGamePlayerConfig _statConfig;
    public InGameDeckConfig DeckConfig => this._statConfig?.DeckConfig;
 
    public List<InGame_CardDataModelLevels> _bag;
    public Dictionary<int, InGame_CardDataModelLevels> _dictionaryBags;

    public int AmountCardInBag => 0;//(_bag == null || _bag.Count == 0) ? 0 : this._bag.Sum(x => x._amountCard);
    public bool IsHasCardIsBag => AmountCardInBag > 0;

    public BaseInGameMainPlayerDataModel() : base()
    {
        this._bag = new List<InGame_CardDataModelLevels>();
        this._dictionaryBags = new Dictionary<int, InGame_CardDataModelLevels>();
    }
    public BaseInGameMainPlayerDataModel SetStatConfig(InGamePlayerConfig config)
    {
        _statConfig = config;
        this.SetHP(config._maxHP);
        this.SetBaseStat(config._baseDamage, config._baseShield, config._baseHeal);
        return this;
    }
    public bool TryGetCardInBag(int id, out InGame_CardDataModelLevels card)
    {
        _dictionaryBags ??= new Dictionary<int, InGame_CardDataModelLevels>();
        return _dictionaryBags.TryGetValue(id, out card);
    }

    /// <summary>
    /// Add cards to player bags
    /// These usually from pallet
    /// </summary>
    /// <param name="cards"></param>
    /// return level up cards
    public List<InGame_CardDataModelLevels> AddCardsToPallet(List<InGame_CardDataModel> cards)
    {
        _bag ??= new List<InGame_CardDataModelLevels>();
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelLevels>();

        List<InGame_CardDataModelLevels> levelUpIfHas = new List<InGame_CardDataModelLevels>();

        if (cards != null && cards.Count > 0)
        {
            foreach (InGame_CardDataModel cardDataModel in cards)
            {
                int old = 0;
                int newValue = 1;
                int currentGoal = 0;

                if (_dictionaryBags.TryGetValue(cardDataModel._id, out InGame_CardDataModelLevels cardInBag))
                {
                    old = cardInBag._currentCard;
                    newValue = old + cardDataModel._amount;
                    currentGoal = cardInBag.CurrentGoal;

                    cardInBag.AddACard(cardDataModel, out bool isLevelUp);
                    if (isLevelUp)
                        levelUpIfHas.Add(cardInBag);
                }
                else
                {
                    InGame_CardDataModelLevels c = new InGame_CardDataModelLevels(cardDataModel, 1);
                    this._bag.Add(c);
                    this._dictionaryBags.Add(c._cardID, c);

                    old = 0;
                    newValue = cardDataModel._amount;
                    currentGoal = c.CurrentGoal;
                }

                this._currentCoinPoint += cardDataModel._coinPoint;


                InGamePlayerUpgradePalletAnim.Instance.ShowCollectUpgrade(cardDataModel._id, old, newValue, currentGoal);
            }
        }
        return levelUpIfHas;
    }

    public bool IsOwningThisCardID(int cardID)
    {
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelLevels>();
        return this._dictionaryBags.ContainsKey(cardID);
    }
}
public class BaseInGameEnemyDataModel : BaseInGamePlayerDataModel
{
    public InGameEnemyStatConfig _statConfig;

    public BaseInGameEnemyDataModel SetStatConfig(InGameEnemyStatConfig config)
    {
        _statConfig = config;
        this.SetHP(config.MaxHP);
        return this;
    }
}