using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardPointPalletUI : MonoBehaviour
{
    [SerializeField] private float _reduceSpacePerCard, _originalSpacePerCard;
    [SerializeField] private int _cardStartReduce;
    private List<BaseCardItem> _items;
    public GridLayoutGroup _grid;

    private RectTransform _rectTf;
    public RectTransform RectTF
    {
        get
        {
            if (this._rectTf == null)
                this._rectTf = this.transform as RectTransform;
            return _rectTf;
        }
    }
    public void Turn(bool isOn) => this.gameObject.SetActive(isOn);
    public void AppendItem(BaseCardItem item)
    {
        _items ??= new List<BaseCardItem>();
        _items.Add(item);
        item.transform.SetParent(this.RectTF);
        this._grid.spacing = _items.Count < _cardStartReduce ? new Vector2(_originalSpacePerCard, 0) : new Vector2((_items.Count - _cardStartReduce + 1) * _reduceSpacePerCard, 0);
    }
    public void ClearPallet() => _items?.Clear();
}
