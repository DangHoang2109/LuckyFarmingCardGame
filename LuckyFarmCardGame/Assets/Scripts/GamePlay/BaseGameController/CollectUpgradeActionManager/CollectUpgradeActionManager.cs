using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectUpgradeActionManager : DoActionManager
{
}
public class DoCollectUpgrade : IDoAction
{
    private InGameBagCardProgressAnim _anim;

    public DoCollectUpgrade() : base()
    {
    }
    public DoCollectUpgrade(int id) : base(id)
    { }
    public DoCollectUpgrade(InGameBagCardProgressAnim anim) : base()
    { 
        this._anim = anim;
    }
    public override IEnumerator DoAction()
    {
        if(_anim != null)
        {
            _anim.DoAnim();
        }

        yield return new WaitUntil(() => _anim._readyForNext);
    }
}