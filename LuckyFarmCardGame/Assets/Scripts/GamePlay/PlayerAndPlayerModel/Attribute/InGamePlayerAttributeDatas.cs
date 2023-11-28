using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerAttributeDatas
{
    public int _seatID;
    public Dictionary<AttributeID, AttributeDataValue> _datas;
    public void JoinGame(int seatID)
    {
        this._seatID = seatID;
        _datas ??= new Dictionary<AttributeID, AttributeDataValue>();
    }
    public void ClearAll()
    {
        _seatID = -1;
        _datas?.Clear();
    }
    public bool TryGetData(AttributeID id, out AttributeDataValue item)
    {
        _datas ??= new Dictionary<AttributeID, AttributeDataValue>();
        return _datas.TryGetValue(id, out item);
    }
    public bool TryGetValue(AttributeID id, out int currentValue)
    {
        if (_datas.TryGetValue(id, out var item))
        {
            currentValue = item.GetValue();
            return true;
        }
        currentValue = 0;
        return false;
    }
    public int AddAttribute(AttributeID id, int value, int turnActive = 1, bool isCountingTurn = true)
    {
        //find the item
        if (!TryGetData(id, out AttributeDataValue item))
        {
            item = new AttributeDataValue()
            {
                _id = id
            };
            _datas.Add(id, item);
        }
        item.AddAttribute(value);
        item.SetTurn(turnActive, isCountingTurn);

        return item.GetValue();
    }
    public int SetAttribute(AttributeID id, int value, int turnActive = 1, bool isCountingTurn = true)
    {
        //find the item
        if (!TryGetData(id, out AttributeDataValue item))
        {
            item = new AttributeDataValue()
            {
                _id = id
            };
            _datas.Add(id, item);
        }
        item.SetAttribute(value);
        item.SetTurn(turnActive, isCountingTurn);

        return item.GetValue();
    }
    public void StartTurn()
    {
        _datas ??= new Dictionary<AttributeID, AttributeDataValue>();
        List< AttributeID > ids = new List< AttributeID >();
        foreach (var item in _datas.Values)
        {
            item.CountingTurn();
            if (item.GetTurnLeft() <= 0) 
                ids.Add(item._id);
        }
        foreach (var item in ids)
        {
            _datas.Remove(item);
        }
    }

    public void AssignCallback(AttributeID id, System.Action<AttributeID, int, int> cb)
    {
        if(TryGetData(id, out AttributeDataValue item))
        {
            item._onChangeValue -= cb;
            item._onChangeValue += cb;
        }
    }
}

public class AttributeDataValue
{
    public AttributeID _id;
    private int _currentValue = 0;
    private int _currentTurnActive;
    private bool _countDownTurn;

    public System.Action<AttributeID, int, int> _onChangeValue;

    public void CountingTurn()
    {
        if (_countDownTurn)
        {
            _currentTurnActive--;
            if (_currentTurnActive <= 0)
            {
                _currentTurnActive = 0;
            }
            _onChangeValue?.Invoke(_id,_currentValue, _currentTurnActive);
        }
    }
    public int GetValue() => this._currentValue;
    public int GetTurnLeft() => this._currentTurnActive;

    public void AddAttribute(int value)
    {
        _currentValue += value;
        _onChangeValue?.Invoke(_id, _currentValue, _currentTurnActive);
    }
    public void SetAttribute(int value)
    {
        _currentValue = value;
        _onChangeValue?.Invoke(_id, _currentValue, _currentTurnActive);
    }
    public void SetTurn(int validTurn, bool isCountingTurn = true)
    {
        this._currentTurnActive = validTurn;
        this._countDownTurn = isCountingTurn;
    }
}