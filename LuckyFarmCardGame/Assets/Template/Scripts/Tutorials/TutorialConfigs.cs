using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(menuName = "Configs/TutorialConfigs", fileName = "TutorialConfigs")]
public class TutorialConfigs : ScriptableObject
{
    [ValueDropdown("CompleteStepViews", ExpandAllMenuItems = true)]
    public int completeStep;
    
    [TableList]
    public List<TutorialConfig> tutorials;
    private static TutorialConfigs _instance;

    public static TutorialConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<TutorialConfigs>("Configs/TutorialConfigs");
            }

            return _instance;
        }
    }

    public TutorialConfig GetTutorialByStep(int step)
    {
        return this.tutorials.Find(x => x.step == step);
    }

    public bool TryGetTutorialConfig(int step, out TutorialConfig config)
    {
        config = this.GetTutorialByStep(step);
        return config != null;
    }
    #if UNITY_EDITOR
    private IEnumerable CompleteStepViews()
    {
        return this.tutorials.Select(x => new ValueDropdownItem($"{x.step} - {x.title}", x.step));
    }
    #endif
}
[System.Serializable]
public class TutorialConfig
{
    [TableColumnWidth(100, Resizable = false)]
    public int step;
    public string title;
    [TextArea]
    public string message;
    [ValueDropdown("ListNextStepViews", ExpandAllMenuItems = true)]
    public int nextStep = -1;

    /// <summary>
    /// Làm đen nền + highlight sáng 1 vùng
    /// False: Ko đen nền, tap anywhere sẽ run next step
    /// </summary>
    public bool _isShowHighLight;
    #if UNITY_EDITOR
    private IEnumerable ListNextStepViews()
    {
        var temp = Resources.Load<TutorialConfigs>("Configs/TutorialConfigs").tutorials
            .Select(x => new ValueDropdownItem($"{x.step} - {x.title}", x.step)).ToList();
        temp.Insert(0, new ValueDropdownItem("None", -1));
        return temp;
    }
    #endif
}
