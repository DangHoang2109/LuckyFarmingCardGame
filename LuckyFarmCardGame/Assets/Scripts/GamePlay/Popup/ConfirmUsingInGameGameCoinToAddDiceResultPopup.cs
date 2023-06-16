using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ConfirmUsingInGameGameCoinToAddDiceResultPopup : MonoBehaviour
{
    protected int _amountCoinNeedUsing;

    public TextMeshProUGUI _tmpAmountCoinUse, _tmpAmountDiceResAdding;
    [Space(5f)]
    public Button[] _btns;
    [Space(5f)]
    [SerializeField] protected float _timeAnimShow;
    [SerializeField] protected float _yPosHiding;

    protected System.Action<int> _onConfirmUsingOrNot;
    public ConfirmUsingInGameGameCoinToAddDiceResultPopup Init(System.Action<int> cb)
    {
        this._onConfirmUsingOrNot -= cb;
        this._onConfirmUsingOrNot += cb;

        this.gameObject.SetActive(false);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this._yPosHiding);

        return this;
    }
    public ConfirmUsingInGameGameCoinToAddDiceResultPopup ParseDataAndShow(int amountCoinNeedToUse, int amountDiceResWillBeAdd)
    {
        this._amountCoinNeedUsing = amountCoinNeedToUse;

        this._tmpAmountCoinUse.SetText($"-{amountCoinNeedToUse}");
        this._tmpAmountDiceResAdding.SetText($"+{amountDiceResWillBeAdd}");

        this.gameObject.SetActive(true);
        this.transform.DOLocalMoveY(0, _timeAnimShow)
            .OnComplete(()=> EnableButton(true));

        return this;
    }
    void Hiding()
    {
        EnableButton(false);
        this.transform.DOLocalMoveY(_yPosHiding, _timeAnimShow)
            .OnComplete(() => this.gameObject.SetActive(false));
    }
    void EnableButton(bool isOn)
    {
        foreach (Button item in _btns)
        {
            item.interactable = isOn;
        }
    }

    public void OnClickConfirm() 
    {
        _onConfirmUsingOrNot?.Invoke(_amountCoinNeedUsing);
        Hiding();
    }
    public void OnClickReject()
    {
        _onConfirmUsingOrNot?.Invoke(-1);
        Hiding();
    }
}
