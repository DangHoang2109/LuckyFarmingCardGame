using System.Collections.Generic;
using UnityEngine;

namespace Cosina.Components
{
    /// <summary>
/// handle a list of invoker and a list of repeated invoker
/// </summary>
public class Invoker : MonoSingleton<Invoker>
{
    private List<System.Action> calls;
    private List<System.Action> callsToAdd;
    private List<System.Action> callsToRemove;
    private Dictionary<System.Action, float> callTimers;

    private List<System.Action> callRepeats;
    private List<System.Action> callRepeatsToAdd;
    private List<System.Action> callRepeatsToRemove;
    private Dictionary<System.Action, float> callRepeatTimers;
    private Dictionary<System.Action, float> callRepeatTimes;

    private static object _lockCallAdd = new object();
    private static object _lockCallRemove = new object();
    private static object _lockCallRepeatAdd = new object();
    private static object _lockCallRepeatRemove = new object();

    public override void Init()
    {
        this.calls = new List<System.Action>();
        this.callsToAdd = new List<System.Action>();
        this.callsToRemove = new List<System.Action>();
        this.callTimers = new Dictionary<System.Action, float>();

        this.callRepeats = new List<System.Action>();
        this.callRepeatsToAdd = new List<System.Action>();
        this.callRepeatsToRemove = new List<System.Action>();
        this.callRepeatTimers = new Dictionary<System.Action, float>();
        this.callRepeatTimes = new Dictionary<System.Action, float>();
    }

    /// <summary>
    /// invoke a function after a while
    /// </summary>
    public static void Invoke(System.Action call, float delay = 0f)
    {
        lock (_lockCallAdd)
        {
            Invoker instance = Invoker.Instance;
            instance.callsToAdd.Add(call);
            instance.callTimers[call] = delay;
        }
    }

    /// <summary>
    /// Cancel the ONE-TIME invoking function
    /// <para>DO NOT PUT in LAMBDA</para>
    /// </summary>
    public static void CancelInvoke(System.Action call)
    {
        lock (_lockCallRemove)
        {
            Invoker instance = Invoker.Instance;
            instance.callsToRemove.Add(call);
        }
    }

    /// <summary>
    /// invoke a function after a delay, then repeated after interval
    /// <para>BE CAREFUL WHEN PUT IN LAMBDA</para>
    /// <para>you may not be able to cancel a lambda callback</para>
    /// </summary>
    public static void InvokeRepeat(System.Action call, float startDelay = 0f, float interval = 0f)
    {
        lock (_lockCallRepeatAdd)
        {
            Invoker instance = Invoker.Instance;
            instance.callRepeatsToAdd.Add(call);
            instance.callRepeatTimers[call] = startDelay;
            instance.callRepeatTimes[call] = interval;
        }
    }

    /// <summary>
    /// Cancel the REPEATED-time invoking function
    /// <para>DO NOT PUT in LAMBDA</para>
    /// </summary>
    public static void CancelInvokeRepeat(System.Action call)
    {
        lock (_lockCallRepeatRemove)
        {
            Invoker instance = Invoker.Instance;
            instance.callRepeatsToRemove.Add(call);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        this.CheckInvokeOne();
        this.InvokeOnce(dt);
        this.CheckInvokeRepeat();
        this.InvokeRepeat(dt);
    }

    private void CheckInvokeOne()
    {
        lock (_lockCallAdd)
        {
            for (int i = this.callsToAdd.Count - 1; i >= 0; --i)
            {
                if (this.calls.Contains(this.callsToAdd[i]))
                    continue;

                this.calls.Add(this.callsToAdd[i]);
            }
            this.callsToAdd.Clear();
        }

        lock (_lockCallRemove)
        {
            for (int i = this.callsToRemove.Count - 1; i >= 0; --i)
            {
                if (this.calls.Contains(this.callsToRemove[i]))
                {
                    this.calls.Remove(this.callsToRemove[i]);
                }
            }
            this.callsToRemove.Clear();
        }
    }

    private void InvokeOnce(float dt)
    {
        for (int i = this.calls.Count - 1; i >= 0; --i)
        {
            if ((this.callTimers[this.calls[i]] -= dt) <= 0)
            {
                try
                {
                    this.calls[i]?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Invoker catch an error");
                    Debug.LogError(e.ToString());
                }

                this.callsToRemove.Add(this.calls[i]);

            }
        }
    }


    private void CheckInvokeRepeat()
    {
        lock (_lockCallRepeatAdd)
        {
            for (int i = this.callRepeatsToAdd.Count - 1; i >= 0; --i)
            {
                if (this.callRepeats.Contains(this.callRepeatsToAdd[i]))
                    continue;

                this.callRepeats.Add(this.callRepeatsToAdd[i]);
            }
            this.callRepeatsToAdd.Clear();
        }

        lock (_lockCallRepeatRemove)
        {
            for (int i = this.callRepeatsToRemove.Count - 1; i >= 0; --i)
            {
                if (this.callRepeats.Contains(this.callRepeatsToRemove[i]))
                {
                    this.callRepeats.Remove(this.callRepeatsToRemove[i]);
                }
            }
            this.callRepeatsToRemove.Clear();
        }
    }

    private void InvokeRepeat(float dt)
    {
        for (int i = this.callRepeats.Count - 1; i >= 0; --i)
        {
            if ((this.callRepeatTimers[this.callRepeats[i]] -= dt) <= 0)
            {
                try
                {
                    this.callRepeats[i]?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Invoker catch an error");
                    Debug.LogError(e.ToString());
                }
                this.callRepeatTimers[this.callRepeats[i]] = this.callRepeatTimes[this.callRepeats[i]];
            }
        }
    }
}

}
