using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class InGameTurnNotification : MonoBehaviour
{
    #region Property in Inspector
    [SerializeField]
    protected float _yPostionForPlayer, _yPostionForBot;
    [SerializeField]
    protected TextMeshProUGUI _tmpText;
    [SerializeField]
    protected CanvasGroup _canvas;
    #endregion Property in Inspector

    public void ShowText(string content, bool mainPlayerTurn, float timeStay = -1)
    {
        this._tmpText.SetText(content);
        //this.transform.localPosition = new Vector3(this.transform.localPosition.x, mainPlayerTurn ? _yPostionForPlayer : _yPostionForBot);
        this._canvas.alpha = 0;
        Sequence seq = DOTween.Sequence();
        seq.SetId(this.GetInstanceID());
        seq.Join(this._canvas.DOFade(1f, 0.25f));
        if(timeStay > 0)
        {
            seq.AppendInterval(timeStay);
            seq.OnComplete(() => this.DisableText());
        }
        
    }
    public void DisableText()
    {
        this._canvas.DOFade(0f, 0.25f)
            .SetId(this.GetInstanceID());
    }
}
