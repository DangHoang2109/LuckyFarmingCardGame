using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutorialSpeechPopup : MonoBehaviour
{
    public TextMeshProUGUI _tmpText;
    public RectTransform _panelContent;
    public UnityEngine.UI.Image _blocker;
    [SerializeField]
    private int step;
    private string tutText;

    [SerializeField] private float _MaxTimer;
    public void SetText(int step,string text)
    {
        this.step = step;
        tutText = text;
        this._tmpText.text = $"{text}";
        this._panelContent.sizeDelta = new Vector2(_tmpText.preferredWidth + 0, _tmpText.preferredHeight + 0);
    }
    public void SetIsNeedClick(bool _isClickToNextStep)
    {
        this._isClickToNextStep = _isClickToNextStep;
    }
    public void SetTimer(bool isTimer)
    {
        _isTimer = isTimer;
        _waitingTimer = _MaxTimer;
    }

    public bool _isClickToNextStep; // true for tut not highlight on any button type
    private bool _isTimer;
    private float _waitingTimer;
    private void Update()
    {
        if(_isTimer && _waitingTimer > 0)
        {
            _waitingTimer -= Time.deltaTime;
            if(_waitingTimer <= 0)
            {
                TutorialManager.Instance.DoTutorial(this.step);
            }
            this._tmpText.text = $"{tutText} ({(int)_waitingTimer})";
        }
        if (!_isClickToNextStep)
            return;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            TutorialManager.Instance.DoTutorial(this.step);
        }
#else
        if (Input.touchCount > 0)
        {
            TutorialManager.Instance.DoTutorial(this.step);
        }
#endif
    }
    public void OnClose()
    {
        this.gameObject.SetActive(false);
    }
    public void OnShow(bool isBlockBehin)
    {
        _blocker.raycastTarget = isBlockBehin;
        this.gameObject.SetActive(true);
    }
    //public static TutorialSpeechPopup ShowDialog(string text)
    //{
    //    var TutorialSpeechPopup = GameManager.Instance.OnShowDialog<TutorialSpeechPopup>("Dialogs/TutorialSpeechPopup");
    //    //TutorialSpeechPopup.SetText(text); 
    //    TutorialSpeechPopup.SetIsNeedClick(false);
    //    return TutorialSpeechPopup;
    //}
}
