using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoActionManager : MonoSingleton<DoActionManager>
{
    protected Queue<IDoAction> doSomething = new Queue<IDoAction>();
    protected Coroutine coRunining;

    public virtual void AddAction(IDoAction action)
    {
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

    public  virtual void RunningInstanceAction()
    {
        if (this.coRunining == null)
        {
            this.coRunining = StartCoroutine(this.RunAction());
        }
    }
    protected virtual IEnumerator RunAction()
    {
        while (this.doSomething.Count > 0)
        {
            yield return this.doSomething.Dequeue().DoAction();
        }

        this.coRunining = null;
    }

    public virtual void AddActionFirst(IDoAction action)
    {
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
}
