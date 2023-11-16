using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class CardGameController : MonoBehaviour
{
    #region Callback 
    // Một card sắp được draw ra
    public System.Action _onGoingDrawingCard;
    // Một card được đưa vào pallet
    public System.Action<int> _onCardPutToPallet;
    //Khi user draw ra 1 card đã có trên pallet
    public System.Action _onPalletConflict;
    //Khi dice đã roll và có thể bị destroy hay ko, invoke dice result và có bị destroy pallet ko
    public System.Action<int,int,bool> _onDiceShowedResult;
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

    [SerializeField] protected CardAnimationItem _cardAnim;
    [SerializeField] protected BaseCardItem _cardPrefab;
    [SerializeField] protected BaseCardCircleItem _cardCirclePrefab;

    [SerializeField] protected Transform _tfPalletPanel, _tfActEffectPanel, _tfDeckPanel;
    [Space(5f)]
    [SerializeField] protected GameObject _gLightningAnimator;
    //[Space(5f)]
    //[SerializeField] protected InGameDiceRollingAnimator _diceAnimator;
    //public InGameDiceRollingAnimator DiceAnimator => _diceAnimator;

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

    [Space(5f)]
    protected InGameManager _ingameManager;
    public InGameManager InGameManager
    {
        get
        {
            if (_ingameManager == null)
                _ingameManager = InGameManager.Instance;
            return _ingameManager;
        }
    }
    #endregion Prop on Editor

    #region Data Prop
    protected List<InGame_CardDataModel> _cardsOnPallet;
    public List<InGame_CardDataModel> CardsOnPallet;

    protected InGameDeckConfig _deckConfig;
    protected List<int> _currentDeck;

    public int DeckCardAmount => _currentDeck?.Count ?? 0;

    protected int _currentTurnDiceResult;
    public int CurrentTurnDiceResult => _currentTurnDiceResult;
    protected Coroutine _coroutineWaitDecidingUseGameCoin;

    protected bool _isMainUserTurn = false;
    #endregion Data Prop

    #region Config
    protected int _coinForEachDicePoint = 5;
    public int CoinForEachDicePoint => _coinForEachDicePoint;

    #endregion

    public void InitGame()
    {
        InitGame(InGameDeckConfigs.Instance.GetStandardDeck());
    }
    public void InitGame(InGameDeckConfig deckConfig)
    {
        _cardsOnPallet = new List<InGame_CardDataModel>();

        //get deck contain
        _deckConfig = deckConfig;
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

    #region Callback Adding
    public void AddCallback_CardOnGoingDrawed(System.Action cb)
    {
        _onGoingDrawingCard -= cb;
        _onGoingDrawingCard += cb;
    }
    public void AddCallback_CardPutToPallet(System.Action<int> cb)
    {
        _onCardPutToPallet -= cb;
        _onCardPutToPallet += cb;
    }
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
    public void AddCallback_DiceResultShowed(System.Action<int,int,bool> cb)
    {
        _onDiceShowedResult -= cb;
        _onDiceShowedResult += cb;
    }

    #endregion Callback Adding


    #region Turn Action
    public void BeginTurn(bool isMainUserTurn)
    {
        _isMainUserTurn = isMainUserTurn;
        this.EnableDrawingCardFromDeck(_isMainUserTurn);
    }
    public void ContinueTurn()
    {
        this.EnableDrawingCardFromDeck(_isMainUserTurn);
    }
    /// <summary>
    /// This flow is shit
    /// Gamemanager cần phải biết lúc nào gamecontroller có khả năng tiếp tục turn : Các animtion đã xong
    /// Cái flow nhùng này là vì hiện tại gamecontroller đang đảm nhiệm animation bằng coroutine luôn
    /// Nếu sau này implement 1 hệ thống animation riêng thì sẽ ko bị nữa
    /// </summary>
    public void TellGameManagerICanContinueTurn()
    {
        InGameManager.OnTellControllerContinueTurn();
    }
    /// <summary>
    /// User dùng coin để thay đổi kết quả dice, hoặc tùy theo game rule
    /// </summary>
    /// <param name="amountUsing"></param>
    public void OnUserDecideToUseGameCoin(int amountUsing)
    {
        //check result dice luôn, vì user đã thao tác rồi
        if (this._coroutineWaitDecidingUseGameCoin != null)
            StopCoroutine(_coroutineWaitDecidingUseGameCoin);

        if (amountUsing > 0)
        {
            int amountAdding = amountUsing / _coinForEachDicePoint;
            //tăng số trên dice 1 điểm với mỗi _coinForEachDicePoint amount point
            //mechanic này để giảm độ rủi ro với việc roll dice, vì số dice bé hơn pallet thì user sẽ bị destroy
            this._currentTurnDiceResult += Mathf.RoundToInt(amountAdding);

            //this.DiceAnimator?.AddDiceValue(amountAdding, _currentTurnDiceResult, OnFinalCheckDiceResultWithPallet);
        }
        else
        {
            OnFinalCheckDiceResultWithPallet();
        }
    }
    #endregion Turn Action

    #region Draw and Interacting with Pallet
    public void OnRevealTopDeck(int amount)
    {
        List<InGame_CardDataModel> topCards = GetDeckTopCards(amount,isWillPopThatCardOut: false);
        StartCoroutine(ieOnRevealcardsAndDestroy(topCards));

        IEnumerator ieOnRevealcardsAndDestroy(List<InGame_CardDataModel> topCards)
        {
            //List<BaseCardItem> cardRevealWhichWIllBeDestroy = new List<BaseCardItem>();

            string topContent = "";
            foreach (InGame_CardDataModel item in topCards)
            {
                topContent += $"{item._id} ";

                BaseCardItem newCardItem = CreateCardItem(item._id, _tfDeckPanel);
                yield return new WaitForSeconds(this.AnimationTimeConfig._timeWaitRevealTopDeck);
                Destroy(newCardItem.gameObject);
                yield return new WaitForEndOfFrame();
            }

            InGameManager.Instance.Notificator?.DisableText();
            TellGameManagerICanContinueTurn();
            Debug.Log($"CONTROLLER: Reveal {amount} top card {topContent}");
        }
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
        int currentCardLevel = this.InGameManager?.MainUserPlayer?.GetCardLevel(id) ?? 0;

        return new InGame_CardDataModel().SetHost(InGameManager.CurrentTurnPlayer).SetCardID(id, cardConfig).SetCurrentLevel(currentCardLevel);
    }
    private BaseCardItem CreateCardItem(int cardID, Transform whereToSpawn)
    {
        BaseCardItem newCardItem = Instantiate(_cardPrefab, whereToSpawn);
        newCardItem.transform.localPosition = Vector3.zero;
        newCardItem.gameObject.SetActive(true);
        newCardItem.ParseHost(InGameManager.CurrentTurnPlayer).ParseInfo(cardID);
        return newCardItem;
    }
    private BaseCardItem CreateCardItem(ref InGame_CardDataModel card)
    {
        BaseCardItem newCardItem = Instantiate(_cardCirclePrefab, _tfActEffectPanel);
        newCardItem.transform.localPosition = Vector3.zero;
        newCardItem.gameObject.SetActive(true);
        newCardItem.ParseHost(InGameManager.CurrentTurnPlayer).ParseInfo(card._id); 
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
        _cardAnim.PlayDraw(card._id, cb: OnCompleteAnimationDrawFromDeck);

        void OnCompleteAnimationDrawFromDeck()
        {
            BaseCardItem newCardItem = CreateCardItem(ref card); //spawn a circle in standing 
            StartCoroutine(ieDrawAndAddCardToPallet(card, newCardItem));
        }
    }



    protected IEnumerator ieDrawAndAddCardToPallet(InGame_CardDataModel card, BaseCardItem cardItem)
    {
        yield return new WaitForSeconds(0.2f); //this.AnimationTimeConfig._timeACardStayOnActiveEffectPanel

        //if in pallet already has this card
        int indexOfSameIDCardIfExistOnPallet = _cardsOnPallet.FindIndex(x => x._id == card._id);
        if (indexOfSameIDCardIfExistOnPallet >= 0 && indexOfSameIDCardIfExistOnPallet < _cardsOnPallet.Count)
        {
            Debug.Log($"RULE: EXISTED CARD {card._id} at index {indexOfSameIDCardIfExistOnPallet}");
            _gLightningAnimator.gameObject.SetActive(true);
            yield return new WaitForSeconds(this.AnimationTimeConfig._timeConflictAnimationShowing);
            _gLightningAnimator.gameObject.SetActive(false);

            yield return new WaitForSeconds(this.AnimationTimeConfig._timeWaitOnBeforeDestroyPallet);

            _onPalletConflict?.Invoke();
            //destroying the conflict card
            cardItem.OnDestroyingEffect();
            OnPalletConflict();
        }
        //else: let it in
        else
        {
            _cardsOnPallet.Add(card);


            //Let it move to pallet
            cardItem.transform.SetParent(this._tfPalletPanel);
            _onCardPutToPallet?.Invoke(card._id);

            yield return new WaitForEndOfFrame(); //this.AnimationTimeConfig._timeACardStayOnActiveEffectPanel

            //activate that card effect
            card?.OnPlacedToPallet();
        }
    }

    private void OnPalletConflict()
    {
        //update rule of axie: Pallet conflict will cause to destroy it, remove the dice
        this.DestroyPallet();

        ////ask the rule to see what to do?
        //ThisFuncShouldOnRule_TellControllerToRollingDice();

        //void ThisFuncShouldOnRule_TellControllerToRollingDice()
        //{
        //    RollADiceAndCheckPalletCondition();
        //}
    }
    public void RollADiceAndCheckPalletCondition()
    {
        if (this._cardsOnPallet == null || this._cardsOnPallet.Count == 0)
            return;

        _currentTurnDiceResult = this.RollADice();

        bool willDestroy = WillPalletBeDestroyWithThisDiceResult(out int pointNeeding);

        Debug.Log($"RULE: Rolling dice {_currentTurnDiceResult} compare to pallet {this._cardsOnPallet.Count}");
        //if dice result < pallet amount card => the pallet will be destroyed

        //this.DiceAnimator?.RollingDice(_currentTurnDiceResult, stayWhenRollComplete: willDestroy, callback: OnDiceShowResultComplte, this.AnimationTimeConfig._timeWaitDiceRolling, this.AnimationTimeConfig._timeWaitShowDiceResult);

        void OnDiceShowResultComplte()
        {
            float timeWaitForDeciding = 15f;

            Debug.Log($"CONTROLER: Dice anim complete, star coroutine waiting for {timeWaitForDeciding} sec");

            _onDiceShowedResult?.Invoke(_currentTurnDiceResult, pointNeeding, willDestroy);
            if (willDestroy && InGameManager.CurrentTurnPlayerModel.IsCanUseGameCoin(pointNeeding* _coinForEachDicePoint))
                this._coroutineWaitDecidingUseGameCoin = StartCoroutine(ieWaitPlayerDecideToUseCoin(timeWaitForDeciding));
            else
                OnFinalCheckDiceResultWithPallet();
        }
        IEnumerator ieWaitPlayerDecideToUseCoin(float timeWaitForDeciding)
        {
            yield return new WaitForSeconds(timeWaitForDeciding);

            OnFinalCheckDiceResultWithPallet();
        }
    }
    //Lúc này nếu user có dùng coin thì kết quả turnDiceResult đã đổi,
    protected void OnFinalCheckDiceResultWithPallet()
    {
        Debug.Log($"CONTROLER: Wait deciding complete, handle dice result");
        //DiceAnimator?.Hide();

        if (WillPalletBeDestroyWithThisDiceResult(out _))
        {
            this.DestroyPallet();
        }
        else
        {
            this.RuleForce_PullCardFromPalletToUser();
        }
    }
    protected bool WillPalletBeDestroyWithThisDiceResult(out int pointNeedToSafe)
    {
        pointNeedToSafe = _cardsOnPallet.Count - _currentTurnDiceResult + 1;

        return _currentTurnDiceResult <= _cardsOnPallet.Count;
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
        return UnityEngine.Random.Range(1, 7);
    }
    #endregion Draw and Interacting with Pallet

    #region Interacting with player bag

    #endregion Interacting with player bag

    #region Bot Looker API Need
    /// <summary>
    /// Tỉ lệ conflict hiện tại 
    /// </summary>
    /// <returns></returns>
    public float GetPalletConflictChance()
    {
        //đếm số card có cùng id với pallet hiện tại, lấy số lượng đó chia cho amount deck hiện tại
        if (this._cardsOnPallet.Count == 0) 
            return 0f;

        this.CheckDeck();
        int amountCardCauseConflict = this._currentDeck.Where(x => _cardsOnPallet.Find((c)=> c._id == x) != null).Count();
        return amountCardCauseConflict / DeckCardAmount;
    }
    public List<int> GetCurrentPalletIDs()
    {
        List<int> pallet = new List<int>();
        foreach (InGame_CardDataModel item in this.CardsOnPallet)
        {
            pallet.Add(item._id);
        }
        return pallet;
    }
    /// <summary>
    /// Lấy lượng coin nhận được khi pull pallet này về
    /// </summary>
    /// <returns></returns>
    public int GetPalletTotalCoin()
    {
        //đếm số card có cùng id với pallet hiện tại, lấy số lượng đó chia cho amount deck hiện tại
        if (this._cardsOnPallet.Count == 0)
            return 0;
        return _cardsOnPallet.Sum(x => x._coinPoint);
    }
    public InGameBaseCardEffectID GetTopDeckEffect()
    {
        CheckDeck();
        return this.GetDeckTopCard(isWillPopThatCardOut: false)._effect;
    }
    public int GetTopDeckID()
    {
        CheckDeck();
        return this.GetDeckTopCard(isWillPopThatCardOut: false)._id;
    }
    public bool GetResultPalletConflictIfThisCardJoin(int cardID)
    {
        CheckDeck();

        if (this._cardsOnPallet.Count == 0)
            return false;

        return this.CardsOnPallet.Find(x => x._id == cardID) != null;
    }
    #endregion Bot Looker API Need
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