using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InGameBotPlayerItem : InGameBasePlayerItem
{
    #region Prop on editor
    public BaseInGameEnemyDataModel EnemyModel => (this.PlayerModel as BaseInGameEnemyDataModel);
    public int DamagePerTurn => EnemyModel?.DamagePerTurn ?? 0;


    public Transform _tfPosition;
    protected CSkeletonAnimator _animator;
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
    public InGameEnemyStatConfig StatConfig => this.EnemyModel?._statConfig;
    public InGameEnemyInfoConfig InfoConfig => this.StatConfig?.Info;
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
    public override InGameBasePlayerItem SetAPlayerModel(BaseInGamePlayerDataModel model)
    {
        base.SetAPlayerModel(model);

        //set avatar
        SetUI();
        InitPlayerItSelf();
        StartCoroutine(ShowUp());
        return this;
    }
    protected virtual void SetUI()
    {
        this._animator = Instantiate(this.StatConfig?.Info?._animator, this._tfPosition);
    }
    private IEnumerator ShowUp()
    {
        Debug.Log("Yo?");
        yield return new WaitForEndOfFrame();
        this._animator?.ShowAppear();
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
    public override void Attacked(int dmg)
    {
        base.Attacked(dmg);
        this._animator?.ShowAttacked();
    }
    public override void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = this.DamagePerTurn; //replace with this host info
        Debug.Log("ENEMY: FUCK THE MAIN" + dmg);

        InGameManager.Instance.OnPlayerAttacking(InGameManager.Instance.MainUserPlayer.ID, dmg);
        this._animator?.ShowAttack();
        base.AttackSingleUnit(dmg);
    }
    public override void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = this.DamagePerTurn; //replace with this host info
        Debug.Log("ENEMY: FUCK THE MAIN" + dmg);
        this._animator?.ShowAttack();
        InGameManager.Instance.OnPlayerAttackingAllUnit(isEnemySide: false, dmg);
        base.AttackAllUnit(dmg);
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
