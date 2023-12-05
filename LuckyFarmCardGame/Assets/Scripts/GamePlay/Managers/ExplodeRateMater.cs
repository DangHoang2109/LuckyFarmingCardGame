using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ExplodeRateMater : MonoBehaviour
{
    private CardGameController _controller;
    // Start is called before the first frame update
    public Image _imgFillRate;
    public TextMeshProUGUI _tmpRate;

    int tweenID;
    void Start()
    {
        tweenID = this.GetInstanceID();
        _imgFillRate.fillAmount = 0f;
    }
    public void Init(CardGameController contller)
    {
        this._controller = contller;
    }
    public void OnChangeDeckOrPalletInfo(int deckAmount)
    {
        //ignore param, call to controller and get rate
        float explodeRate = _controller.GetPalletConflictChance();
        Debug.Log($"EXPLODE RATE: {explodeRate}");

        float oldRate = _imgFillRate.fillAmount;
        DOTween.Kill(this.tweenID);
        Sequence seq = DOTween.Sequence();
        seq.SetId(this.tweenID);
        seq.Join(this._imgFillRate.DOFillAmount(explodeRate, Mathf.Abs(explodeRate - oldRate))); //thời gian đều
        
    }
}
