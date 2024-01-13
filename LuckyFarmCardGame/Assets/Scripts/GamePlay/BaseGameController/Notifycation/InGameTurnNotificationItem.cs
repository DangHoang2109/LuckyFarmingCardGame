using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameTurnNotificationItem : MonoBehaviour
{
    public bool isInUse = false;
    #region Property in Inspector
    [SerializeField]
    protected TextMeshProUGUI _tmpText;
    [SerializeField]
    protected CanvasGroup _canvas;
    #endregion Property in Inspector

    public InGameTurnNotificationItem SetUp()
    {
        this.isInUse = false;
        this._tmpText.text = "";
        this._canvas.alpha = 0;
        this.transform.localPosition = Vector3.zero;
        return this;
    }
    public Sequence ShowText(string content, bool mainPlayerTurn, Vector3 playerShowPos, float delay, float timeStay = 0.5f, System.Action<InGameTurnNotificationItem> onComplete = null)
    {
        isInUse = true;
        DOTween.Kill(this.GetInstanceID());
        this._tmpText.SetText(content);

        this.transform.position = playerShowPos;
        this.transform.localPosition += new Vector3(Random.Range(-77, 67), Random.Range(-50f, 50f), 0);

        //this.transform.localPosition = new Vector3(this.transform.localPosition.x, mainPlayerTurn ? _yPostionForPlayer : _yPostionForBot);
        this._canvas.alpha = 0;
        Sequence seq = DOTween.Sequence();
        seq.SetId(this.GetInstanceID());
        seq.SetDelay(delay);
        seq.Join(this._canvas.DOFade(1f, 0.25f));
        seq.Join(this.transform.DOPunchScale(new Vector3(1.1f, 1.1f), 0.25f));
        

        if (timeStay > 0)
        {
            seq.AppendInterval(timeStay);
            seq.OnComplete(() => OnComplete(onComplete));
        }
        return seq;
    }
    void OnComplete(System.Action<InGameTurnNotificationItem> cb)
    {
        DisableText();
        cb?.Invoke(this);
    }
    public void DisableText()
    {
        this._canvas.DOFade(0f, 0.25f)
            .SetId(this.GetInstanceID());
        isInUse = false;
    }
}
