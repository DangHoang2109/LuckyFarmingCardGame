using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quan3 ly1 action toan app nhu template
/// neu ban chi can cac action trong game, su dung CardActionManager
/// </summary>
public class DoActionManager : BaseDoActionManager
{
    private static DoActionManager _instance;
    public static DoActionManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);
    }
}
public class DoTutorialAction : IDoAction
{
    public DoTutorialAction() : base()
    {
    }
    public DoTutorialAction(int id) : base(id)
    { }
    public override IEnumerator DoAction()
    {
        TutorialManager.Instance.BeginTutorial(this.id);
        yield return new WaitUntil(() => TutorialManager.Instance.NextStepTutorial == -1);
    }
}
