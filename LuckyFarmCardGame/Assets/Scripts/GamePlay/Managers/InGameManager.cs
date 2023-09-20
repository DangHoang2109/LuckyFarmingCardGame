using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manage GameState: WinLose, State, Init Match
/// </summary>
public class InGameManager : MonoSingleton<InGameManager>
{
    #region Property in Inspector
    [SerializeField]
    private CardGameController _gameController;
    public CardGameController GameController => _gameController;

    /// <summary>
    /// Seat for 4 players, but not every match will have enough 4 players
    /// The left over will be disable
    /// </summary>
    [SerializeField]
    protected List<InGameBasePlayerItem> _players;

    protected List<BaseInGamePlayerDataModel> _playersModels;

    [Space(5f)]
    [SerializeField] protected InGameTurnNotification _notificator;
    public InGameTurnNotification Notificator => _notificator;

    [Space(5f)]
    [SerializeField] protected ConfirmUsingInGameGameCoinToAddDiceResultPopup _confirmUsingCoin;
    public ConfirmUsingInGameGameCoinToAddDiceResultPopup ConfirmUsingCoin => _confirmUsingCoin;

    #endregion Property in Inspector

    #region Data
    protected int _turnIndex;

    protected GameState _gameState;
    public bool IsPlaying => this._gameState == GameState.PLAYING;

    public int _currentTurnCoinNeedUsing = 0;
    #endregion Data

    #region Getter
    public InGameBasePlayerItem CurrentTurnPlayer => _players[this._turnIndex];
    public BaseInGamePlayerDataModel CurrentTurnPlayerModel => _players[this._turnIndex]?.PlayerModel;


    #endregion Getter


    private void Start()
    {
        this._gameState = GameState.WAITING;

        //Init Game
        InitGame();
        //Assign Game Rule Component

        //Init Game Controller
        GameController?.AddCallback_CardOnGoingDrawed(this.OnACardGoingBeDrawed);
        GameController?.AddCallback_CardPutToPallet(this.OnLetUserActionWhenCardActiveEffectWhenPutToPallet);
        GameController?.AddCallback_PalletConflict(this.OnPalletConflict);
        GameController?.AddCallback_PalletDestroyed(this.OnPalletDestroyed);
        GameController?.AddCallback_PalletPulledByRule(this.OnPalletPulledByRule);
        GameController?.AddCallback_DiceResultShowed(this.DiceResultShowed);

        GameController?.InitGame();

        //Start the game
        StartGame();
    }

    protected void InitGame()
    {
        int amountPlayerJoin = 2;//Random.Range(2, this._players.Count); //pulling this info fromn JoinGameData outside
        Debug.Log($"INGAME MANGE: Init {amountPlayerJoin} players");
        //Init player seat
        InitPlayers(amountPlayerJoin);

        //Init using coin confirmation
        ConfirmUsingCoin?.Init(OnUserDecideToUseGameCoin);

        //Roll a turn index 
        this._turnIndex = Random.Range(0, amountPlayerJoin);
    }

    protected void InitPlayers(int amountPlayerJoin)
    {
        _playersModels ??= new List<BaseInGamePlayerDataModel>();
        if (this._players != null && this._players.Count > 0)
        {
            //Pull a random goal Config to this player;
            List<InGameMissionGoalCardConfig> goalConfigs = InGameMissionGoalCardConfigs.Instance.GetRandomConfigs(amountPlayerJoin);
            for (int i = 0; i < this._players.Count; i++)
            {
                _players[i].gameObject.SetActive(i < amountPlayerJoin);
                if (i >= amountPlayerJoin)
                    continue;

                //change to parse from player item
                BaseInGamePlayerDataModel playerModel = new BaseInGamePlayerDataModel()
                                                                .SetSeatID(id: i, isMain: i == 0);

                _players[i]
                    .SetAPlayerModel(playerModel)
                    .AddMissionGoal(goalConfigs[i]);
                this._playersModels.Add(playerModel);
            }
        }
    }
    public void StartGame()
    {
        this._gameState = GameState.PLAYING;

        OnBeginTurn();
    }

    #region Turn Action
    public void OnBeginTurn()
    {
        this.GameController?.BeginTurn(CurrentTurnPlayer.IsMainPlayer);
        Debug.Log($"GAME MANGE: Player seat {this._turnIndex} begin turn");
        CurrentTurnPlayer.BeginTurn();
    }
    public void ShowNotificationCardAction(string text)
    {
        this.Notificator?.ShowText(text, this.CurrentTurnPlayer.IsMainPlayer);
    }

