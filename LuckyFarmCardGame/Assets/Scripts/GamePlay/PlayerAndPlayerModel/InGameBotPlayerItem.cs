using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBotPlayerItem : InGameBasePlayerItem
{
    #region Prop on editor

    #endregion Prop on editor

    #region Data

    protected InGameAI.AIExecutor _AIexecutor;
    protected InGameAI.AILooker _AIlooker;
    protected InGameAI.AIDecider _AIdecider;

    #endregion Data

    #region Getter
    public InGameAI.AILooker Looker
    {
        get
        {
            if (this._AIlooker == null)
            {
                this._AIlooker = new InGameAI.AILooker(this);
            }

            return this._AIlooker;
        }
    }
    public InGameAI.AIDecider Decider
    {
        get
        {
            if (this._AIdecider == null)
            {
                this._AIdecider = new InGameAI.AIDecider(this);
            }

            return this._AIdecider;
        }
    }
    public InGameAI.AIExecutor Executor
    {
        get
        {
            if (this._AIexecutor == null)
            {
                this._AIexecutor = new InGameAI.AIExecutor(this);
            }

            return this._AIexecutor;
        }
    }
    #endregion Getter

    #region Init Action
    public override InGameBasePlayerItem InitPlayerItSelf()
    {
        //init my AI decider and executer
        _AIexecutor = new InGameAI.AIExecutor(this);
        _AIlooker = new InGameAI.AILooker(this);
        _AIdecider = new InGameAI.AIDecider(this);

        return base.InitPlayerItSelf();
    }

    #endregion InitAction

    #region Turn Action
    public override void BeginTurn()
    {
        base.BeginTurn();

        StartCoroutine(OnWaitingTurn());

        IEnumerator OnWaitingTurn()
        {
            yield return new WaitForSeconds(0.5f);
            //BaseUIController.GlobalMask = true;
            InGameAI.LookingMessage collect = _AIlooker?.Look();
            yield return new WaitForEndOfFrame();
            InGameAI.DecidingMesssage decideMsg = Decider?.Decide(collect);
            yield return new WaitForEndOfFrame();
            Executor.SetDecision(decideMsg);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion Turn Action

    public override void CustomUpdate()
    {
        base.CustomUpdate();
        if (Executor?.IsHasAction() ?? false)
        {
            Executor?.Execute();
        }
    }
}
