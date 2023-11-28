using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The holder of Attribute Icons
/// </summary>
public class PlayerAttributeUI : MonoBehaviour
{
    public Transform _tfPanel;
    public AttributeUI _prefab;
    public List<AttributeUI> _items;
    public Dictionary<AttributeID, AttributeUI> _dicItem;

    public Transform Panel => _tfPanel;
    public bool TryGetItem(AttributeID id, out AttributeUI item)
    {
        _items ??= new List<AttributeUI>();
        _dicItem ??= new Dictionary<AttributeID, AttributeUI>();
        return _dicItem.TryGetValue(id, out item);
    }
    public AttributeUI GetItemSafe(AttributeID id)
    {
        if(!TryGetItem(id, out AttributeUI item))
        {
            item = AddItem(id);
        }
        return item;

    }
    private AttributeUI AddItem(AttributeID id)
    {
        AttributeUI item = Instantiate(this._prefab, this._tfPanel);
        item.SetInfo(id);
        _items.Add(item);
        _dicItem.Add(id, item);
        return item;
    }
    public void AddAttribute(AttributeID id, int value, bool isAnim = true, float durationValue = 1, bool isPercent = false)
    {
        //find the item
        if(!TryGetItem(id, out AttributeUI item))
        {
            item = AddItem(id);
        }
        item.UpdateValue(value, isAnim: isAnim, durationValue: durationValue, isPercent: isPercent);
    }
}
