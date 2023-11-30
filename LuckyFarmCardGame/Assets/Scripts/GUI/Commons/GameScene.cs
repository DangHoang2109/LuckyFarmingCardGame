using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override void StartScene(Action callback = null)
    {
        base.StartScene(callback);
        InGameManager.Instance.PrepareGame();
    }
}
