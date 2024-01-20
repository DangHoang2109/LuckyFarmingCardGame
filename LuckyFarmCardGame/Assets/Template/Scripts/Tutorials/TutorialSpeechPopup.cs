using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutorialSpeechPopup : MonoBehaviour
{
    public TextMeshProUGUI _tmpText;
    public RectTransform _panelContent;
    [SerializeField]
    private int step;
    public void SetText(int step,string text)
    {
        this.step = step;

        this._tmpText.text = $"{text}<br>Tap to continue";
        this._panelContent.sizeDelta = new Vector2(_tmpText.preferredWidth + 0, _tmpText.preferredHeight + 0);
    }
    public void SetIsNeedClick(bool _isClickToNextStep)
    {
        this._isClickToNextStep = _isClickToNextStep;
    }

    public bool _isClickToNextStep; // true for tut not highlight on any button type
    private void Update()
    {
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
    public void OnShow()
    {
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
