using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class AttributeUI : MonoBehaviour
{
    public AttributeID _id;
    public Image _imgIcon;
    public TextMeshProUGUI _tmpValue;
    private int _currentValue = 0;

    public AttributeUI SetInfo(AttributeID id)
    {
        this._id = id;
        this._imgIcon.sprite = AttributeConfigs.Instance.GetIcon(id);
        return this;
    }
    public AttributeUI UpdateValue(int currentVal, bool isAnim = true, float durationValue = 1f, bool isPercent = false)
    {
        int currentCache = _currentValue;
        this._currentValue = currentVal;
        this.gameObject.SetActive((Mathf.Max(currentVal, _currentValue)) > 0);

        if (isAnim)
        {
            DOTween.Kill(this.GetInstanceID());
            Sequence seq = DOTween.Sequence();
            seq.SetId(this.GetInstanceID());
            // Update text value using DOTween
            seq.Join(DOTween.To(() => currentCache, x => this._tmpValue.SetText($"{x} {(isPercent ? "%" : "")}"), currentVal, durationValue));
            // Update image fill amount using DOTween
            seq.OnComplete(() => { this.gameObject.SetActive(currentVal > 0); });
        }
        else
        {
            this._tmpValue.SetText($"{_currentValue} {(isPercent ? "%" : "")}");
            this.gameObject.SetActive(currentVal > 0);
        }

        return this;
    }
    public void OnClick()
    {
        Debug.Log("Show the attribute");
    }
}
