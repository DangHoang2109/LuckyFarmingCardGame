using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InGameBotPlayerItem : InGameBasePlayerItem
{
    #region Prop on editor
    public BaseInGameEnemyDataModel EnemyModel => (this.PlayerModel as BaseInGameEnemyDataModel);

    public Transform _tfPosition;
    protected CSkeletonAnimator _animator;
    #endregion Prop on editor

    #region Data

    protected InGameAI.AIExecutor _AIexecutor;
    protected InGameAI.AILooker _AIlooker;

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
        if(this._animator != null)
        {
            Destroy(this._animator.gameObject);
        }
        this._animator = Instantiate(this.StatConfig?.Info?._animator, this._tfPosition);
    }
    private IEnumerator ShowUp()
    {
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

        InGameAI.LookingMessage collect = Looker?.Look();

        yield return new WaitForEndOfFrame();
        Executor.Decide(collect);
        yield return new WaitForEndOfFrame();
        Executor.SetDecision();
        yield return new WaitForEndOfFrame();
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }
    public override void Attacked(int dmg, System.Action<InGameBasePlayerItem> deaded = null)
    {
        base.Attacked(dmg);
        this._animator?.ShowAttacked(isDead(), OnDeadComplete);

        void OnDeadComplete()
        {
            deaded?.Invoke(this);
        }
    }
    public override void AttackSingleUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = BaseDamagePerTurn; //replace with this host info
        Debug.Log("ENEMY: FUCK THE MAIN" + dmg);

        InGameManager.Instance.OnPlayerAttacking(InGameManager.Instance.MainUserPlayer.SeatID, dmg);
        this._animator?.ShowAttack();
        base.AttackSingleUnit(dmg);
    }
    public override void AttackAllUnit(int dmg = -1)
    {
        if (dmg <= 0)
            dmg = this.BaseDamagePerTurn; //replace with this host info
        Debug.Log("ENEMY: FUCK THE MAIN" + dmg);
        this._animator?.ShowAttack();
        InGameManager.Instance.OnPlayerAttackingAllUnit(isEnemySide: false, dmg);
        base.AttackAllUnit(dmg);
    }
    public override void Dead(Action cb)
    {
        base.Dead(cb);

        this._AIexecutor = null;
        this._AIlooker = null;
        //this._animator?.ShowDead(cb);
    }
    public override void ClearWhenDead()
    {
        base.ClearWhenDead();
        Destroy(this._animator.gameObject);
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
