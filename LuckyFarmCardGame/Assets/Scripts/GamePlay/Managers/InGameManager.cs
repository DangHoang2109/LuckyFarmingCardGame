using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage GameState: WinLose, State, Init Match
/// </summary>
public class InGameManager : MonoSingleton<InGameManager>
{
    [SerializeField]
    private CardGameController _gameController;
    public CardGameController GameController => _gameController;


    private void Start()
    {
        //Init Game

        //Assign Game Rule Component

        //Init Game Controller
        GameController?.InitGame();
        GameController?.AddCallback_PalletConflict(this.OnPalletConflict);
        GameController?.AddCallback_PalletDestroyed(this.OnPalletDestroyed);
        GameController?.AddCallback_PalletPulledByRule(this.OnPalletPulledByRule);

    }



    public void OnDrawCard()
    {
        GameController?.OnDrawACard();
    }
    public void OnUserEndTurn()
    {
        //pull the card by user choice;
        GameController?.PullCardFromPalletToUser();
    }
    public void OnPalletConflict()
    {

    }
    public void OnPalletDestroyed()
    {

    }
    public void OnPalletPulledByRule()
    {

    }

}
