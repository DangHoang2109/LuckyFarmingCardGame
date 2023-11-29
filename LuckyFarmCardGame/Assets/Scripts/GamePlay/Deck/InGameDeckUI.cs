using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InGameDeckUI : MonoBehaviour
{
    public List<Button> _btns;
    public TextMeshProUGUI _tmpDeckAmount;
    public Image _imgDeckIcon;

    public InGameDeckUIAnimator _animDeck;

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
        _onClickDraw?.Invoke();
    }
    public void PlayAnimationShuffleDeck(System.Action cb = null)
    {
        cb += OnShuffleConplete;

        SetInterractable(false);
        _animDeck.gameObject.SetActive(true);
        _imgDeckIcon.gameObject.SetActive(false);
        _animDeck.PlayAnimationShuffleDeck(cb);
    }
    private void OnShuffleConplete()
    {
        SetInterractable(true);
        _imgDeckIcon.gameObject.SetActive(true);
    }

}
