using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTapAnywhere : MonoBehaviour
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
    public void Click()
    {
        TutorialManager.Instance.DoTutorial(this.step);
    }
}
