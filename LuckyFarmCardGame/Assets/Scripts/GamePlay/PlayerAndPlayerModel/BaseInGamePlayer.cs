using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInGamePlayer : MonoBehaviour
{
    protected BaseInGamePlayerDataModel _playerModel;
    public BaseInGamePlayerDataModel PlayerModel => _playerModel;

    public int ID => this.PlayerModel?._id ?? -1;
    /// <summary>
    /// Calling by InGameManager when set up init game
    /// </summary>
    /// <param name="model"></param>
    public virtual void SetAPlayerModel(BaseInGamePlayerDataModel model)
    {
        this._playerModel = model;
    }
    public bool TryGetCardInBag(int id, out InGame_CardDataModelWithAmount card)
    {
        card = null;
        return PlayerModel?.TryGetCardInBag(id, out card) ?? false;
    }

    public virtual void BeginTurn()
    {
    }

    /// <summary>
    /// Called by IngameManager when this user endturn or force endturn success by rolling dice
    /// </summary>
    public virtual void PullCardToBag(List<InGame_CardDataModel> cardReceive)
    {
        if (cardReceive != null && cardReceive.Count > 0) 
        {
            this.PlayerModel.AddCardsToPallet(cardReceive);
        }

        Debug.Log($"PLAYER {this.ID} bag: {this.PlayerModel._dictionaryBags.DebugDicCardInGame()}");
    }
    public void DestroyMyCardByOther(int cardIDToDestroy, int amountToDestroy = 1)
    {
        if(this.PlayerModel != null)
        {
            PlayerModel.DestroyMyCardByOther(cardIDToDestroy, amountToDestroy);
        }
    }
    public InGame_CardDataModelWithAmount PullMyCardToThePallet(int cardIdToPull)
    {
        if (this.PlayerModel != null)
        {
             PlayerModel.PullMyCardToThePallet(cardIdToPull, out InGame_CardDataModelWithAmount splitCard);
            return splitCard;
        }
        return null;
    }
    public virtual bool IsWin()
    {
        return this.PlayerModel?.IsWin() ?? false;
    }
}

public class BaseInGamePlayerDataModel
{
    public int _id;
    public bool _isMainPlayer;

    public List<InGame_CardDataModelWithAmount> _bag;
    public Dictionary<int,InGame_CardDataModelWithAmount> _dictionaryBags;

    protected InGameMissionGoalCardConfig _goalCardConfig;
    public InGameMissionGoalCardConfig GoalCardConfig => _goalCardConfig;
    public BaseInGamePlayerDataModel()
    {
        this._bag = new List<InGame_CardDataModelWithAmount>();
        this._dictionaryBags = new Dictionary<int, InGame_CardDataModelWithAmount>();
    }
    public BaseInGamePlayerDataModel SetSeatID(int id, bool isMain)
    {
        this._id = id;
        this._isMainPlayer = isMain;
        return this;
    }
    public BaseInGamePlayerDataModel AddMissionGoal(InGameMissionGoalCardConfig goalCardConfig)
    {
        this._goalCardConfig = goalCardConfig;

        Debug.Log($"PLAYER {_id} goal: {goalCardConfig._requirement.DebugListCardInGame()}");

        return this;
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
        if (cards != null && cards.Count >0)
        {
            foreach (InGame_CardDataModel cardDataModel in cards)
            {
                if(_dictionaryBags.TryGetValue(cardDataModel._id, out InGame_CardDataModelWithAmount cardInBag))
                {
                    cardInBag.AddACard(cardDataModel);
                }
                else
                {
                    InGame_CardDataModelWithAmount c = new InGame_CardDataModelWithAmount(cardDataModel);
                    this._bag.Add(c);
                    this._dictionaryBags.Add(c._cardID, c);
                }
            }
        }
    }
    public void DestroyMyCardByOther(int cardIDToDestroy, int amountToDestroy = 1)
    {
        _bag ??= new List<InGame_CardDataModelWithAmount>();
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelWithAmount>();

        if (_bag.Count > 0 && TryGetCardInBag(cardIDToDestroy, out InGame_CardDataModelWithAmount card))
        {
            card.SubtractCard(amountToDestroy);
            //refresh card bag 

            Debug.Log($"PLAYER {this._id}: Destroy card complete {InGameUtils.DebugDicCardInGame(this._dictionaryBags)}");
        }
        else
            Debug.LogError("PLAYER: ERROR NOT FOUND CARD TO DESTROY");
    }
    public bool PullMyCardToThePallet(int cardIdToPull, out InGame_CardDataModelWithAmount splitedCard)
    {
        _bag ??= new List<InGame_CardDataModelWithAmount>();
        this._dictionaryBags ??= new Dictionary<int, InGame_CardDataModelWithAmount>();

        if (_bag.Count > 0 && TryGetCardInBag(cardIdToPull, out InGame_CardDataModelWithAmount card))
        {
            card.PullSplitCard(out bool splitToTwoSuccess, out splitedCard);
            if (!splitToTwoSuccess)
            {
                RemoveACard(card);
            }
            //refresh card bag 
            Debug.Log($"PLAYER {this._id}: Pull card complete {InGameUtils.DebugDicCardInGame(this._dictionaryBags)}");
            return true;
        }
        else
            Debug.LogError("PLAYER: ERROR NOT FOUND CARD TO PULL");

        splitedCard = null;
        return false;
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
    public bool IsWin()
    {
        if(this.GoalCardConfig != null)
        {
            foreach (InGame_CardDataModelWithAmount require in GoalCardConfig._requirement)
            {
                //Not have requirement card or have but amount is not enough => return false
                if (TryGetCardInBag(require._cardID, out InGame_CardDataModelWithAmount cardInBag))
                {
                    if (!cardInBag.CompareEnoughOrHigher(require))
                        return false;
                }
                else
                    return false;
            }
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