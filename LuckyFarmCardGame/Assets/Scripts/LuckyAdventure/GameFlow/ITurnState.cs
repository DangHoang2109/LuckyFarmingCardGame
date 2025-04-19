using System.Collections;
using System.Collections.Generic;
using LuckyAdventure.GameUnit;
using UnityEngine;

public class ITurnState : IState
{
    public Unit PlayerTurn {  get; private set; }
    public int PlayerIndex { get; private set; }

    public virtual void Init(Unit playerTurn, int playerIndex)
    {
        PlayerIndex = playerIndex;
        PlayerTurn = playerTurn;
    }
}
