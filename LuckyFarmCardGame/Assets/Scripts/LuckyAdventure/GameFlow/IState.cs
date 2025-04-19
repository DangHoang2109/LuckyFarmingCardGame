using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IState
{
    public virtual void Enter()
    {
        // code that runs when we first enter the state
        Debug.Log($"Enter {this.GetType()}");
    }

    public virtual void Execute()
    {
        // per-frame logic, include condition to transition to a new state
        Debug.Log($"Execute {this.GetType()}");
    }

    public virtual void Exit()
    {
        // code that runs when we exit the state
        Debug.Log($"Exit {this.GetType()}");
    }


}
