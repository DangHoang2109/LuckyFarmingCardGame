using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXActionManager : DoActionManager
{
    public static VFXActionManager Instance => CardGameActionController.Instance.vfxActionManager;
    public void ShowVFxXAttributeChange(AttributeUI item)
    {
        DoShowAttributeChangeVFX act = new DoShowAttributeChangeVFX(item);
        this.AddAction(act);
        this.RunningAction();
    }
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
    public void ShowMultiVFxXByCard(int vfxId, int amount, List<Transform> desPoss, float delay, System.Action<VFXBaseObject> cbOnFirst)
    {
        List<VFXProjectile> vfxObjs = VFXManager.Instance.GetObjects<VFXProjectile>(vfxId, amount);
        if (vfxObjs != null && vfxObjs.Count > 0)
        {
            DoShowCardMultipleVFX act = new DoShowCardMultipleVFX(vfxObjs, desPoss, delay, cbOnFirst);
            this.AddAction(act);
        }
        this.RunningAction();
    }
    public void ShowFromToVFxXByCard(int vfxId, int amount, Transform startPos, Transform desPos, float delay, System.Action<VFXBaseObject> cb)
    {
        List<VFXProjectile> vfxObjs = VFXManager.Instance.GetObjects<VFXProjectile>(vfxId, amount);
        if (vfxObjs != null && vfxObjs.Count > 0)
        {
            DoShowCardFromToVFX act = new DoShowCardFromToVFX(vfxObjs[0], startPos, desPos, delay, cb);
            this.AddAction(act);
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
            _vfxObj.DoAnimation(_desPos: this._desPos, delay: this._delay);
        }

        yield return new WaitUntil(() => _vfxObj.isReadyForNext);
    }
}

public class DoShowCardFromToVFX : IDoAction
{
    private VFXProjectile _vfxObj;
    private Transform _startPos, _desPos;
    public float _delay;
    public System.Action<VFXBaseObject> cb;
    public DoShowCardFromToVFX() : base()
    {
    }
    public DoShowCardFromToVFX(int id) : base(id)
    { }
    public DoShowCardFromToVFX(VFXProjectile vfxObj, Transform startPos, Transform desPos, float delay, System.Action<VFXBaseObject> cb) : base()
    {
        this._vfxObj = vfxObj;

        if (_vfxObj != null)
            _vfxObj.gameObject.SetActive(false);

        this._startPos = startPos;
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
            _vfxObj.DoAnimationFromStartPoint(_startPos,_desPos: this._desPos, delay: this._delay);
        }
        yield return new WaitUntil(() => _vfxObj.isReadyForNext);
    }
}

public class DoShowCardMultipleVFX : IDoAction
{
    private List<VFXProjectile> _vfxObj;
    private List<Transform> _desPos;
    public float _delay;
    public System.Action<VFXBaseObject> cbOnFirst;
    public DoShowCardMultipleVFX() : base()
    {
    }
    public DoShowCardMultipleVFX(int id) : base(id)
    { }
    public DoShowCardMultipleVFX(List<VFXProjectile> vfxObj, List<Transform> desPos, float delay, System.Action<VFXBaseObject> cb) : base()
    {
        this._vfxObj = vfxObj;

        if (_vfxObj != null)
            _vfxObj.ForEach(x => x.gameObject.SetActive(false));

        this._desPos = desPos;
        this._delay = delay;
        this.cbOnFirst = cb;
    }
    public override IEnumerator DoAction()
    {
        if (_vfxObj != null)
        {
            for (int i = 0; i < _vfxObj.Count; i++)
            {
                VFXProjectile item = _vfxObj[i];

                item.transform.localPosition = Vector3.zero;
                item.gameObject.SetActive(true);
                if(i == 0)
                    item.AppendAnimation(this.cbOnFirst);
                item.DoAnimation(_desPos: this._desPos[i], delay: this._delay);
            }
        }
        yield return new WaitUntil(() => AllReady());
    }
    private bool AllReady() => _vfxObj.TrueForAll(x => x.isReadyForNext);
}

public class DoShowAttributeChangeVFX : IDoAction
{
    private AttributeUI _vfxObj;
    public DoShowAttributeChangeVFX() : base()
    {
    }
    public DoShowAttributeChangeVFX(int id) : base(id)
    { }
    public DoShowAttributeChangeVFX(AttributeUI vfxObj) : base()
    {
        this._vfxObj = vfxObj;
    }
    public override IEnumerator DoAction()
    {
        yield return new WaitUntil(() => _vfxObj.IsReadyForNext);
    }
}