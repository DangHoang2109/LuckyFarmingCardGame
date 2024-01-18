using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InGameAI;
public class InGameBotPlayerItem : InGameBasePlayerItem
{
    #region Prop on editor
    public BaseInGameEnemyDataModel EnemyModel => (this.PlayerModel as BaseInGameEnemyDataModel);

    public Transform _tfPosition;
    protected CSkeletonAnimator _animator;

    private bool isInIdle = true; 
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

        Executor.Decide();

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
    private void SetIdleAnimState(bool state)
    {
        isInIdle = state;
    }
    public bool IsInIdleAnimState() => isInIdle;
    public override void BeginTurn()
    {
        if (!IsPlaying)
            return;

        if (_ieThinkingPlan != null)
            StopCoroutine(_ieThinkingPlan);

        SetIdleAnimState(true);
        bool isStunning = this.IsStunning;
        base.BeginTurn();
        if(!isStunning)
            StartPlaningTurn();
        else
            InGameManager.Instance.OnUserEndTurn();
    }
    public override void ContinueTurn()
    {
        //SetIdleAnimState(true);
        base.ContinueTurn();
    }
    void StartPlaningTurn()
    {
        _ieThinkingPlan = StartCoroutine(OnThinkingInTurn());
    }
    IEnumerator OnThinkingInTurn()
    {
        InGameAI.LookingMessage collect = Looker?.Look();

        yield return new WaitForEndOfFrame();
        Executor.Decide();
        yield return new WaitForEndOfFrame();
        Executor.SetDecision();
        yield return new WaitForEndOfFrame();
        StartCoroutine(ExecuteTurn());
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }
    public override int Attacked(int dmg, System.Action<InGameBasePlayerItem> deaded = null)
    {
        dmg = base.Attacked(dmg);
        if(dmg > 0)
        {
            //VFXAttackedEnemyActionManager.Instance.ShowVFxXAttacked(this);
            this._animator?.ShowAttacked(isDead(), OnDeadComplete);
        }
        return dmg;
        void OnDeadComplete()
        {
            deaded?.Invoke(this);
        }
    }
    public override void AttackSingleUnit(int dmg = -1)
    {
        SetIdleAnimState(false);
        if (dmg <= 0)
            dmg = BaseDamagePerTurn; //replace with this host info
        //this._animator?.ShowAttack();
        this._animator?.ShowAnimationWithCallback(animKey: AnimationState.ATTACK_ANIM, cb: () =>
        {
            InGameManager.Instance.OnPlayerAttacking(InGameManager.Instance.MainUserPlayer.SeatID, dmg);
            SetIdleAnimState(true); 
            base.AttackSingleUnit(dmg);
        });
    }
    public override void AttackAllUnit(int dmg = -1)
    {
        SetIdleAnimState(false);

        if (dmg <= 0)
            dmg = this.BaseDamagePerTurn; //replace with this host info
        //this._animator?.ShowAttack();

        this._animator?.ShowAnimationWithCallback(animKey: AnimationState.ATTACK_ANIM, cb: () =>
        {

            InGameManager.Instance.OnPlayerAttackingAllUnit(isEnemySide: false, dmg);
            SetIdleAnimState(true);
            base.AttackAllUnit(dmg);

        });
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

    }
    IEnumerator ExecuteTurn()
    {
        Queue<ExecutionAction> turnActions = this.Executor.GetAction();

        while (turnActions.Count > 0)
        {
            yield return turnActions.Dequeue().Do();
        }
    }
    public void OnClickViewEffect()
    {
        BaseInfoPopup d = BaseInfoPopup.ShowDialog();
        d.ParseData(title: this.InfoConfig.enemyName,description: this.Executor?.GetSkillDescribe());
        d.SetPosition(new Vector3(0, 337));
    }
}
