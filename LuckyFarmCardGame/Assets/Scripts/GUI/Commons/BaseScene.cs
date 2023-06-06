using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{

    public RectTransform dialog;
    private object data;
    private void Awake()
    {
        GameManager.Instance.setBaseScene(this);
    }
    protected virtual IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        this.OnParseData();
    }
    public virtual void OnParseData()
    {

    }
    public virtual void OnLoadScene()
    {
    }
    public virtual void OnClear()
    { }
}
