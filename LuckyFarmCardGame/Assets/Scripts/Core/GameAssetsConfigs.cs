using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GameAssetsConfigs", fileName = "GameAssetsConfigs")]
public class GameAssetsConfigs : ScriptableObject
{
    [Header("Animation button")]
    public RuntimeAnimatorController animBtController;

    [Header("Booster")]
    public BoosterConfigs boosters;

    [Header("Coin cash with value")]
    public SpriteIconValueConfigs valueIconAsset;

    [Header("Spire point battle pass")]
    public Sprite sprPointBattePass;
    public static GameAssetsConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<GameAssetsConfigs>("Home/Configs/GameAssetsConfigs");
        }
    }
}

#region BOOSTER
[System.Serializable]
public class BoosterConfigs
{
    public List<BoosterConfig> boosters;
    public static BoosterConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.boosters;
        }
    }

    public BoosterConfig GetBooster(BoosterType type)
    {
        return this.boosters.Find(x => x.type == type);
    }
}


[System.Serializable]
public class BoosterConfig
{
    public BoosterType type;
    public string name;
    public string description;
    public Sprite spr;
    public Sprite sprOff;
}
#endregion

#region Coin sprite
[System.Serializable]
public class SpriteIconValueConfigs
{
    public static SpriteIconValueConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.valueIconAsset;
        }
    }

    [Header("Coins")]
    public List<SpriteValueIconConfig> coinIcons;

    [Header("Cashs")]
    public List<SpriteValueIconConfig> cashIcons;

    public Sprite GetSpriteCoin(long value)
    {
        for (int i = this.coinIcons.Count - 1; i >= 0; i--)
        {
            if (this.coinIcons[i].value <= value) return this.coinIcons[i].spr;
        }
        return this.coinIcons[0].spr;
    }

    public Sprite GetSpriteCash(long value)
    {
        for (int i = this.cashIcons.Count - 1; i >= 0; i--)
        {
            if (this.cashIcons[i].value <= value) return this.cashIcons[i].spr;
        }
        return this.cashIcons[0].spr;
    }

    public Sprite GetSprite(BoosterType type, long value)
    {
        switch (type)
        {
            case BoosterType.COIN:
                return GetSpriteCoin(value);
            case BoosterType.CASH:
                return GetSpriteCash(value);
        }
        return null;
    }
}

[System.Serializable]
public class SpriteValueIconConfig
{
    public long value;
    public Sprite spr;
}
#endregion
