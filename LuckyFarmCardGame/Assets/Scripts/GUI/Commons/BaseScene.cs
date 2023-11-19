using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseScene : MonoBehaviour
{
    [SerializeField]
    protected CanvasGroup _canvasGroup;

    public CanvasGroup canvasGroup
    {
        get
        {
            if (this._canvasGroup == null)
            {
                this._canvasGroup = this.GetComponent<CanvasGroup>();
            }

            return this._canvasGroup;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        this._canvasGroup = this.GetComponent<CanvasGroup>();
    }
#endif
    public virtual IEnumerator PreLoadScene()
    {
        yield return new WaitForEndOfFrame();
    }

    public virtual void InitScene()
    {
        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0;
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.interactable = false;
        }
    }
    public virtual void StartScene(System.Action callback = null)
    {
        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 1;
            this.canvasGroup.blocksRaycasts = true;
            this.canvasGroup.interactable = true;
        }
        callback?.Invoke();
    }

    public virtual void UnloadScene()
    {
        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0;
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.interactable = false;
        }
    }
}
