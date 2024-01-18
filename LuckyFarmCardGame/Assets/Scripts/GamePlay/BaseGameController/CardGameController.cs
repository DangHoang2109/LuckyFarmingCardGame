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
    public System.Action<int, int, bool> _onDiceShowedResult;
    //Pallet conflict after: roll dice nhưng bị destroy
    public System.Action _onPalletDestroyed;
    //Pallet conflict after: roll dice và thắng roll
    public System.Action<List<InGame_CardDataModel>> _onRuleMakeUserPullCard;

    /// <summary>
    /// Invoke current amount card in deck
    /// </summary>
    public System.Action<int> _onDeckAmountChanging;
    #endregion Callback

    #region Prop on Editor
    [SerializeField]
    protected InGameAnimationTimeController _animationConfig;
    public InGameAnimationTimeController AnimationTimeConfig => _animationConfig;
    [Space(5f)]

    [SerializeField] protected CardAnimationItem _cardAnim;
    public CardAnimationItem CardDrawingAnimator => _cardAnim;

    [SerializeField] protected BaseCardCircleItem _cardCirclePrefab;

    [SerializeField] protected Transform _tfActEffectPanel, _tfDeckPanel;
    [SerializeField] protected CardPointPalletUI _palletUI;

    [Space(5f)]
    [SerializeField] protected ShrineItem _gShrineBonusStage;

    [Space(5f)]
    [SerializeField] protected GameObject _gLightningAnimator;
    [SerializeField] public InGamePlayerUpgradePalletAnim _upgradeCollectorAnim;
    //[Space(5f)]
    //[SerializeField] protected InGameDiceRollingAnimator _diceAnimator;
    //public InGameDiceRollingAnimator DiceAnimator => _diceAnimator;

    [Space(5f)]
    public InGameDeckUI _uiDeckDraw;
    public ExplodeRateMater _explodeMeter;

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

    public static int MAX_ENEMY_ALLOW = 3;
    #endregion Data Prop

    #region Config
    protected int _coinForEachDicePoint = 5;
    public int CoinForEachDicePoint => _coinForEachDicePoint;

    #endregion

    public void InitGame(InGameDeckConfig deckConfig)
    {
        _cardsOnPallet = new List<InGame_CardDataModel>();

        //get deck contain
        _deckConfig = new InGameDeckConfig(deckConfig);
        RecreateTheDeck();

        this._uiDeckDraw._onClickDraw -= this.OnDrawACard;
        this._uiDeckDraw._onClickDraw += this.OnDrawACard;
        this._onDeckAmountChanging -= _uiDeckDraw.OnChangeCardAmount;
        this._onDeckAmountChanging += _uiDeckDraw.OnChangeCardAmount;

        _explodeMeter.Init(this);
        this._onDeckAmountChanging -= _explodeMeter.OnChangeDeckOrPalletInfo;
        this._onDeckAmountChanging += _explodeMeter.OnChangeDeckOrPalletInfo;
        this._onCardPutToPallet -= _explodeMeter.OnChangeDeckOrPalletInfo;
        this._onCardPutToPallet += _explodeMeter.OnChangeDeckOrPalletInfo;
        _onDeckAmountChanging?.Invoke(this.DeckCardAmount);
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
            _onDeckAmountChanging?.Invoke(this.DeckCardAmount);
            _uiDeckDraw?.PlayAnimationShuffleDeck();
        }
    }
    protected void CheckDeck()
    {
        if (DeckCardAmount <= 0)
            RecreateTheDeck();
    }
    public void EnableDrawingCardFromDeck(bool isAllow)
    {
        this._uiDeckDraw?.SetInterractable(isAllow);
    }

    /// <summary>
    /// Can be use with re-order or tutorial to setup card
    /// </summary>
    /// <param name="cardIds"></param>
    public virtual void PlaceCardOnTopDeck(List<int> cardIds)
    {
        CheckDeck();
        _currentDeck.InsertRange(0, cardIds);
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
    public void AddCallback_DiceResultShowed(System.Action<int, int, bool> cb)
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
        List<InGame_CardDataModel> topCards = GetDeckTopCards(amount, isWillPopThatCardOut: false);
        RevealCardsTopDeckDialog d = RevealCardsTopDeckDialog.ShowDialog();
        d.ParseData(topCards, cbClose: OnRevealClosed);

        void OnRevealClosed()
        {
            //InGameManager.Instance.Notificator?.DisableText();
            TellGameManagerICanContinueTurn();
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

    private BaseCardItem CreateCardItem(ref InGame_CardDataModel card)
    {
        BaseCardItem newCardItem = Instantiate(_cardCirclePrefab, _tfActEffectPanel);
        newCardItem.transform.localPosition = Vector3.zero;
        newCardItem.gameObject.SetActive(true);
        newCardItem.ParseHost(InGameManager.CurrentTurnPlayer).ParseInfo(card);
        card.SetCardItemContainer(newCardItem);
        return newCardItem;
    }
    private InGame_CardDataModel GetDeckTopCard(bool isWillPopThatCardOut)
    {
        CheckDeck();

        int topCardId = this._currentDeck?[0] ?? 0;

        if (isWillPopThatCardOut)
        {
            _currentDeck.RemoveAt(0);
            _onDeckAmountChanging?.Invoke(this.DeckCardAmount);
            CheckDeck();
        }

        return InGameUtils.CreateCardDataModel(topCardId);
    }
    public List<InGame_CardDataModel> GetDeckTopCards(int amount, bool isWillPopThatCardOut)
    {
        List<InGame_CardDataModel> top = new List<InGame_CardDataModel>();
        CheckDeck();
        amount = Mathf.Min(amount, this.DeckCardAmount); //ko cho reveal hơn lượng deck còn lại: reveal 3 card tnog khi deck còn 2
        for (int i = 0; i < amount; i++)
        {
            int topCardId = this._currentDeck?[i] ?? 0;
            top.Add(InGameUtils.CreateCardDataModel(topCardId));
        }
        if (isWillPopThatCardOut)
        {
            _currentDeck.RemoveRange(0, amount);
            _onDeckAmountChanging?.Invoke(this.DeckCardAmount);
            CheckDeck();
        }

        return top;
    }
    public void AddCardToDeck(int cardID, int amount)
    {
        CheckDeck();

        for (int i = 0; i < amount - 1; i++)
        {
            _currentDeck.Insert(index: Random.Range(0, _currentDeck.Count), cardID);
        }
        //có ít nhất 1 card sẽ được add trong vòng 8 card đổ lại để user thấy được gợi nhớ (~ avg 1 turn draw 4 card)
        _currentDeck.Insert(index: Random.Range(2, 8), cardID);

        //add vào deck config để hết deck thì recreate vẫn có
        this._deckConfig.AddNewCard(cardID, amount);

        _onDeckAmountChanging?.Invoke(this.DeckCardAmount);
    }
    public bool IsOwningThisCardIDInDeck(int cardID)
    {
        return this._deckConfig.IsContain(cardID);
    }
    public List<InGame_CardDataModelWithAmount> GetAllCardDeckContain()
    {
        return this._deckConfig._deckContain;
    }
    public List<InGame_CardDataModelWithAmount> GetAllCardBonusDeckContain()
    {
        List < InGame_CardDataModelWithAmount > allContain = this._deckConfig._deckContain;
        List<InGame_CardDataModelWithAmount> res = new List<InGame_CardDataModelWithAmount>();
        foreach (var item in allContain)
        {
            if (InGameCardConfigs.Instance.GetCardConfig(item._cardID)?._isBonusTier ?? false)
                res.Add(item);   
        }
        return res;
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
            _gLightningAnimator.gameObject.SetActive(true);
            yield return new WaitForSeconds(this.AnimationTimeConfig._timeConflictAnimationShowing);
            _gLightningAnimator.gameObject.SetActive(false);

            yield return new WaitForSeconds(this.AnimationTimeConfig._timeWaitOnBeforeDestroyPallet);

            //_onPalletConflict?.Invoke();
            //destroying the conflict card
            cardItem.OnDestroyingEffect();
            OnPalletConflict();
        }
        //else: let it in
        else
        {
            _cardsOnPallet.Add(card);

            //Let it move to pallet
            _palletUI.AppendItem(cardItem);
            _onCardPutToPallet?.Invoke(card._id);

            yield return new WaitForEndOfFrame(); //this.AnimationTimeConfig._timeACardStayOnActiveEffectPanel

            //activate that card effect
            card?.OnPlacedToPallet();
        }
    }

    private void OnPalletConflict()
    {
        ///rule mới: thay vì pallet bị destroy, làm quái mạnh lên và bắt buộc thu card nếu pallet bị conflict để reduce pain khi liều bị fail
        InGameManager.OnUserEndTurn();

        void OldPhase2()
        {
            //update rule of axie: Pallet conflict will cause to destroy it, remove the dice
            this.DestroyPallet();
        }

        void OldPhase1()
        {
            ////ask the rule to see what to do?
            //ThisFuncShouldOnRule_TellControllerToRollingDice();

            //void ThisFuncShouldOnRule_TellControllerToRollingDice()
            //{
            //    RollADiceAndCheckPalletCondition();
            //}
        }
    }
    #region Roll Dice Mechanic
    private int RollADice()
    {
        return UnityEngine.Random.Range(1, 7);
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
            if (willDestroy && InGameManager.CurrentTurnPlayerModel.IsCanUseGameCoin(pointNeeding * _coinForEachDicePoint))
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
    #endregion 

    public void DestroyPallet()
    {
        //play the destroying card animation
        if (this._cardsOnPallet == null || this._cardsOnPallet.Count == 0)
            return;

        //active effect of card
        //foreach (InGame_CardDataModel cardDataModel in _cardsOnPallet)
        //{
        //    cardDataModel.OnDestroyedByConflict();
        //}

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
        //Debug.Log("CONTROLER: PULL PALLET");

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
        _palletUI.ClearPallet();

        this._explodeMeter.OnChangeDeckOrPalletInfo(0);
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
        int amountCardCauseConflict = this._currentDeck.Where(x => _cardsOnPallet.Find((c) => c._id == x) != null).Count();
        return (float)amountCardCauseConflict / DeckCardAmount;
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

    #region Shrine And Layout Of Game
    public void GoShrineBonus()
    {
        //this._gShrineBonusStage.gameObject.SetActive(true);
        this._palletUI.Turn(false);
        _gShrineBonusStage.Show(OnCompleteShowShrine);

        void OnCompleteShowShrine()
        {
            InGameManager.OnCompleteShowShrine();
        }
    }
    public void QuitShrineBonus()
    {
        _gShrineBonusStage.Hide(() => {
            //this._gShrineBonusStage.gameObject.SetActive(false);
            this._palletUI.Turn(true);
        });
    }
    #endregion Shrine And Layout Of Game
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