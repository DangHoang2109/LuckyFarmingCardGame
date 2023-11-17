using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXActionManager : DoActionManager
{
    public static VFXActionManager Instance => CardGameActionController.Instance.vfxActionManager;
    public void ShowVFxXBycard(int vfxId, int amount, Transform desPos, float delay, System.Action<VFXBaseObject> cb)
    {
        List<VFXProjectile> vfxObjs = VFXManager.Instance.GetObjects<VFXProjectile>(vfxId, amount);
        if(vfxObjs != null && vfxObjs.Count > 0)
        {
            foreach (var item in vfxObjs)
            {
                DoShowCardVFX act = new DoShowCardVFX(item, desPos, delay, cb);
                this.AddAction(act);
            }
        }
        this.RunningAction();
    }
}
public class DoShowCardVFX : IDoAction
{
    private VFXProjectile _vfxObj;
    private Transform _desPos;
    public float _delay;
    public System.Action<VFXBaseObject> cb;
    public DoShowCardVFX() : base()
    {
    }
    public DoShowCardVFX(int id) : base(id)
    { }
    public DoShowCardVFX(VFXProjectile vfxObj, Transform desPos, float delay, System.Action<VFXBaseObject> cb) : base()
    {
        this._vfxObj = vfxObj;

        if (_vfxObj != null)
            _vfxObj.gameObject.SetActive(false);

        this._desPos = desPos;
        this._delay = delay;
        this.cb = cb;
    }
    public override IEnumerator DoAction()
    {
        if (_vfxObj != null)
        {
            _vfxObj.transform.localPosition = Vector3.zero;
            _vfxObj.gameObject.SetActive(true);
            _vfxObj.AppendAnimation(this.cb);
            _vfxObj.DoAnimation(_desPos: this._desPos, delay:this._delay);
        }

        yield return new WaitUntil(() => _vfxObj.isReadyForNext);
    }
}