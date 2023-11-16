using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IDoAction
{
    protected int id;
    public IDoAction()
    { }

    public IDoAction(int _id)
    {
        this.id = _id;
    }
    public virtual IEnumerator DoAction()
    {
        yield return null;
    }
}