using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class HPBarUI : MonoBehaviour
{
    public Image _imgFill;
    public TextMeshProUGUI _tmpValue;

    private int _currentValue = 0, _maxValue = 0;

    public HPBarUI SetMaxValue(int maxVal, bool isSetCurrentToMax = true)
    {
        this._maxValue = maxVal;
        if (isSetCurrentToMax)
            this._currentValue = maxVal;

        this._tmpValue.SetText($"{_currentValue}/{this._maxValue}");
        return this;
    }
    public HPBarUI UpdateValue(int currentVal, bool isAllowExceedMax = false, bool isAnim = true, float durationValue = 1f)
    {
        int currentCache = _currentValue;
        this._currentValue = currentVal;
        if (!isAllowExceedMax)
            this._currentValue = Mathf.Clamp(_currentValue, 0, this._maxValue);

        float fill = Mathf.Clamp((float)_currentValue / _maxValue, 0f, 1f);
        if (isAnim)
        {
            DOTween.Kill(this.GetInstanceID());
            Sequence seq = DOTween.Sequence();
            seq.SetId(this.GetInstanceID());
            // Update text value using DOTween
            seq.Join(DOTween.To(() => currentCache , x => this._tmpValue.SetText($"{x}/{this._maxValue}"), currentVal, durationValue));
            // Update image fill amount using DOTween
            seq.Join(DOTween.To(() => _imgFill.fillAmount, x => _imgFill.fillAmount = x, fill, durationValue));
        }
        else
        {
            this._tmpValue.SetText($"{_currentValue}/{this._maxValue}");
            this._imgFill.fillAmount = fill;
        }

        return this;
    }
}
