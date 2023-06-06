using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FxBooster : MonoBehaviour
{
    [Header("Image")]
    public Image imgBooster;

    public void ShowBooster(BoosterType type)
    {
        BoosterConfig boosterConfig = BoosterConfigs.Instance.GetBooster(type);
        if (boosterConfig != null)
        {
            this.imgBooster.sprite = boosterConfig.spr;
        }
    }

    public void ShowBooster(Sprite spr)
    {
        this.imgBooster.sprite = spr;
    }
}
