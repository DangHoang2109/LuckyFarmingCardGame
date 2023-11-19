using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InGameDeckUI : MonoBehaviour
{
    public List<Button> _btns;
    public TextMeshProUGUI _tmpDeckAmount;

    public System.Action _onClickDraw;
    public void OnChangeCardAmount(int cardAmount)
    {
        if(_tmpDeckAmount != null)
            this._tmpDeckAmount.text = cardAmount.ToString();
    }

    public void SetInterractable(bool isCanClick)
    {
        if(_btns != null && _btns.Count > 0)
            _btns.ForEach(x => x.interactable = isCanClick);
    }
    public void OnClickDeck()
    {
        Debug.LogError("DRAW");
        _onClickDraw?.Invoke();
    }
}
