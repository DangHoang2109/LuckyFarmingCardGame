using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VFXBaseObject : MonoBehaviour
{
    [SerializeField] protected int _id;
    public int ID => _id;
    [SerializeField] protected Image imgIcon;
    public System.Action<VFXBaseObject> onCompleteAnime;
    public RectTransform itemRect => this.transform as RectTransform;
    protected float scaleCurve = 50f;
    protected float moveDuration = 0.5f;
     
    protected void OnEnable()
    {
        this.transform.localScale = Vector3.zero;
    }

    public virtual void Init(System.Action<VFXBaseObject> _onCompleteAnime = null)
    {
        this.onCompleteAnime = _onCompleteAnime;
    }
    public virtual void AppendAnimation(System.Action<VFXBaseObject> _onCompleteAnime = null)
    {
        this.onCompleteAnime -= _onCompleteAnime;
        this.onCompleteAnime += _onCompleteAnime;
    }
    public virtual void ParseSprite(Sprite _spr)
    {
        if(this.imgIcon != null)
            this.imgIcon.sprite = _spr;
    }

    public virtual Sequence DoAnimation(Transform _desPos, float delay = 0, PoolPathType _pathType = PoolPathType.STRAIGHT_SCATTERED, float _scaleCurve = 50f, float _moveDuration = 0.5f, List<Transform> _midPoint = null, AnimationCurve _curveX = null, AnimationCurve _curveY = null)
    {
        this.scaleCurve = _scaleCurve;
        this.moveDuration = _moveDuration;
        switch (_pathType)
        {
            case PoolPathType.NONE:
                return null;
            case PoolPathType.STRAIGHT_SCATTERED:
                return DoAnimationGoStraightTarget(_desPos, delay);
            case PoolPathType.BASIC_CURVE:
                return DoAnimationBasicCurve(_desPos, delay);
            case PoolPathType.CUSTOM_CURVE:
                return DoCurveAnimation(_desPos, _curveX, _curveY, delay);
            case PoolPathType.CATMULLROM:
                return DoAnimationCatmullRom(_desPos, _midPoint, delay);
            default:
                return null;
        }
    }

    public Sequence DoAnimationGoStraightTarget(Transform _desPos, float delay = 0)
    {
        Sequence seq = DOTween.Sequence();

        this.transform.DOScale(1, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

        this.transform.DORotate(Vector3.zero, moveDuration).SetDelay(delay + 0.5f).SetEase(Ease.Flash);

        itemRect.DOMove(_desPos.position, moveDuration).SetDelay(delay + 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            onCompleteAnime?.Invoke(this);

            //this.transform.DOScale(0, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            //{
            //});
        });

        return seq;
    }
    public Sequence DoAnimationCatmullRom(Transform _desPos, List<Transform> _midPoint, float delay = 0)
    {

        if (_midPoint == null || _midPoint.Count == 0)
        {
            onCompleteAnime?.Invoke(this);
            return null;
        }

        ///Start -> Middle -> End
        List<Vector3> convertPos = new List<Vector3>() { this.transform.position };
        for (int i = 0; i < _midPoint.Count; i++)
        {
            convertPos.Add(_midPoint[i].position);
        }
        convertPos.Add(_desPos.position);
        Vector3[] paths = convertPos.ToArray();
        Sequence seq = DOTween.Sequence();

        this.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        //this.transform.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.3f).SetEase(Ease.Flash);

        itemRect.DOPath(paths, moveDuration, PathType.CatmullRom).SetDelay(delay + 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            onCompleteAnime?.Invoke(this);

            //this.transform.DOScale(0, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            //{
            //});
        });

        return seq;
    }

    public Sequence DoAnimationBasicCurve(Transform _desPos, float delay = 0)
    {
        Sequence seq = DOTween.Sequence();

        this.transform.DOScale(1, 0.3f)/*.SetDelay(delay)*/.SetEase(Ease.OutBack);

        //this.transform.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);

        itemRect.DOMoveY(_desPos.position.y, moveDuration).SetDelay(delay + 0.3f).SetEase(Ease.InBack);
        itemRect.DOMoveX(_desPos.position.x, moveDuration).SetDelay(delay + 0.3f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            onCompleteAnime?.Invoke(this);

            //this.transform.DOScale(0, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            //{
            //});
        });

        return seq;
    }
    
    public Sequence DoCurveAnimation(Transform _target, AnimationCurve _curveX, AnimationCurve _curveY, float delay = 0)
    {
        Sequence seq = DOTween.Sequence();

        StartCoroutine(Curve(_target, _curveX, _curveY, delay));

        return seq;
    }

    IEnumerator Curve(Transform _target, AnimationCurve _curveX, AnimationCurve _curveY, float delay = 0)
    {
        this.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        float duration = moveDuration;
        float time = 0f;
        Vector3 end = _target.position - (_target.forward * 0.55f); // lead the target a bit to account for travel time, your math will vary
        yield return new WaitForSeconds(delay);
        while (time < duration)
        {
            time += Time.deltaTime;

            float linearT = time / duration;
            float heightT = _curveY.Evaluate(linearT);
            float widthT = _curveX.Evaluate(linearT);

            //float height = heightT; //Mathf.Lerp(0f, 3.0f, heightT); // change 3 to however tall you want the arc to be

            transform.position = Vector2.Lerp(transform.position, end, linearT) + new Vector2(widthT, heightT) * this.scaleCurve;

            yield return null;
        }
        onCompleteAnime?.Invoke(this);
    }
}