    public void OnDrawCard()
    {
        GameController?.OnDrawACard();
    }
    /// <summary>
    /// called by ingame controller when a card is drawing
    /// </summary>
    private void OnACardGoingBeDrawed()
    {
        this.CurrentTurnPlayer?.OnACardGoingBeDrawed();
    }
    public void OnUserEndTurn()
    {
        //pull the card by user choice;
        GameController?.PullCardFromPalletToUser(OnPullingAnimationComplete);

        void OnPullingAnimationComplete(List<InGame_CardDataModel> cardsReceive)
        {
            OnUserPullCardFromPalletToBag(cardsReceive);
            OnLogicEndTurn();
        }
    }
    private void OnUserPullCardFromPalletToBag(List<InGame_CardDataModel> cardsReceive)
    {
        if (cardsReceive != null && cardsReceive.Count > 0)
        {
            this.CurrentTurnPlayer.PullCardToBag(cardsReceive);
        }
    }


    /// <summary>
    /// User dùng coin để thay đổi kết quả dice, hoặc tùy theo game rule
    /// </summary>
    /// <param name="amountUsing"></param>
    public void OnUserDecideToUseGameCoin(int amountUsing)
    {
        //trừ coin từ user đang dùng
        if(this.CurrentTurnPlayerModel?.SubtractGameCoinIfCan(amountUsing) ?? false)
        {
            this.GameController?.OnUserDecideToUseGameCoin(amountUsing);
        }
    }
    /// <summary>
    /// User dùng coin để thay đổi kết quả dice, hoặc tùy theo game rule
    /// </summary>
    /// <param name="amountUsing"></param>
    public void OnUserDecideToUseGameCoin()
    {
        //trừ coin từ user đang dùng
        if(this._currentTurnCoinNeedUsing > 0)
            if (this.CurrentTurnPlayerModel?.SubtractGameCoinIfCan(_currentTurnCoinNeedUsing) ?? false)
            {
                this.GameController?.OnUserDecideToUseGameCoin(_currentTurnCoinNeedUsing);
            }
    }
    /// <summary>
    /// Một card có effect đã được đưa vào trong pallet
    /// inform cho user biết để xử lý
    /// </summary>
    public void OnLetUserActionWhenCardActiveEffectWhenPutToPallet(int cardID)
    {
        this.CurrentTurnPlayer?.Action_ACardPutToPallet(cardID);
    }
    /// <summary>
    /// Một card có effect đã được đưa vào trong pallet
    /// inform cho user biết để xử lý
    /// </summary>
    public void OnLetUserActionWhenPalletConflictAndNeedUseCoin(int amountCoinNeeding, int pointAdding)
    {
        this.CurrentTurnPlayer?.Action_DecideAndUseCoin(amountCoinNeeding, pointAdding);
    }

    private void OnLogicEndTurn()
    {
        //Check end game
        if (this.CurrentTurnPlayer.IsWin())
        {
            Debug.Log($"INGAME MANGE: Seat {CurrentTurnPlayer.ID} won the game");
            OnEndGame();
            return;
        }
        CurrentTurnPlayer.EndTurn();

        //roll to next index
        this._turnIndex = InGameUtils.RollIndex(this._turnIndex, this._playersModels.Count);

        //if not endgame
        //begin next user turn
        OnBeginTurn();
    }

    public void OnBotClickChoseToggleBagUIItem(int playerID, int cardID)
    {
        InGameBasePlayerItem player = this._players.Find(x => x.ID == playerID);
        if(player != null)
        {
            if(player.BagVisual.TryFindUIItem(cardID, out InGameBagCardTypeUIItem uiCardItem))
            {
                uiCardItem.ClickToggleFromMManager();
            }
        }
    }

    private void Update()
    {
        if(IsPlaying)
            CustomUpdate();
    }
    protected virtual void CustomUpdate()
    {
        this.CurrentTurnPlayer?.CustomUpdate();
    }
    #endregion Turn Actions

    #region Pallet behavior
    public void DiceResultShowed(int diceResult, int pointNeeding, bool willBeDestroy)
    {
        int amountCointNeed = pointNeeding * (GameController?.CoinForEachDicePoint ?? 1);
        if (willBeDestroy && this.CurrentTurnPlayerModel.IsCanUseGameCoin(amountCointNeed))
        {
            _currentTurnCoinNeedUsing = amountCointNeed;
            OnLetUserActionWhenPalletConflictAndNeedUseCoin(amountCointNeed, pointNeeding);
        }
    }
    /// <summary>
    /// Normally only Main Player will call this
    /// </summary>
    public void ShowConfirmUsingCoin(int amountCoinNeeding, int pointAdding)
    {
        this.ConfirmUsingCoin?.ParseDataAndShow(amountCoinNeedToUse: amountCoinNeeding, amountDiceResWillBeAdd: pointAdding);
    }
    public void OnPalletConflict()
    {

    }
    public void OnPalletDestroyed()
    {
        OnLogicEndTurn();
    }
    public void OnPalletPulledByRule(List<InGame_CardDataModel> cardsReceive)
    {
        OnUserPullCardFromPalletToBag(cardsReceive);
        OnLogicEndTurn();
    }
    #endregion Pallet behavior

