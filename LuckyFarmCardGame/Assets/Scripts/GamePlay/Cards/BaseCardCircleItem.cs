using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCardCircleItem : BaseCardItem
{
    public void OnClickItem()
    {
        Debug.Log($"Card Effect: {this.cardModel?.GetSkillDescribe()}");
    }
}
