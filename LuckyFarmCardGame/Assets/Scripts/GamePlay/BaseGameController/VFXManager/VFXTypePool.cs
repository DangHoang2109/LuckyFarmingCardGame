using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VFXTypePool : MonoBehaviour
{
    public int id;
    public VFXBaseObject prefab;
    protected Queue<VFXBaseObject> availablePools;
    protected List<VFXBaseObject> unavailablePools;

    public List<VFXBaseObject> PrepareItems(int _amount, System.Action<VFXBaseObject> cb)
    {
        this.availablePools ??= new Queue<VFXBaseObject>();
        this.unavailablePools ??= new List<VFXBaseObject>();

        VFXBaseObject item;
        List<VFXBaseObject> pools = new List<VFXBaseObject>();
        if (_amount > availablePools.Count)
        {
            for (int i = availablePools.Count; i < _amount; i++)
            {
                item = Instantiate(prefab, this.transform);
                item.Init(cb);
                availablePools.Enqueue(item);
            }
        }

        for (int i = 0; i < _amount; i++)
        {
            item = availablePools.Dequeue();
            unavailablePools.Add(item);
            pools.Add(item);
            item.Init(cb);
        }
        return pools;
    }
    public void ReturnObject(VFXBaseObject _poolItem)
    {
        unavailablePools.Remove(_poolItem);
        availablePools.Enqueue(_poolItem);
        _poolItem.gameObject.SetActive(false);
    }
}