using System;
using System.Collections;
using System.Collections.Generic;
//using Cosina.DataManagement;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public const int FIRST_TUTORIAL = 100;
    public const int COMPLETE_TUTORIAL = 100;
    private const string TUTORIAL_KEY = "Tutorial_Key";
    private bool isShow;

    public bool IsShow => isShow;
    
    [SerializeField]
    private TutorialFaceImage panelFace;
    [SerializeField] private TutorialTapAnywhere panelTapAnywhere;

    private Dictionary<int, List<TutorialHighLight>> dicHightLights = new Dictionary<int, List<TutorialHighLight>>();
    public static System.Action<int> tutorialCallback;
    public bool IsCompleteTutorial()
    {
        return this.TutorialCurrentStep >= TutorialConfigs.Instance.completeStep;
    }
    [Obsolete("Use TutorialCurrentStep")]
    private int currentStep = -1;

    private int nextStep = -1;
    public int NextStepTutorial => this.nextStep;
    public int TutorialCurrentStep
    {
        get
        {
            if (this.currentStep == -1)
            {
                return PlayerPrefs.GetInt(TUTORIAL_KEY, FIRST_TUTORIAL);
            }

            return this.currentStep;
        }
        set
        {
            this.currentStep = value;
            //PlayerPrefs.SetInt(TUTORIAL_KEY, this.currentStep);
            //LogGameAnalytics.Instance.LogTutorialStep(currentStep);
        }
    }

    private TutorialConfig config
    {
        get
        {
            return TutorialConfigs.Instance.GetTutorialByStep(this.TutorialCurrentStep);
        }
    }

    private void OnEnable()
    {
        //SaveManager.Instance.RegisterCallbackOnDataLoaded(this.LoadDataDone);
    }

    private void LoadDataDone()
    {
        
    }

    public void RegisterHighlight(int step, TutorialHighLight highLight)
    {
        if (this.dicHightLights.TryGetValue(step, out List<TutorialHighLight> highLights))
        {
            highLights.Add(highLight);
        }
        else
        {
            highLights = new List<TutorialHighLight>();
            highLights.Add(highLight);
            this.dicHightLights.Add(step, highLights);
        }
    }

    public void BeginTutorial(int step)
    {
        this.TutorialCurrentStep = step;
        this.nextStep = this.TutorialCurrentStep;
        this.ShowTutorial();
    }
    public void ShowTutorial()
    {
        if (this.config == null)
        {
            return;
        }

        this.isShow = true;
        ShowTutorialStep(this.config);
    }

    private void ShowTutorialStep(TutorialConfig config)
    {
        this.ShowFace(false);
        this.ShowHighLight(this.TutorialCurrentStep, false);
        this.ShowTapAnyWhere(config.step, false);

        Debug.Log($"TUT: {config.message}");
        if (config._isShowHighLight)
        {
            this.ShowFace(true);
            this.ShowHighLight(this.TutorialCurrentStep, true);
        }
        else
        {
            this.ShowTapAnyWhere(config.step, true);
        }
    }

    public void DoTutorial(int step)
    {
        if (step != this.TutorialCurrentStep)
        {
            return;
        }

        if (this.config.nextStep != -1)
        {
            TutorialConfig nextStep = TutorialConfigs.Instance.GetTutorialByStep(this.config.nextStep);
            if (nextStep != null)
            {
                this.TutorialCurrentStep = nextStep.step;
                ShowTutorialStep(nextStep);
            }
        }
        else
        {
            this.nextStep = -1;
            this.ShowFace(false);
            this.ShowHighLight(this.TutorialCurrentStep, false);
            this.ShowTapAnyWhere(this.TutorialCurrentStep, false);
            this.isShow = false;
        }
    }
    

    public void ShowHighLight(int _step, bool _isShow)
    {
        if(this.dicHightLights.TryGetValue(_step, out List<TutorialHighLight> values))
        {
            foreach (var h in values)
            {
                if (_isShow)
                {
                    
                    this.panelFace.Play(h.rect);
                }
                
                h.ShowTutorial(_isShow);
            }
        }
    }
    
    private void ShowFace(bool isOn)
    {
        this.panelFace.gameObject.SetActive(isOn);
    }
    
    public void ShowTapAnyWhere(int _step, bool _isShow)
    {
        panelTapAnywhere.gameObject.SetActive(_isShow);
        if(_isShow)
            panelTapAnywhere.step = _step;
    }
#if UNITY_EDITOR

#endif
}
