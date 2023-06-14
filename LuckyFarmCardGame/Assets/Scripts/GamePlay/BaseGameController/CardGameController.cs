﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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


    #region Prop on Editor
    [SerializeField]
    protected InGameAnimationTimeController _animationConfig;
    public InGameAnimationTimeController AnimationTimeConfig => _animationConfig;
    [Space(5f)]

    [SerializeField] protected BaseCardItem _cardPrefab;
    [SerializeField] protected Transform _tfPalletPanel, _tfActEffectPanel, _tfDeckPanel;
    [Space(5f)]
    [SerializeField] protected GameObject _gLightningAnimator;
    [Space(5f)]
    [SerializeField] protected InGameDiceRollingAnimator _diceAnimator;
    public InGameDiceRollingAnimator DiceAnimator => _diceAnimator;

    [Space(5f)]
    public Button _btnDeckDraw;

    [Space(5f)]
    [SerializeField] protected InGameTurnNotification _notificator;

    [Space(5f)]
    protected InGameCardConfigs _ingameCardConfigs;
    public InGameCardConfigs IngameCardConfigs
    {
        get 
        {
            if (_ingameCardConfigs == null)
                _ingameCardConfigs = InGameCardConfigs.Instance;
            return _ingameCardConfigs;
        }
    }
    #endregion Prop on Editor

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

        this.EnableDrawingCardFromDeck(true);
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

            ////test deck top
            //Debug.LogError("THE DECK CONTENT IS NOT RANDOM");
            //_currentDeck.Insert(0, 2);
            //_currentDeck.Insert(1, 4);
            //_currentDeck.Insert(0, 0);

            //end test deck top
        }
    }
    protected void CheckDeck()
    {
        if (DeckCardAmount <= 0)
            RecreateTheDeck();
    }
    public void EnableDrawingCardFromDeck(bool isAllow)
    {
        if(_btnDeckDraw != null)
            this._btnDeckDraw.interactable = isAllow;
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

    #region Turn Action
    public void BeginTurn()
    {
        this.EnableDrawingCardFromDeck(true);
    }
    public void ContinueTurn()
    {
        this.EnableDrawingCardFromDeck(true);
    }
    #endregion Turn Action

    #region Draw and Interacting with Pallet
    public void OnRevealTopDeck(int amount)
    {
        List<InGame_CardDataModel> topCards = GetDeckTopCards(amount,isWillPopThatCardOut: false);

        List<BaseCardItem> cardRevealWhichWIllBeDestroy = new List<BaseCardItem>();
        string topContent = "";
        foreach (InGame_CardDataModel item in topCards)
        {
            topContent += $"{item._id} ";

            BaseCardItem newCardItem = CreateCardItem(item._id, _tfDeckPanel);
            cardRevealWhichWIllBeDestroy.Add(newCardItem);
        }

        StartCoroutine(ieOnRevealComplete());

        IEnumerator ieOnRevealComplete()
        {
            yield return new WaitForSeconds(this.AnimationTimeConfig._timeWaitRevealTopDeck);
            for (int i = cardRevealWhichWIllBeDestroy.Count - 1; i >=0 ; i--)
            {
                Destroy(cardRevealWhichWIllBeDestroy[i].gameObject);
            }
            InGameManager.Instance.Notificator?.DisableText();

            ContinueTurn();
        }
        Debug.Log($"CONTROLLER: Reveal {amount} top card {topContent}");
    }
    public void OnDrawACard()
    {
        this.EnableDrawingCardFromDeck(false);

        //Get a card from deck
        InGame_CardDataModel topDeckCard = GetDeckTopCard(isWillPopThatCardOut: true);

        //activate that card effect
        topDeckCard?.OnDrawedFromDeck();

        //put it to pallet
        PutACardToPallet(topDeckCard);
    }

    private InGame_CardDataModel CreateCardDataModel(int id)
    {        
        InGameCardConfig cardConfig = IngameCardConfigs?.GetCardConfig(id);

        return new InGame_CardDataModel().SetCardID(id, cardConfig);
    }
    private BaseCardItem CreateCardItem(int cardID, Transform whereToSpawn)
    {
        BaseCardItem newCardItem = Instantiate(_cardPrefab, whereToSpawn);
        newCardItem.transform.localPosition = Vector3.zero;
        newCardItem.gameObject.SetActive(true);
        newCardItem.ParseInfo(cardID);
        return newCardItem;
    }
    private BaseCardItem CreateCardItem(ref InGame_CardDataModel card)
    {
        BaseCardItem newCardItem = Instantiate(_cardPrefab, this._tfActEffectPanel);
        newCardItem.gameObject.SetActive(true);
        newCardItem.ParseInfo(card._id);

        card.SetCardItemContainer(newCardItem);
        return newCardItem;
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

        BaseCardItem newCardItem = CreateCardItem(ref card);

        StartCoroutine(ieDrawAndAddCardToPallet(card, newCardItem));
    }



    protected IEnumerator ieDrawAndAddCardToPallet(InGame_CardDataModel card, BaseCardItem cardItem)
    {
        yield return new WaitForSeconds(this.AnimationTimeConfig._timeACardStayOnActiveEffectPanel);

        //if in pallet already has this card
        int indexOfSameIDCardIfExistOnPallet = _cardsOnPallet.FindIndex(x => x._id == card._id);
        if (indexOfSameIDCardIfExistOnPallet >= 0 && indexOfSameIDCardIfExistOnPallet < _cardsOnPallet.Count)
        {
            Debug.Log($"RULE: EXISTED CARD {card._id} at index {indexOfSameIDCardIfExistOnPallet}");
            _gLightningAnimator.gameObject.SetActive(true);
            yield return new WaitForSeconds(this.AnimationTimeConfig._timeConflictAnimationShowing);
            _gLightningAnimator.gameObject.SetActive(false);

            //destroying the conflict card
            cardItem.OnDestroyingEffect();

            yield return new WaitForSeconds(this.AnimationTimeConfig._timeWaitOnBeforeDestroyPallet);

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

            //Let it move to pallet
            cardItem.transform.SetParent(this._tfPalletPanel);

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
        //if dice result < pallet amount card => the pallet will be destroyed

        this.DiceAnimator?.RollingDice(diceResult, callback: OnDiceShowResultComplte, this.AnimationTimeConfig._timeWaitDiceRolling, this.AnimationTimeConfig._timeWaitShowDiceResult);

        void OnDiceShowResultComplte()
        {
            if (diceResult < _cardsOnPallet.Count)
            {
                this.DestroyPallet();
            }
            else
            {
                this.RuleForce_PullCardFromPalletToUser();
            }
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

        PullCardFromPalletToUser(OnPullingComplete);

        void OnPullingComplete(List<InGame_CardDataModel> receiveCards)
        {
            _onRuleMakeUserPullCard?.Invoke(receiveCards);
        }
    }
    public void PullCardFromPalletToUser(System.Action<List<InGame_CardDataModel>> onAnimationComplete)
    {
        Debug.Log("CONTROLER: PULL PALLET");

        List<InGame_CardDataModel> cardFromPallet = new List<InGame_CardDataModel>(this._cardsOnPallet);

        //active effect of card
        foreach (InGame_CardDataModel cardDataModel in _cardsOnPallet)
        {
            //behavior of card
            cardDataModel.OnPulledToBag();
        }

        StartCoroutine(iePullingCardFromPalletToBag());

        IEnumerator iePullingCardFromPalletToBag()
        {
            yield return new WaitForSeconds(this.AnimationTimeConfig._timeWaitPullingOnBeforeClearingPallet);
            ClearPallet();
            onAnimationComplete?.Invoke(cardFromPallet);
        }
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
    public void DestroyPlayerCard(InGameBasePlayerItem player, int cardIDToDestroy)
    {
        StartCoroutine(ieAnimation());

        IEnumerator ieAnimation()
        {
            yield return new WaitForSeconds(this.AnimationTimeConfig?._timeWaitAnimationDestroyingCard ?? 0);
            if (player != null)
            {
                player.DestroyMyCardByOther(cardIDToDestroy);
            }

            ContinueTurn();
        }
    }
    public void PullPlayerCardToHisPallet(InGameBasePlayerItem player, int cardIDToPull)
    {
        if (player != null)
        {
            InGame_CardDataModelWithAmount cardPulled = player.PullMyCardToThePallet(cardIDToPull);
            PutACardToPallet(CreateCardDataModel(cardPulled._cardID));


            ContinueTurn();
        }
    }
    #endregion Interacting with player bag
}

[System.Serializable]
public class InGameAnimationTimeController
{
    public float _timeConflictAnimationShowing;

    public float _timeACardStayOnActiveEffectPanel;
    public float _timeWaitOnBeforeDestroyPallet;
    public float _timeWaitPullingOnBeforeClearingPallet;

    public float _timeWaitDiceRolling;
    public float _timeWaitShowDiceResult;

    public float _timeWaitBeforeEnableDeckDrawable;

    public float _timeWaitRevealTopDeck;
    public float _timeWaitAnimationDestroyingCard;
    public float _timeWaitAnimationPullingCard;

}