using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class AttributeUI : MonoBehaviour, IQueueFlowable
{
    public AttributeID _id;
    public Image _imgIcon;
    public TextMeshProUGUI _tmpValue;
    private int _currentValue;

    public bool IsReadyForNext { get ; set; }

    public AttributeUI SetInfo(AttributeID id)
    {
        this._id = id;
        this._imgIcon.sprite = AttributeConfigs.Instance.GetIcon(id);
        return this;
    }
    private void OnEnable()
    {
        //Debug.Log($"FUCK WAKE UP {this.gameObject.name} {this.gameObject.activeInHierarchy}"); 
        //this._currentValue = 0;
    }
    public AttributeUI UpdateValue(int currentVal, int currentTurn, bool isHasTurnActive, bool isAnim = true, float durationValue = 1f, bool isPercent = false)
    {
        IsReadyForNext = false;
        VFXActionManager.Instance.ShowVFxXAttributeChange(this);
        int currentCache = _currentValue;
        this._currentValue = currentVal;

        bool turn = !isHasTurnActive || isHasTurnActive && currentTurn > 0;
        bool val = _currentValue > 0; //(Mathf.Max(currentCache, _currentValue)) > 0;
        bool isActive = turn && val;
        this.gameObject.SetActive(isActive);
        this._tmpValue.SetText($"{_currentValue} {(isPercent ? "%" : "")}");

        Debug.Log($"Set active obj -id {this._id} -currentTurn {currentTurn} -turn {turn} -_currentValue {_currentValue} -val {val}{this.gameObject.activeInHierarchy}");
        IsReadyForNext = true;
        //if (isActive)
        //{
        //    if (isAnim)
        //    {
        //        IsReadyForNext = false;

        //        DOTween.Kill(this.GetInstanceID());
        //        Sequence seq = DOTween.Sequence();
        //        seq.SetId(this.GetInstanceID());
        //        // Update text value using DOTween
        //        seq.Join(DOTween.To(() => currentCache, x => this._tmpValue.SetText($"{x} {(isPercent ? "%" : "")}"), currentVal, durationValue));
        //        // Update image fill amount using DOTween
        //        seq.OnComplete(() => { this.gameObject.SetActive(isActive); IsReadyForNext = true; });
        //    }
        //    else
        //    {
        //        this._tmpValue.SetText($"{_currentValue} {(isPercent ? "%" : "")}");
        //        this.gameObject.SetActive(isActive);
        //    }
        //}


        return this;
    }
    public void OnClick()
    {
        Debug.Log("Show the attribute");
    }
    private void OnDisable()
    {
        
    }
}
