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

    public virtual int CurrentHP { get => currentHP; set => currentHP = value; }
    public virtual int MaxHP { get => maxHP; set => maxHP = value; }

    protected int currentHP;
    protected int maxHP;

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
        if(this._cardID == palletCard._id)
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
    public InGame_CardDataModelWithAmount PullSplitCard(out bool splitToTwoSuccess ,out InGame_CardDataModelWithAmount splitedCard, int amountToSplit = 1)
    {
        splitedCard = new InGame_CardDataModelWithAmount(this) { _amountCard = amountToSplit };
        splitToTwoSuccess = true;
        if(this._amountCard <= amountToSplit)
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
    public List<InGame_CardDataModelWithAmount> _bag;
    public Dictionary<int, InGame_CardDataModelWithAmount> _dictionaryBags;

    public int AmountCardInBag => (_bag == null || _bag.Count == 0) ? 0 : this._bag.Sum(x => x._amountCard);
    public bool IsHasCardIsBag => AmountCardInBag > 0;



    public BaseInGameMainPlayerDataModel() : base()
    {
        this._bag = new List<InGame_CardDataModelWithAmount>();
        this._dictionaryBags = new Dictionary<int, InGame_CardDataModelWithAmount>();
    }
    public bool TryGetCardInBag(int id, out InGame_CardDataModelWithAmount card)
    {
        _dictionaryBags ??= new Dictionary<int, InGame_CardDataModelWithAmount>();
        return _dictionaryBags.TryGetValue(id, out card);
    }

    /// <summary>
    /// Add cards to player bags
    /// These usually from pallet
    /// </summary>
    /// <param name="cards"></param>
    public void AddCardsToPallet(List<InGame_CardDataModel> cards)
    {
        _bag ??= new List<InGame_CardDataModelWithAmount>();
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelWithAmount>();
        if (cards != null && cards.Count > 0)
        {
            foreach (InGame_CardDataModel cardDataModel in cards)
            {
                if (_dictionaryBags.TryGetValue(cardDataModel._id, out InGame_CardDataModelWithAmount cardInBag))
                {
                    cardInBag.AddACard(cardDataModel);
                }
                else
                {
                    InGame_CardDataModelWithAmount c = new InGame_CardDataModelWithAmount(cardDataModel);
                    this._bag.Add(c);
                    this._dictionaryBags.Add(c._cardID, c);
                }

                this._currentCoinPoint += cardDataModel._coinPoint;
            }
        }
    }
    public void RemoveACard(InGame_CardDataModelWithAmount card)
    {
        _bag ??= new List<InGame_CardDataModelWithAmount>();
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelWithAmount>();
        if (this._dictionaryBags.ContainsKey(card._cardID))
        {
            _bag.Remove(card);
            _dictionaryBags.Remove(card._cardID);
        }
    }
}
public class BaseInGameEnemyDataModel : BaseInGamePlayerDataModel
{

}