using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXAttackedEnemyActionManager : BaseDoActionManager
{
    public static VFXAttackedEnemyActionManager Instance => CardGameActionController.Instance.vfxEnemyAttacked;
    public void ShowVFxXAttacked(InGameBotPlayerItem enemy)
    {
        //DoShowEnemyAttackedVFX act = new DoShowEnemyAttackedVFX(enemy);
        //this.AddAction(act);
        //this.RunningAction();
    }
}
//public class DoShowEnemyAttackedVFX : IDoAction
//{
//    private InGameBotPlayerItem _enemy;
//    public DoShowEnemyAttackedVFX() : base()
//    {
//    }
//    public DoShowEnemyAttackedVFX(int id) : base(id)
//    { }
//    public DoShowEnemyAttackedVFX(InGameBotPlayerItem e ) : base()
//    {
//        this._enemy = e;
//    }
//    public override IEnumerator DoAction()
//    {
//        yield return new WaitUntil(() => this._enemy.IsInIdleAnimState);
//    }
//}