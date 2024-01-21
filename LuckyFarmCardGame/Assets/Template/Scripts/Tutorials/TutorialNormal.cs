using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNormal : MonoBehaviour
{
    [SerializeField]
    public int step;

    private bool IsCompeteTutorial
    {
        get
        {
            return TutorialManager.Instance.IsCompleteTutorial();
        }
    }
    private void OnEnable()
    {
        TutorialManager.tutorialCallback += this.OnTutorialCallback;
        //Cosina.DataManagement.SaveManager.Instance.RegisterCallbackOnDataLoaded(() =>
        //{
        //    this.CheckCompleteTutorial();
        //});
    }
//    private void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetMouseButtonDown(0))
//        {
//            TutorialManager.Instance.DoTutorial(this.step);
//        }
//#else
//        if (Input.touchCount > 0)
//        {
//            TutorialManager.Instance.DoTutorial(this.step);
//        }
//#endif
//    }
    private void OnTutorialCallback(int step)
    {

    }
    private void CheckCompleteTutorial()
    {
        if (!this.IsCompeteTutorial)
        {
            return;
        }
    }
    public void ShowTutorial(bool isShow = true)
    {
    }
}