    #region End game behavior
    public void OnEndGame()
    {
        Debug.Log($"INGAME MANGE: End Game");
        this._gameState = GameState.ENDING;

    }
    #endregion End game behavior

    #region Card activator behavior
    public void OnTellControllerContinueTurn()
    {
        this.GameController?.ContinueTurn();
        CurrentTurnPlayer?.ContinueTurn();
    }
    public void OnTellControllerToRollDice()
    {
        this.GameController?.RollADiceAndCheckPalletCondition();
    }
    public void OnTellControllerToDrawCards(int cardToDraw)
    {
        for (int i = 0; i < cardToDraw; i++)
        {
            this.OnDrawCard();
        }
    }
    public void OnTellControllerToRevealTopCard(int cardToReveal)
    {
        this.GameController?.OnRevealTopDeck(cardToReveal);
    }
    /// <summary>
    /// Return result active succes or not to card activator
    /// </summary>
    /// <returns></returns>
    public bool OnTellControllerToDestroyOtherCard()
    {
        //Bật bag chose pallet của các player khác
        List<int> otherPlayerID = GenIdsOtherPlayer();
        bool activeSuccess = otherPlayerID != null && otherPlayerID.Count >= 1;
        if (activeSuccess)
        {
            foreach (int id in otherPlayerID)
            {
                _players[id].ReadyInDestroyCardEffectStage(1, OnCompleteChosing);
            }
        }
        else
        {
            Debug.Log("GAME MANGE: No player has card in bag to destroy");
            OnTellControllerContinueTurn();
        }
        return activeSuccess;

        List<int> GenIdsOtherPlayer()
        {
            List<int> idNotMe = new List<int>();
            foreach (BaseInGamePlayerDataModel item in this._playersModels)
            {
                if (item._id != this.CurrentTurnPlayer.ID && item.IsHasCardIsBag)
                    idNotMe.Add(item._id);
            }
            return idNotMe;
        }
        void OnCompleteChosing(int playerBeingChoseID, List<int> cardsChosed)
        {
            //Only support cxard chose 1 destroying
            GameController?.DestroyPlayerCard(player: _players[playerBeingChoseID], cardsChosed[0]);
            this.Notificator?.DisableText();
        }
    }
    /// <summary>
    /// Return result active succes or not to card activator
    /// </summary>
    /// <returns></returns>
    public bool OnTellControllerToPullMyCard()
    {
        if (!CurrentTurnPlayer.IsHasCardIsBag)
        {
            OnTellControllerContinueTurn();
            return false;
        }

        CurrentTurnPlayer.ReadyInPullingCardEffectStage(1, OnCompleteChosing);
        return true;
        void OnCompleteChosing(int turnPlayerID, List<int> cardsChosed)
        {
            //Only support cxard chose 1 destroying
            if(turnPlayerID == this.CurrentTurnPlayer.ID)
                this.GameController?.PullPlayerCardToHisPallet(this.CurrentTurnPlayer, cardsChosed[0]);

            this.Notificator?.DisableText();
        }
    }
    #endregion Card activator behavior


    #region Bot Looker API Need
    public List<InGameAI.OtherPlayerLookingInfo> GetOtherPlayerLookingInfos()
    {
        List<InGameAI.OtherPlayerLookingInfo> res = new List<InGameAI.OtherPlayerLookingInfo>();
        int currentTurnId = this.CurrentTurnPlayer.ID;
        foreach (InGameBasePlayerItem playerItem in this._players)
        {
            if (!playerItem.IsPlaying || playerItem.ID == currentTurnId)
                continue;

            BaseInGamePlayerDataModel model = playerItem.PlayerModel;
            InGameAI.OtherPlayerLookingInfo info = new InGameAI.OtherPlayerLookingInfo()
            {
                _playerID = model._id,
                _currentCoin = model.CurrentCoinPoint,
                _totalCard = model.AmountCardInBag,
                _bagList = model._bag,
                _bagDic = model._dictionaryBags
            };
            res.Add(info);
        }
        return res;
    }
    #endregion Bot Looker API Need
}

public enum GameState
{
    WAITING = 0,
    PLAYING = 1,
    PAUSING = 2,
    ENDING = 3,
}