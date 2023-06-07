using System.Collections;
using System.Collections.Generic;
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
            for (int i = 0; i < this._players.Count; i++)
            {
                _players[i].gameObject.SetActive(i < amountPlayerJoin);
                if (i >= amountPlayerJoin)
                    break;


                BaseInGamePlayerDataModel playerModel = new BaseInGamePlayerDataModel()
                                                                .SetSeatID(id: i, isMain: i == 0);

                _players[i].SetAPlayerModel(playerModel);
                this._playersModels.Add(playerModel);
            }
        }
    }

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
        List<InGame_CardDataModel> cardsReceive = GameController?.PullCardFromPalletToUser();
        OnUserPullCardFromPalletToBag(cardsReceive);

        OnLogicEndTurn();
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
        //roll to next index
        this._turnIndex = InGameUtils.RollIndex(this._turnIndex, this._playersModels.Count);

        //Check end game

        //if not endgame
        //begin next user turn
        OnBeginTurn();
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

}
