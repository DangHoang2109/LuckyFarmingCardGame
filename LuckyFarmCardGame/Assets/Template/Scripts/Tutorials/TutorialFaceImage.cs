using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialFaceImage : MaskableGraphic, ICanvasRaycastFilter
{
    private RectTransform _target;
    private Vector2 _targetMin;
    private Vector2 _targetMax;
    [SerializeField]
    private RectTransform _targetArea;
    [SerializeField]
    private RectTransform targetPanel;
    public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if(_isClickable)
            return RectTransformUtility.RectangleContainsScreenPoint(_targetArea, sp, eventCamera);
        else
            return !RectTransformUtility.RectangleContainsScreenPoint(_targetArea, sp, eventCamera);

        //if (!_btnClickOn.gameObject.activeInHierarchy)
        //    return !RectTransformUtility.RectangleContainsScreenPoint(_targetArea, sp, eventCamera);
        //else
        //    return RectTransformUtility.RectangleContainsScreenPoint(_btnClickOn.transform as RectTransform, sp, eventCamera);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Play(RectTransform target)
    {
        gameObject.SetActive(true);
        
        this.targetPanel = target;
        this.UpdateTargetArea();
        LateUpdate();
    }

    private void UpdateTargetArea()
    {
        if (this.targetPanel == null)
        {
            return;
        }
        var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main,  this.targetPanel.position);

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, Camera.main,
                out localPoint))
        {
            Close();
            return;
        }
       
        UpdateRectToTarget(_targetArea, localPoint);
        //if(_btnClickOn.gameObject.activeInHierarchy)
        //    UpdateRectToTarget(this._btnClickOn.transform as RectTransform, localPoint);

        _targetArea.ForceUpdateRectTransforms();
        _target = _targetArea;
        _target.ForceUpdateRectTransforms();
    }
    void UpdateRectToTarget(RectTransform _targetArea, Vector2 localPoint)
    {
        _targetArea.anchorMax = this.targetPanel.anchorMax;
        _targetArea.anchorMin = this.targetPanel.anchorMin;
        _targetArea.anchoredPosition = this.targetPanel.anchoredPosition;
        _targetArea.anchoredPosition3D = this.targetPanel.anchoredPosition3D;
        _targetArea.offsetMax = this.targetPanel.offsetMax;
        _targetArea.offsetMin = this.targetPanel.offsetMin;
        _targetArea.pivot = this.targetPanel.pivot;
        _targetArea.sizeDelta = this.targetPanel.sizeDelta;
        _targetArea.localPosition = localPoint;

        _targetArea.ForceUpdateRectTransforms();
        _target = _targetArea;
        _target.ForceUpdateRectTransforms();
    }
    public void Init()
    {
        Close();
    }
    

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();

        var maskRect = rectTransform.rect;

        var maskRectLeftTop = new Vector2(-maskRect.width / 2, maskRect.height / 2);
        var maskRectLeftBottom = new Vector2(-maskRect.width / 2, -maskRect.height / 2);
        var maskRectRightTop = new Vector2(maskRect.width / 2, maskRect.height / 2);
        var maskRectRightBottom = new Vector2(maskRect.width / 2, -maskRect.height / 2);

        var targetRectLeftTop = new Vector2(_targetMin.x, _targetMax.y);
        var targetRectLeftBottom = _targetMin;
        var targetRectRightTop = _targetMax;
        var targetRectRightBottom = new Vector2(_targetMax.x, _targetMin.y);
        
        toFill.AddVert(maskRectLeftBottom, color, Vector2.zero);
        toFill.AddVert(targetRectLeftBottom, color, Vector2.zero);
        toFill.AddVert(targetRectRightBottom, color, Vector2.zero);
        toFill.AddVert(maskRectRightBottom, color, Vector2.zero);
        toFill.AddVert(targetRectRightTop, color, Vector2.zero);
        toFill.AddVert(maskRectRightTop, color, Vector2.zero);
        toFill.AddVert(targetRectLeftTop, color, Vector2.zero);
        toFill.AddVert(maskRectLeftTop, color, Vector2.zero);
        
        toFill.AddTriangle(0, 1, 2);
        toFill.AddTriangle(2, 3, 0);
        toFill.AddTriangle(3, 2, 4);
        toFill.AddTriangle(4, 5, 3);
        toFill.AddTriangle(6, 7, 5);
        toFill.AddTriangle(5, 4, 6);
        toFill.AddTriangle(7, 6, 1);
        toFill.AddTriangle(1, 0, 7);
    }
    
    
    void LateUpdate()
    {
        this.UpdateTargetArea();
        RefreshView();
    }
    
    private void RefreshView()
    {
        Vector2 newMin;
        Vector2 newMax;
        if (_target != null && _target.gameObject.activeSelf)
        {
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform, _target);
            newMin = bounds.min;
            newMax = bounds.max;
        }
        else
        {
            newMin = Vector2.zero;
            newMax = Vector2.zero;
        }
        if (_targetMin != newMin || _targetMax != newMax)
        {
            _targetMin = newMin;
            _targetMax = newMax;
            SetAllDirty();
        }
    }

    protected int _step;
    protected bool _isClickable;
    /// <summary>
    /// Use this if you need to highlight anywhere need tap, but it not be a button
    /// </summary>
    /// <param name="step"></param>
    /// <param name="isClickable"></param>
    public void SetClickable(int step, bool isClickable)
    {
        _isClickable = isClickable;
        this._step = step;
    }
}
