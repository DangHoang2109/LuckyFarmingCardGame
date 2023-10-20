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

    protected Coroutine _ieThinkingPlan;
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
        StartPlaningTurn();
    }
    public override void ContinueTurn()
    {
        base.ContinueTurn();
        StartPlaningTurn();
    }
    void StartPlaningTurn()
    {
        _ieThinkingPlan = StartCoroutine(OnThinkingInTurn());
    }
    IEnumerator OnThinkingInTurn()
    {
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Bot player: MAKE ME ATTACKKKKKKK");

        //InGameAI.LookingMessage collect = Looker?.Look();
        //yield return new WaitForEndOfFrame();
        InGameAI.DecidingMesssage decideMsg = Decider?.Decide(null);
        yield return new WaitForEndOfFrame();


        Executor.SetDecision(decideMsg);
        yield return new WaitForEndOfFrame();
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }
    public override void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = 1; //replace with this host info
        Debug.Log("ENEMY: FUCK THE MAIN" + dmg);
        base.AttackSingleUnit(dmg);
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
