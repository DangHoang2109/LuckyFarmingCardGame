using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine;


public enum PoolPathType
{
    NONE = -1,
    STRAIGHT_SCATTERED = 0,
    BASIC_CURVE = 1,
    CUSTOM_CURVE = 2,
    BEZIER = 3,
    CATMULLROM = 4,
    STRAIGHT_FIRE = 5,
}

public class VFXManager : MonoSingleton<VFXManager>
{
    [SerializeField] protected RectTransform boosterContent;
    [SerializeField] protected List<VFXTypePool> boosterPfs;
    private Dictionary<int, VFXTypePool> boosterPool;
    [SerializeField] protected float randomX;
    [SerializeField] protected float randomY;
    
    public PoolPathType pathType;
    public float moveDuration = 0.5f;

    public AnimationCurve curveX;
    public AnimationCurve curveY;
    public float scaleCurve;

    public List<Transform> midPointTfs;

    public List<Vector3> randomPos;
    public List<Vector3> randomRot;
    private IList<Vector3> shufflePos;
    private IList<Vector3> shuffleRot;

    public virtual int MAX_POOL_PER_ANIME => 10;

    /// <summary>
    /// Callback when 1 item complete animation
    /// </summary>
    private System.Action onCompletePerItemCb;
    private System.Action onCompleteAllCallback;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.boosterPfs = new List<VFXTypePool>(GetComponentsInChildren<VFXTypePool>());
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        randomPos = new List<Vector3>();
        randomRot = new List<Vector3>();

        boosterPool ??= new Dictionary<int, VFXTypePool>();
        foreach (var item in this.boosterPfs)
        {
            boosterPool.Add(item.id, item);
        }
    }
    /// <summary>
    /// Start Rotate and Position of PoolItem is scattered.
    /// Item go Straight to target
    /// </summary>
    /// <param name="_desTransform"></param>
    /// <param name="_rewardConfig"></param>
    /// <param name="_onCompletePerItemCb"></param>
    /// <param name="_onCompleteAllCb"></param>
    /// <returns></returns>
    public virtual VFXManager ShowFX(int id, int amount, Transform _desTransform, Transform _startTransform = null, PoolPathType _pathType = PoolPathType.STRAIGHT_SCATTERED, System.Action _onCompletePerItemCb = null, System.Action _onCompleteAllCb = null)
    {
        this.onCompletePerItemCb = _onCompletePerItemCb;
        this.onCompleteAllCallback = _onCompleteAllCb;

        StartCoroutine(ParseData(id,amount, _desTransform, _startTransform, _pathType));
        return this;
    }

    public virtual IEnumerator ParseData(int id, int _amount, Transform _desTransform, Transform _startTransform = null, PoolPathType _pathType = PoolPathType.STRAIGHT_SCATTERED)
    {
        yield return new WaitForEndOfFrame();
        if (_amount > MAX_POOL_PER_ANIME)
        {
            _amount = MAX_POOL_PER_ANIME;
        }
        VFXBaseObject item;
        if (!this.boosterPool.TryGetValue(id, out VFXTypePool pool))
        {
            yield break;
        }
        List<VFXBaseObject> pools = pool.PrepareItems(_amount, OnBoosterPoolComplete);
        
        switch (_pathType)
        {
            case PoolPathType.NONE:
                break;
            case PoolPathType.STRAIGHT_SCATTERED:
                SetUpAnimationScatter(pools);
                break;
            case PoolPathType.BASIC_CURVE:
            case PoolPathType.CUSTOM_CURVE:
            case PoolPathType.BEZIER:
            case PoolPathType.CATMULLROM:
                SetUpAnimationGathered(pools, _startTransform);
                break;
            default:
                break;
        }


        StartCoroutine(DoAnimation(_desTransform, pools, _pathType));

        /// <summary>
        /// Random Position and Rotation Pool Item
        /// </summary>
        /// <param name="_pools"></param>
        void SetUpAnimationScatter(List<VFXBaseObject> _pools)
        {
            // Random Position and Rotation Pool Item
            if (randomPos.Count < _pools.Count + 2)
            {
                for (int i = randomPos.Count; i < _pools.Count + 2; i++)
                {
                    randomPos.Add(new Vector3(this.transform.position.x + Random.Range(-randomX, randomX), this.transform.position.y + Random.Range(-randomY, randomY)));
                    randomRot.Add(new Vector3(0, 0, Random.Range(0f, 360f)));
                }
            }

            shufflePos = randomPos.Shuffle();
            shuffleRot = randomRot.Shuffle();

            for (int i = 0; i < _pools.Count; i++)
            {
                _pools[i].transform.position = shufflePos[i];
                _pools[i].transform.rotation = Quaternion.Euler(shuffleRot[i]);

                _pools[i].gameObject.SetActive(true);
            }
        }
        void SetUpAnimationGathered(List<VFXBaseObject> _pools, Transform _startPos)
        {
            if (_startPos == null)
            {
                //Middle Screen
                _startPos = this.transform;
            }
            for (int i = 0; i < _pools.Count; i++)
            {
                _pools[i].transform.position = _startPos.position;
                _pools[i].transform.rotation = _startPos.rotation;

                _pools[i].gameObject.SetActive(true);
            }
        }
    }
    protected virtual IEnumerator DoAnimation(Transform _desPos, List<VFXBaseObject> _pools, PoolPathType _pathType = PoolPathType.STRAIGHT_SCATTERED)
    {
        yield return new WaitForSeconds(0.1f);

        float delay = 0;
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < _pools.Count; i++)
        {
            delay = i * 0.1f;
            _pools[i].DoAnimation(_desPos, delay, _pathType, scaleCurve, moveDuration, midPointTfs, curveX, curveY);
        }

        seq.AppendInterval(delay + 1f);
        seq.OnComplete(() => {
            this.onCompleteAllCallback?.Invoke();
        });
    }

    /// <summary>
    /// Callback when 1 pool item complete animation
    /// </summary>
    /// <param name="_poolItem"></param>
    private void OnBoosterPoolComplete(VFXBaseObject _poolItem)
    {
        ReturnObject(_poolItem);
        onCompletePerItemCb?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
        }
    }

    /// <summary>
    /// Get object for treating animation yourself
    /// Use when you want to custom with some action block
    /// </summary>
    /// <returns></returns>
    public List<T> GetObjects<T>(int id, int _amount) where T : VFXBaseObject
    {
        if (!this.boosterPool.TryGetValue(id, out VFXTypePool pool))
        {
            return new List<T>();
        }
        List<VFXBaseObject> result = pool.PrepareItems(_amount, ReturnObject);

        List<T> pools = new List<T>();
        foreach (VFXBaseObject item in result)
        {
            pools.Add(item as T);
        }

        return pools;
    }
    /// <summary>
    /// Return the object to me when you dont want it 
    /// </summary>
    public void ReturnObject(VFXBaseObject _poolItem)
    {
        if (this.boosterPool.TryGetValue(_poolItem.ID, out VFXTypePool pool))
        {
            pool.ReturnObject(_poolItem);
        }
    }
}

public static class VFXGameID
{
    public const int None = -1;
    public const int orbCardExp = 0;
    public const int AttackSword = 1;
    public const int DefendShield = 2;
    public const int HealFlower = 3;
    public const int CardFly = 4;
    public const int orbHPRed = 5;
    public const int BuffGreen = 6;
    public const int FireCircle = 7;
    public const int BuffBlue = 8;
    public const int SwordBlue = 9;

}