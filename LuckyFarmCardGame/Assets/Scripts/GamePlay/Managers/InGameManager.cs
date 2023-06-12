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
    #endregion Property in Inspector

    #region Data
    protected int _turnIndex;
    #endregion Data

    #region Getter
    public InGameBasePlayerItem CurrentTurnPlayer => _players[this._turnIndex];


    #endregion Getter

    private void Start()
    {
        //Init Game
        InitGame();
        //Assign Game Rule Component

        //Init Game Controller

        GameController?.AddCallback_PalletConflict(this.OnPalletConflict);
        GameController?.AddCallback_PalletDestroyed(this.OnPalletDestroyed);
        GameController?.AddCallback_PalletPulledByRule(this.OnPalletPulledByRule);

        GameController?.InitGame();

        //Start the game
        OnBeginTurn();
    }

    protected void InitGame()
    {
        int amountPlayerJoin = 2;//Random.Range(2, this._players.Count); //pulling this info fromn JoinGameData outside
        Debug.Log($"INGAME MANGE: Init {amountPlayerJoin} players");
        //Init player seat
        InitPlayers(amountPlayerJoin);

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

    #region Turn Action
    public void OnBeginTurn()
    {
        this.GameController?.BeginTurn();
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
    #endregion Turn Actions

    #region Pallet behavior
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
    }
    #endregion End game behavior

    #region Card activator behavior
    public void OnTellControllerContinueTurn()
    {
        this.GameController?.ContinueTurn();
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
}
