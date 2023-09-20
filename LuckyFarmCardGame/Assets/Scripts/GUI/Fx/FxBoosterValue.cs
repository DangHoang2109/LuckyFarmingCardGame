using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FxBoosterValue : MonoBehaviour
{
    public Image icon;
    public TMPro.TextMeshProUGUI tmpValue;
    public RectTransform rect;
    public RectTransform goBubble;
    public Vector3 speedVector = new Vector3(0, 35f);

    private float durationFly = 0f;
    private float delayTime = 0f;

    public FxBoosterValue SetDelay(float delay)
    {
        if (delay > 0)
        {
            goBubble.gameObject.SetActive(false);
            delayTime = delay;
        }

        return this;
    }

    public FxBoosterValue SetTimeBubbleUp(float _durationFly)
    {
        goBubble.transform.localPosition = Vector3.zero;
        this.durationFly = _durationFly;
        return this;
    }

    public FxBoosterValue SetTextnSprite(string text, Sprite _sprite = null)
    {
        if (_sprite == null)
        {
            icon.gameObject.SetActive(false);
        }
        else
        {
            icon.gameObject.SetActive(true);
            icon.sprite = _sprite;
        }
        tmpValue.SetText(text);
        goBubble.localPosition = Vector3.zero;
        AutoSizing();
        return this;
    }

    public FxBoosterValue SetTextColor(string colorStr)
    {
        if (ColorUtility.TryParseHtmlString(colorStr, out Color color ))
        {
            tmpValue.color = color;
        }

        return this;
    }

    public FxBoosterValue SetTextColor(Color color)
    {
        tmpValue.color = color;

        return this;
    }

    public FxBoosterValue SetAnimationText(long fromVal, long toVal, float duration = 1.0f, string _format = "{0}")
    {
        icon.gameObject.SetActive(false);
        long cur = fromVal;
        Sequence seq = DOTween.Sequence();
        //seq.AppendInterval(timeDelay);
        seq.Append(DOTween.To(() => cur, x => cur = x, toVal, duration)).SetEase(Ease.Linear).OnUpdate(
            () => 
            {
                this.tmpValue.SetText(string.Format(_format, cur));
            });
        return this;
    }

    private void Update()
    {
        if (durationFly > 0)
        {
            if (delayTime > 0)
            {
                delayTime -= Time.deltaTime;
                return;
            }
            else
            {
                goBubble.gameObject.SetActive(true);
            }

            goBubble.position += speedVector * Time.deltaTime;
            durationFly -= Time.deltaTime;
            if (durationFly <=0)
            {
                FxHelper.Instance.ReturnBubble(this);
                gameObject.SetActive(false);
            }
        }
    }

    public void AutoSizing()
    {
        if (!tmpValue.enableAutoSizing)
        {
            tmpValue.rectTransform.sizeDelta = new Vector2(tmpValue.preferredWidth, tmpValue.preferredHeight);
            rect.sizeDelta = tmpValue.rectTransform.sizeDelta;
        }
    }
}
