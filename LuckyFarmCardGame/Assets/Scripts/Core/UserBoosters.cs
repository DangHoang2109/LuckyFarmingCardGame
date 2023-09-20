using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 618

[System.Serializable]
public class UserBoosters
{
    public List<BoosterCommodity> boosters;

    private static UserBoosters instance;

    public static UserBoosters Instance
    {
        get
        {
            //if(instance == null)
            //{
            //    instance = GameDataManager.Instance.Boosters;
            //}
            //return instance;
            return GameDataManager.Instance.Boosters;
        }
    }

    public UserBoosters()
    {
        this.boosters = new List<BoosterCommodity>();
    }
    /// <summary>
    /// Create new user
    /// </summary>
    public void CreateUser()
    {
        this.AddBooster(BoosterType.COIN, GameDefine.COIN_DEFAULT);
        this.AddBooster(BoosterType.CASH, GameDefine.CASH_DEFAULT);
        this.AddBooster(BoosterType.CUP, GameDefine.CUP_DEFAULT);
        this.AddBooster(BoosterType.BAG, GameDefine.BAG_DEFAULT);
    }

    public void AddBooster(BoosterType type,long value)
    {
        BoosterCommodity b = this.GetBoosterCommodity(type);
        if(b == null)
        {
            this.boosters.Add(new BoosterCommodity(type, value));
            this.Save();
        }
    }
    public BoosterCommodity AddValueBooster(BoosterType type, long value)
    {
        BoosterCommodity b = this.GetBoosterCommodity(type);
        if (b != null)
        {
            b.Add(value);
            this.Save();
            return b;
        }
        return null;
    }
    public BoosterCommodity AddValueBooster(BoosterCommodity booster)
    {
        BoosterCommodity b = this.GetBoosterCommodity(booster.type);
        if (b != null)
        {
            b.Add(booster.GetValue());
            this.Save();
            return b;
        }
        return null;
    }
    public BoosterCommodity SetValueBooster(BoosterType type, long value)
    {
        BoosterCommodity b = this.GetBoosterCommodity(type);
        if (b != null)
        {
            b.Set(value);
            this.Save();
            return b;
        }
        return null;
    }
    public BoosterCommodity UseBooster(BoosterType type, long value)
    {
        BoosterCommodity b = this.GetBoosterCommodity(type);
        if (b != null)
        {
            if (b.Use(value))
            {
                this.Save();

                return b;
            }
        }
        return null;
    }
    public bool IsHasBooster(BoosterType b)
    {
        return this.boosters.Find(x=>x.type == b) != null;
    }
    public bool IsHasBooster(BoosterType type, long value)
    {
        BoosterCommodity b = this.GetBoosterCommodity(type);
        if (b != null)
        {
            return b.CanUse(value);
        }
        return false;
    }
    public BoosterCommodity GetBoosterCommodity(BoosterType type)
    {
        return this.boosters.Find(x=>x.type == type);
    }

    public void Save()
    {
        GameDataManager.Instance.SaveBoosterData();
    }
}
public enum BoosterType
{
    NONE = -1,
    COIN = 0,
    CASH = 1,
    CUP = 2,

    BAG = 100,

    GLOVE = 200,

    TICKET_TOUR_MONTHLY = 300,
    TICKET_WINSTREAK = 301,
    TICKET_QUICKFIRE = 302,
    TICKET_STARGAME = 303,
    TICKET_TOUR_WEEKLY = 304,

    CUE = 400,
    CUE_CARDS = 401,

    CHALLENGE_TICKET = 500,
}


[System.Serializable]
public class BoosterCommodity
{ 
    public BoosterType type;
    [System.Obsolete("Use GetValue() instead")]
    public string value;
    public BoosterCommodity()
    {
        this.type = BoosterType.NONE;
        this.value = "0";// why not set constant? cuz constant can be decoded easily
    }
    public BoosterCommodity(BoosterType key, long value)
    {
        this.type = key;
        this.value = (value).ToString();
    }

    public BoosterCommodity(BoosterType key, long value, int id)
    {
        this.type = key;
        this.value = (value).ToString();
    }
    public void Set(long value)
    {
        this.value = (value).ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void Add(long value)
    {
        value += long.Parse(this.value);
        if (value < 0) value = 0;
        this.value = (value).ToString();
    }

    public void Add(string otherBoosterValue)
    {
        long v = long.Parse(otherBoosterValue) + long.Parse(this.value);
        if (v < 0) v = 0;
        this.value = v.ToString();
    }

    public bool Use(long value)
    {
        long v = long.Parse(this.value);
        if (v >= value)
        {
            v -= value;
            if (v < 0) v = 0;
            this.value = (v).ToString();
            return true;
        }
        return false;
    }
    public bool CanUse(long value)
    {
        long v = long.Parse(this.value);
        return v >= value;
    }

    public long GetValue()
    {
        return long.Parse(this.value);
    }

}
