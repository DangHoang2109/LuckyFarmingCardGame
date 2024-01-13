using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMoveToTarget : MonoBehaviour
{
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private RectTransform centerTrans, panelContent;
    #region Move To Item

    private RectTransform mScrollTransform => this._scrollRect.transform as RectTransform;
    private RectTransform mContent => this.panelContent;

    public void OnMoveToCenter(RectTransform target)
    {
        this.StartCoroutine(this.OnWaitingCenterITem(target));
    }

    private IEnumerator OnWaitingCenterITem(RectTransform target)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        this.CenterOnItem(target);
    }

    private void CenterOnItem(RectTransform target)
    {
        // Item is here
        var itemCenterPositionInScroll = GetWorldPointInWidget(this.mScrollTransform, GetWidgetWorldPoint(target));
        // But must be here
        var targetPositionInScroll = GetWorldPointInWidget(this.mScrollTransform, GetWidgetWorldPoint(this.centerTrans));
        // So it has to move this distance
        var difference = targetPositionInScroll - itemCenterPositionInScroll;
        difference.z = 0f;
        
        //clear axis data that is not enabled in the scrollrect
        if (!this._scrollRect.horizontal)
        {
            difference.x = 0f;
        }
        if (!this._scrollRect.vertical)
        {
            difference.y = 0f;
        }
 
        var normalizedDifference = new Vector2(
            difference.x / (this.mContent.rect.size.x - mScrollTransform.rect.size.x),
            difference.y / (this.mContent.rect.size.y - mScrollTransform.rect.size.y));
        
        var newNormalizedPosition = this._scrollRect.normalizedPosition - normalizedDifference;
        if (this._scrollRect.movementType != ScrollRect.MovementType.Unrestricted)
        {
            newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
            newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
        }
        this._scrollRect.normalizedPosition = newNormalizedPosition;
    }

    private Vector3 GetWidgetWorldPoint(RectTransform target)
    {
        //pivot position + item size has to be included
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);
        var localPosition = target.localPosition + pivotOffset;
        return target.parent.TransformPoint(localPosition);
    }
    private Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
    {
        return target.InverseTransformPoint(worldPoint);
    }

    #endregion
}
