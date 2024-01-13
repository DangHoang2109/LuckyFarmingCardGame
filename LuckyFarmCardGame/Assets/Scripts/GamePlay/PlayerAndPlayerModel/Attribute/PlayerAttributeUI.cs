using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The holder of Attribute Icons
/// </summary>
public class PlayerAttributeUI : MonoBehaviour
{
    protected InGameBasePlayerItem _host;
    public Transform _tfPanel;
    public AttributeUI _prefab;
    public List<AttributeUI> _items;

    public Dictionary<AttributeID, AttributeUI> _dicItem;

    public Transform Panel => _tfPanel;

    private void Start()
    {
    }
    public void JoinGame(InGameBasePlayerItem h)
    {
        _host = h;
        _dicItem ??= new Dictionary<AttributeID, AttributeUI>();

        foreach (var item in _dicItem.Values)
        {
            _host.AssignCallbackToAttributeData(item._id, OnChangeValueOfAttribute);
        }
    }
    public bool TryGetItem(AttributeID id, out AttributeUI item)
    {
        _items ??= new List<AttributeUI>();
        _dicItem ??= new Dictionary<AttributeID, AttributeUI>();
        return _dicItem.TryGetValue(id, out item);
    }
    private AttributeUI AddItem(AttributeID id)
    {
        AttributeUI item = Instantiate(this._prefab, this._tfPanel);
        item.SetInfo(id);

        _items.Add(item);
        _dicItem.Add(id, item);

        //bind cái callback vô
        _host.AssignCallbackToAttributeData(id, OnChangeValueOfAttribute);
        return item;
    }
    public void AddAttribute(AttributeID id, int value, int turnActive = 1, bool isCountingTurn = true, bool isAnim = true, float durationValue = 1, bool isPercent = false)
    {
        //find the item
        if(!TryGetItem(id, out AttributeUI item))
        {
            item = AddItem(id);
        }
        else
        {
            //nếu item này đang bị inactive -> đã từng có -> reuse nhưng att data bị clear rồi -> cần bind lại callback
            if(!item.gameObject.activeInHierarchy)
                _host.AssignCallbackToAttributeData(id, OnChangeValueOfAttribute);
        }
        item.UpdateValue(value, turnActive, isCountingTurn, isAnim: isAnim, durationValue: durationValue, isPercent: isPercent);
    }
    protected void OnChangeValueOfAttribute(AttributeID id, int value, int turnActive)
    {
        AddAttribute(id, value, turnActive);
    }
}
