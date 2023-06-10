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
    protected List<BaseInGamePlayer> _players;

    protected List<BaseInGamePlayerDataModel> _playersModels;
    #endregion Property in Inspector

    #region Data
    protected int _turnIndex;
    #endregion Data

    #region Getter
    public BaseInGamePlayer CurrentTurnPlayer => _players[this._turnIndex];


    #endregion Getter

    private void Start()
    {
        //Init Game
        InitGame();
        //Assign Game Rule Component

        //Init Game Controller
        GameController?.InitGame();
        GameController?.AddCallback_PalletConflict(this.OnPalletConflict);
        GameController?.AddCallback_PalletDestroyed(this.OnPalletDestroyed);
        GameController?.AddCallback_PalletPulledByRule(this.OnPalletPulledByRule);


        //Start the game
        OnBeginTurn();
    }

    protected void InitGame()
    {
        int amountPlayerJoin = Random.Range(2, this._players.Count); //pulling this info fromn JoinGameData outside
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
                    break;

                BaseInGamePlayerDataModel playerModel = new BaseInGamePlayerDataModel()
                                                                .SetSeatID(id: i, isMain: i == 0)
                                                                .AddMissionGoal(goalConfigs[i]);

                _players[i].SetAPlayerModel(playerModel);
                this._playersModels.Add(playerModel);
            }
        }
    }

    #region Turn Action
    public void OnBeginTurn()
    {
        Debug.Log($"GAME MANGE: Player seat {this._turnIndex} begin turn");
        CurrentTurnPlayer.BeginTurn();
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
    public void OnTellControllerToDestroyOtherCard()
    {
        Debug.Log("GAME MANGE: Pick a player to destroy");
        int idPlayerDestroy = Test_RandAPlayerToDestroy();

        if(idPlayerDestroy >= 0 && idPlayerDestroy < this._playersModels.Count)
        {
            int cardTODesrtoy = Test_RandACardIDToDestroy(_playersModels[idPlayerDestroy]);
            GameController?.DestroyPlayerCard(player: this._players[idPlayerDestroy], cardTODesrtoy);
        }
        else
        {
            Debug.Log("GAME MANGE: No player has card in bag to destroy");
        }

        int Test_RandAPlayerToDestroy()
        {
            List<int> idNotMe = new List<int>();
            foreach (BaseInGamePlayerDataModel item in this._playersModels)
            {
                if (item._id != this.CurrentTurnPlayer.ID && item._bag.Count > 0)
                    idNotMe.Add(item._id);
            }
            if (idNotMe.Count == 0)
                return -1;

            return idNotMe[Random.Range(0, idNotMe.Count)];
        }
        int Test_RandACardIDToDestroy(BaseInGamePlayerDataModel player)
        {
            return player._bag.GetRandom()._cardID;
        }
    }

    public void OnTellControllerToPullMyCard()
    {
        int cardTOPull = Test_RandACardIDToPull(CurrentTurnPlayer.PlayerModel);

        if (cardTOPull >= 0)
            this.GameController?.PullPlayerCardToHisPallet(this.CurrentTurnPlayer, Test_RandACardIDToPull(CurrentTurnPlayer.PlayerModel));
        else
            Debug.Log("INGAME MANAGE: NO CARD IN BAG TO PULL");
        int Test_RandACardIDToPull(BaseInGamePlayerDataModel player)
        {
            if (player._bag.Count > 0)
                return player._bag.GetRandom()._cardID;
            else
                return -1;
        }
    }
    #endregion Card activator behavior
}
