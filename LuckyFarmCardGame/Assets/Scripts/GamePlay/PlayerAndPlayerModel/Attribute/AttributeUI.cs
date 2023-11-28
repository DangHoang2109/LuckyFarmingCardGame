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
    private int _currentValue = 0;

    public bool IsReadyForNext { get ; set; }

    public AttributeUI SetInfo(AttributeID id)
    {
        this._id = id;
        this._imgIcon.sprite = AttributeConfigs.Instance.GetIcon(id);
        return this;
    }
    private void OnEnable()
    {
        Debug.Log($"FUCK WAKE UP {this.gameObject.name} {this.gameObject.activeInHierarchy}");
    }
    public AttributeUI UpdateValue(int currentVal, int currentTurn, bool isHasTurnActive, bool isAnim = true, float durationValue = 1f, bool isPercent = false)
    {
        VFXActionManager.Instance.ShowVFxXAttributeChange(this);
        int currentCache = _currentValue;
        this._currentValue = currentVal;

        bool turn = !isHasTurnActive || isHasTurnActive && currentTurn > 0;
        bool val = (Mathf.Max(currentVal, _currentValue)) > 0;
        bool isActive = turn && val;
        this.gameObject.SetActive(isActive);

        Debug.Log($"Set active obj {this._id} {turn} {val} {this.gameObject.name} {this.gameObject.activeInHierarchy}");
        if (isActive)
        {
            if (isAnim)
            {
                IsReadyForNext = false;

                DOTween.Kill(this.GetInstanceID());
                Sequence seq = DOTween.Sequence();
                seq.SetId(this.GetInstanceID());
                // Update text value using DOTween
                seq.Join(DOTween.To(() => currentCache, x => this._tmpValue.SetText($"{x} {(isPercent ? "%" : "")}"), currentVal, durationValue));
                // Update image fill amount using DOTween
                seq.OnComplete(() => { this.gameObject.SetActive(turn && currentVal > 0); IsReadyForNext = true; });
            }
            else
            {
                this._tmpValue.SetText($"{_currentValue} {(isPercent ? "%" : "")}");
                this.gameObject.SetActive(turn && currentVal > 0);
            }
        }


        return this;
    }
    public void OnClick()
    {
        Debug.Log("Show the attribute");
    }
}
