using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class ShieldUI : MonoBehaviour
{
    public TextMeshProUGUI _tmpValue;
    private int _currentValue = 0;

    public ShieldUI UpdateValue(int currentVal, bool isAnim = true, float durationValue = 1f)
    {
        int currentCache = _currentValue;
        this._currentValue = currentVal;

        if (isAnim)
        {
            DOTween.Kill(this.GetInstanceID());
            Sequence seq = DOTween.Sequence();
            seq.SetId(this.GetInstanceID());
            // Update text value using DOTween
            seq.Join(DOTween.To(() => currentCache, x => this._tmpValue.SetText($"{x}"), currentVal, durationValue));
            // Update image fill amount using DOTween
        }
        else
        {
            this._tmpValue.SetText($"{_currentValue}");
        }

        return this;
    }
}
