using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InGameBasePlayerItem : MonoBehaviour
{
    #region Prop on editor
    public TMPro.TextMeshProUGUI _tmpCoinValue;

    public HPBarUI _hpBar;
    public ShieldUI _shieldUI;
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
    public virtual int CurrentShield
    {
        get
        {
            return this.PlayerModel?.Shield ?? 0;
        }
        set
        {
            if (this.PlayerModel != null)
            {
                this.PlayerModel.Shield = value;
                this._shieldUI.UpdateValue(this.PlayerModel.Shield);
            }
        }
    }

    public bool IsPlaying => this._playerModel != null;
    protected BaseInGamePlayerDataModel _playerModel;
    public BaseInGamePlayerDataModel PlayerModel => _playerModel;

    public int ID => this.PlayerModel?._id ?? -1;
    public bool IsMainPlayer => ID == 0;

    //in effect card
    protected List<int> CardBeingChose; protected int _amountCardToBeChoseInEffect; protected System.Action<int, List<int>> _onCompleteBeingChoseInEffect;

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
            _onCompleteBeingChoseInEffect?.Invoke(this.ID, new List<int>(CardBeingChose));

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
            _onCompleteBeingChoseInEffect?.Invoke(this.ID, new List<int>(CardBeingChose));

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
    public virtual void Attacked(int dmg)
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
            InGameManager.Instance.OnAPlayerDie(this.ID);
        }
    }
    public virtual void AddHP(int heal)
    {
        this.CurrentHP += heal;
    }
    public virtual void AddShield(int shieldUnit = -1)
    {
        if (shieldUnit <= 0)
            shieldUnit = 1; //replace with this host info
        Debug.Log("HOST: CREATE SHIELD" + shieldUnit);
        this.CurrentShield += shieldUnit;
    }
    public virtual void ResetShield()
    {
        Debug.Log("HOST: RESET SHIELD");
        this.CurrentShield = 0;
    }

    public virtual void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info
        Debug.Log("HOST: ATTACK ONE UNIT" + dmg);

        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info
        Debug.Log("HOST: ATTACK ALL UNIT" + dmg);

        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void Heal(int heal = -1)
    {
        if (heal <= 0)
            heal = 1; //replace with this host info
        Debug.Log("HOST: HEAL" + heal);
        InGameManager.Instance.OnPlayerHeal(this.ID, heal);
        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    public virtual void DefenseCreateShield(int shieldUnit = -1)
    {
        if (shieldUnit <= 0)
            shieldUnit = 1; //replace with this host info
        Debug.Log("HOST: DEFENSE" + shieldUnit);
        InGameManager.Instance.OnPlayerDefense(this.ID, shieldUnit);

        InGameManager.Instance.OnTellControllerContinueTurn();
    }
    #endregion Turn Action

    public virtual bool isDead()
    {
        return this.PlayerModel?.IsDead() ?? false;
    }

    public virtual void CustomUpdate()
    {
    }
    

}

public class BaseInGamePlayerDataModel
{
    public int _id;
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
    public int Shield { get => shield; set => shield = Mathf.Clamp(value, 0, int.MaxValue); }

    protected int currentHP;
    protected int maxHP;
    protected int shield;

    public BaseInGamePlayerDataModel()
    {
    }
    public BaseInGamePlayerDataModel SetSeatID(int id, bool isMain)
    {
        this._id = id;
        this._isMainPlayer = isMain;
        return this;
    }
    public BaseInGamePlayerDataModel SetHP(int maxHP)
    {
        this.CurrentHP = this.MaxHP = maxHP;
        return this;
    }
    public virtual BaseInGamePlayerDataModel StartGame()
    {
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

}


public class BaseInGameMainPlayerDataModel : BaseInGamePlayerDataModel
{
    public List<InGame_CardDataModelLevels> _bag;
    public Dictionary<int, InGame_CardDataModelLevels> _dictionaryBags;

    public int AmountCardInBag => 0;//(_bag == null || _bag.Count == 0) ? 0 : this._bag.Sum(x => x._amountCard);
    public bool IsHasCardIsBag => AmountCardInBag > 0;

    public BaseInGameMainPlayerDataModel() : base()
    {
        this._bag = new List<InGame_CardDataModelLevels>();
        this._dictionaryBags = new Dictionary<int, InGame_CardDataModelLevels>();
    }
    public override BaseInGamePlayerDataModel StartGame()
    {
        //setting up the dic with full of user monster card, starting at level 1
        var _deckConfig = InGameDeckConfigs.Instance.GetStandardDeck();
        if(_deckConfig != null)
        {
            foreach (var item in _deckConfig._deckContain)
            {
                InGame_CardDataModelLevels c = new InGame_CardDataModelLevels(id: item._cardID);
                this._bag.Add(c);
                this._dictionaryBags.Add(c._cardID, c);
            }
        }
        this._bag.DebugListCardInGame();
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
    public List<InGame_CardDataModel> AddCardsToPallet(List<InGame_CardDataModel> cards)
    {
        _bag ??= new List<InGame_CardDataModelLevels>();
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelLevels>();

        List<InGame_CardDataModel> levelUpIfHas = new List<InGame_CardDataModel>();

        if (cards != null && cards.Count > 0)
        {
            foreach (InGame_CardDataModel cardDataModel in cards)
            {
                if (_dictionaryBags.TryGetValue(cardDataModel._id, out InGame_CardDataModelLevels cardInBag))
                {
                    cardInBag.AddACard(cardDataModel, out bool isLevelUp);
                    if (isLevelUp)
                        levelUpIfHas.Add(cardDataModel);
                }
                else
                {
                    InGame_CardDataModelLevels c = new InGame_CardDataModelLevels(cardDataModel, 1);
                    this._bag.Add(c);
                    this._dictionaryBags.Add(c._cardID, c);
                }

                this._currentCoinPoint += cardDataModel._coinPoint;
            }
        }
        return levelUpIfHas;
    }
}
public class BaseInGameEnemyDataModel : BaseInGamePlayerDataModel
{

}