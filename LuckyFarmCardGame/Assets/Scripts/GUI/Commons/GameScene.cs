using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override void StartScene(Action callback = null)
    {
        base.StartScene(callback);
        InGameManager.Instance.PrepareGame(CreateTempJoinGameData());
    }

    static TempJoinGameData CreateTempJoinGameData()
    {
        InGamePlayerConfig playerConfig = TestSceneLoader.Instance.CurrentPlayerConfig;
        InGameDeckConfig deckConfig = playerConfig.DeckConfig;
        return new TempJoinGameData()
        {
            _playerConfig = playerConfig,
            _playerDeck = deckConfig
        };
    }
}

public class TempJoinGameData
{
    //current player stat
    public InGamePlayerConfig _playerConfig;
    /// <summary>
    /// player original deck or adding bonus card or previous game card
    /// </summary>
    public InGameDeckConfig _playerDeck;
}