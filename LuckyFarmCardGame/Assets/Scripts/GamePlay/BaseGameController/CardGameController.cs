using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameController : MonoBehaviour
{
    #region Callback 
    //Khi user draw ra 1 card đã có trên pallet
    public System.Action _onPalletConflict;
    //Pallet conflict after: roll dice nhưng bị destroy
    public System.Action _onPalletDestroyed;
    //Pallet conflict after: roll dice và thắng roll
    public System.Action<List<InGame_CardDataModel>> _onRuleMakeUserPullCard;
    #endregion Callback

    #region Data Prop
    public List<InGame_CardDataModel> _cardsOnPallet;

    protected InGameDeckConfig _deckConfig;
    protected List<int> _currentDeck;

    public int DeckCardAmount => _currentDeck?.Count ?? 0;

    #endregion Data Prop
    public void InitGame()
    {
        _cardsOnPallet = new List<InGame_CardDataModel>();

        //get deck contain
        _deckConfig = InGameDeckConfigs.Instance.GetStandardDeck();
        RecreateTheDeck();
    }

    #region Action with Deck
    protected virtual void RecreateTheDeck()
    {
        if (_deckConfig != null)
        {
            _currentDeck ??= new List<int>();
            foreach (InGame_CardDataModelWithAmount cardDataModelWithAmount in _deckConfig._deckContain)
            {
                for (int i = 0; i < cardDataModelWithAmount._amountCard; i++)
                {
                    _currentDeck.Add(cardDataModelWithAmount._cardID);
                }
            }

            _currentDeck.Shuffle();
        }
    }
    protected void CheckDeck()
    {
        if (DeckCardAmount <= 0)
            RecreateTheDeck();
    }
    #endregion Action with Deck

    public void AddCallback_PalletConflict(System.Action cb)
    {
        _onPalletConflict -= cb;
        _onPalletConflict += cb;
    }
    public void AddCallback_PalletDestroyed(System.Action cb)
    {
        _onPalletDestroyed -= cb;
        _onPalletDestroyed += cb;
    }
    public void AddCallback_PalletPulledByRule(System.Action<List<InGame_CardDataModel>> cb)
    {
        _onRuleMakeUserPullCard -= cb;
        _onRuleMakeUserPullCard += cb;
    }


    #region Draw and Interacting with Pallet
    public void OnRevealTopDeck(int amount)
    {
        List<InGame_CardDataModel> topCards = GetDeckTopCards(amount,isWillPopThatCardOut: false);
        string topContent = "";
        foreach (InGame_CardDataModel item in topCards)
        {
            topContent += $"{item._id} ";
        }
        Debug.Log($"CONTROLLER: Reveal {amount} top card {topContent}");
    }
    public void OnDrawACard()
    {
        //Get a card from deck
        InGame_CardDataModel topDeckCard = GetDeckTopCard(isWillPopThatCardOut: true);

        //activate that card effect
        topDeckCard?.OnDrawedFromDeck();

        //put it to pallet
        PutACardToPallet(topDeckCard);
    }

    private InGame_CardDataModel CreateCardDataModel(int id)
    {
        return new InGame_CardDataModel().SetCardID(id);
    }
    private InGame_CardDataModel GetDeckTopCard(bool isWillPopThatCardOut)
    {
        CheckDeck();

        int topCardId = this._currentDeck?[0] ?? 0;

        if (isWillPopThatCardOut)
            _currentDeck.RemoveAt(0);

        return CreateCardDataModel(topCardId);
    }
    private List<InGame_CardDataModel> GetDeckTopCards(int amount,bool isWillPopThatCardOut)
    {
        List<InGame_CardDataModel> top = new List<InGame_CardDataModel>();
        for (int i = 0; i < amount; i++)
        {
            top.Add(GetDeckTopCard(isWillPopThatCardOut));
        }
        return top;
    }
    private void PutACardToPallet(InGame_CardDataModel card)
    {
        _cardsOnPallet ??= new List<InGame_CardDataModel>();

        //if in pallet already has this card
        int indexOfSameIDCardIfExistOnPallet = _cardsOnPallet.FindIndex(x => x._id == card._id);
        if (indexOfSameIDCardIfExistOnPallet >= 0 && indexOfSameIDCardIfExistOnPallet < _cardsOnPallet.Count)
        {
            Debug.Log($"RULE: EXISTED CARD {card._id} at index {indexOfSameIDCardIfExistOnPallet}");
            _onPalletConflict?.Invoke();
            OnPalletConflict();
        }
        //else: let it in
        else
        {
            _cardsOnPallet.Add(card);
            Debug.Log($"RULE: ADD CARD {card._id} at index {_cardsOnPallet.Count - 1}");

            //activate that card effect
            card?.OnPlacedToPallet();
        }
    }

    private void OnPalletConflict()
    {
        //ask the rule to see what to do?
        ThisFuncShouldOnRule_TellControllerToRollingDice();

        void ThisFuncShouldOnRule_TellControllerToRollingDice()
        {
            RollADiceAndCheckPalletCondition();
        }
    }
    public void RollADiceAndCheckPalletCondition()
    {
        if (this._cardsOnPallet == null || this._cardsOnPallet.Count == 0)
            return;

        int diceResult = this.RollADice();
        Debug.Log($"RULE: Rolling dice {diceResult} compare to pallet {this._cardsOnPallet.Count}");
        //if dice result > pallet amount card => the pallet will be destroyed
        if (diceResult > _cardsOnPallet.Count)
        {
            this.DestroyPallet();
        }
        else
        {
            this.RuleForce_PullCardFromPalletToUser();
        }
    }
    public void DestroyPallet()
    {
        //play the destroying card animation
        if (this._cardsOnPallet == null || this._cardsOnPallet.Count == 0)
            return;

        //active effect of card
        foreach (InGame_CardDataModel cardDataModel in _cardsOnPallet)
        {
            cardDataModel.OnDestroyedByConflict();
        }

        Debug.Log("CONTROLER: DESTROY PALLET");
        _onPalletDestroyed?.Invoke();


        ClearPallet();
    }
    public void RuleForce_PullCardFromPalletToUser()
    {
        Debug.Log("CONTROLER: RULE PULL PALLET");

        List<InGame_CardDataModel> receiveCards = PullCardFromPalletToUser();
        _onRuleMakeUserPullCard?.Invoke(receiveCards);
    }
    public List<InGame_CardDataModel> PullCardFromPalletToUser()
    {
        Debug.Log("CONTROLER: PULL PALLET");

        List<InGame_CardDataModel> cacheCardsInPallet = new List<InGame_CardDataModel>(this._cardsOnPallet);

        //active effect of card
        foreach (InGame_CardDataModel cardDataModel in _cardsOnPallet)
        {
            cardDataModel.OnPulledToBag();
        }

        ClearPallet();

        return cacheCardsInPallet;
    }
    private void ClearPallet()
    {
        this._cardsOnPallet ??= new List<InGame_CardDataModel>();
        this._cardsOnPallet.Clear();
    }
    private int RollADice()
    {
        return UnityEngine.Random.Range(0, 6);
    }
    #endregion Draw and Interacting with Pallet

    #region Interacting with player bag
    public void DestroyPlayerCard(BaseInGamePlayer player, int cardIDToDestroy)
    {
        if(player != null)
        {
            player.DestroyMyCardByOther(cardIDToDestroy);
        }
    }
    public void PullPlayerCardToHisPallet(BaseInGamePlayer player, int cardIDToPull)
    {
        if (player != null)
        {
            InGame_CardDataModelWithAmount cardPulled = player.PullMyCardToThePallet(cardIDToPull);
            PutACardToPallet(CreateCardDataModel(cardPulled._cardID));

        }
    }
    #endregion Interacting with player bag
}
