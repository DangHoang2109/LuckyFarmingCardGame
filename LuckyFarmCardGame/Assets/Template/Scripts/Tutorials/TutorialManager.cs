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
    [SerializeField]
    private TutorialFaceHighLightNoTap panelHighLight;

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
    [SerializeField] private TutorialSpeechPopup _tutorialPopup;
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
            _tutorialPopup.OnClose();
            return;
        }

        this.isShow = true;
        ShowTutorialStep(this.config);
    }

    private void ShowTutorialStep(TutorialConfig config)
    {
        //close previous tut step and popup
        this.ShowFace(false);
        this.ShowHightLight(false);
        this.ShowHighLight_NeedTap(this.TutorialCurrentStep, false);
        this.ShowHightLight_NoTap(this.TutorialCurrentStep, false);

        if (!this._tutorialPopup.gameObject.activeInHierarchy)
            _tutorialPopup.OnShow();

        Debug.Log($"TUT: {config.message}");
        switch (config._tutType)
        {
            case TutorialType.TAP:
                this.ShowFace(true);
                this.ShowHighLight_NeedTap(this.TutorialCurrentStep, true);
                _tutorialPopup.SetIsNeedClick(false);
                _tutorialPopup.SetText(TutorialCurrentStep, config.message);
                break;
            case TutorialType.HIGHLIGHT_ONLY:
                this.ShowHightLight(true);
                this.ShowHightLight_NoTap(this.TutorialCurrentStep, true);
                _tutorialPopup.SetText(TutorialCurrentStep, config.message);
                _tutorialPopup.SetIsNeedClick(true);
                break;
            case TutorialType.SPEECH_ONLY:
                _tutorialPopup.SetText(TutorialCurrentStep, config.message);
                _tutorialPopup.SetIsNeedClick(true);
                break;
        }

        //if (config._isShowHighLight)
        //{
        //    this.ShowFace(true);
        //    this.panelFace.SetClickable(TutorialCurrentStep, config._isNeedTapOnFace);
        //    this.ShowHighLight(this.TutorialCurrentStep, true);
        //}
        //else
        //{
        //    this.ShowTapAnyWhere(config.step, true);
        //}
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
            this.ShowHighLight_NeedTap(this.TutorialCurrentStep, false);
            this.isShow = false;
        }
    }
    
    public void ShowHightLight_NoTap(int _step, bool _isShow)
    {
        if (this.dicHightLights.TryGetValue(_step, out List<TutorialHighLight> values))
        {
            HightLight(values);
        }
        else
        {
            //find that object again
            List<TutorialHighLight> tutHighlight = new List<TutorialHighLight>(FindObjectsOfType<TutorialHighLight>());
            if (tutHighlight != null && tutHighlight.Count > 0)
            {
                List<TutorialHighLight> highLightsByCode = tutHighlight.FindAll(x => x.step == _step);
                if (highLightsByCode != null && highLightsByCode.Count > 0)
                {
                    this.dicHightLights.Add(_step, highLightsByCode);
                    HightLight(highLightsByCode);
                }
            }
        }
        void HightLight(List<TutorialHighLight> values)
        {
            foreach (var h in values)
            {
                if (_isShow)
                {
                    this.panelHighLight.Play(h.rect);
                }

                h.ShowTutorial(_isShow);
            }
        }
    }
    public void ShowHighLight_NeedTap(int _step, bool _isShow)
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
        else
        {
            //find that object again
            List< TutorialHighLight> tutHighlight = new List<TutorialHighLight>( FindObjectsOfType<TutorialHighLight>());
            if(tutHighlight != null && tutHighlight.Count > 0)
            {
                List<TutorialHighLight> highLightsByCode = tutHighlight.FindAll(x => x.step == _step);
                if(highLightsByCode != null && highLightsByCode.Count > 0)
                {
                    this.dicHightLights.Add(_step, highLightsByCode);
                }
            }
        }
    }
    
    private void ShowFace(bool isOn)
    {
        this.panelFace.gameObject.SetActive(isOn);
    }
    private void ShowHightLight(bool isOn)
    {
        this.panelHighLight.gameObject.SetActive(isOn);
    }
#if UNITY_EDITOR

#endif
}
