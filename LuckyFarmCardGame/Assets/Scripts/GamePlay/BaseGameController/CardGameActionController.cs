using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Quản lý toàn bộ action manager trong game
/// Mỗi 1 con là một singleton thì ko được vì kế thừa
/// </summary>
public class CardGameActionController : MonoSingleton<CardGameActionController>
{
    public VFXActionManager vfxActionManager;
    public CollectUpgradeActionManager collectUpgradeActionManager;

    public bool IsVFXRunning => vfxActionManager?.IsRunning ?? false;
    public bool IsUpgradeCollectorRunning => collectUpgradeActionManager?.IsRunning ?? false;

    public bool IsAnyRunning => IsVFXRunning || IsUpgradeCollectorRunning;

    public void AddCallbackWhenFXComplete(System.Action cb)
    {
        DoNoFXRunning act = new DoNoFXRunning(this, cb);
        AddActionAndRun(act);
    }
    #region Outside Game Manager can add action to control the flow of game
    /// <summary>
    /// Example: Dont get new turn if any vfx or upgrade collector still runing
    /// </summary>
    protected Queue<IDoAction> doSomething = new Queue<IDoAction>();
    protected Coroutine coRunining;
    public virtual void AddAction(IDoAction action)
    {
        doSomething ??= new Queue<IDoAction>();
        this.doSomething.Enqueue(action);
    }
    public virtual void AddActionAndRun(IDoAction action)
    {
        AddAction(action);
        RunningAction();
    }
    public virtual void RunningAction()
    {
        if (this.coRunining == null)
        {
            this.coRunining = StartCoroutine(this.RunAction());
        }
    }
    protected virtual IEnumerator RunAction()
    {
        doSomething ??= new Queue<IDoAction>();

        while (this.doSomething.Count > 0)
        {
            yield return this.doSomething.Dequeue().DoAction();
        }

        this.coRunining = null;
    }
    public virtual void AddActionFirst(IDoAction action)
    {
        doSomething ??= new Queue<IDoAction>();
        Queue<IDoAction> tempAtionsQueue = new Queue<IDoAction>();
        while (this.doSomething.Count > 0)
        {
            tempAtionsQueue.Enqueue(this.doSomething.Dequeue());
        }
        this.doSomething.Enqueue(action);
        while (tempAtionsQueue.Count > 0)
        {
            this.doSomething.Enqueue(tempAtionsQueue.Dequeue());
        }
    }
    #endregion Outside Game Manager can add action to control the flow of game
}
public class DoNoFXRunning : IDoAction
{
    CardGameActionController _controller;
    System.Action _cb;
    public DoNoFXRunning() : base()
    {
    }
    public DoNoFXRunning(int id) : base(id)
    { }
    public DoNoFXRunning(CardGameActionController controller, System.Action cb) : base()
    {
        _controller = controller;
        this._cb = cb;
    }
    public override IEnumerator DoAction()
    {
        yield return new WaitUntil(() => !_controller.IsAnyRunning);
        Debug.Log("No more wuning " + _controller.IsAnyRunning);
        _cb?.Invoke();
    }
}