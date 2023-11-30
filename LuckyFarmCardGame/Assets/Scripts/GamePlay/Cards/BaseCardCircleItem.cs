using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCardCircleItem : BaseCardItem
{
    public void OnClickItem()
    {
        //Show card effect
        Debug.Log($"Card Effect: {CardConfig.GetBaseLevelDescription()}");
    }
}
