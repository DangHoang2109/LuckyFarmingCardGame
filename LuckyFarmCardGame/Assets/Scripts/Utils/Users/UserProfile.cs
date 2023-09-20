using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CallbackEventObject : UnityEvent<object>
{

}
public class CallbackEventInt : UnityEvent<int>
{

}
public class CallbackEventLong : UnityEvent<long>
{

}
public class CallbackEventBooster : UnityEvent<BoosterCommodity>
{

}
public class UserProfile : MonoSingleton<UserProfile>
{
    private Dictionary<BoosterType, CallbackEventBooster> boostes;
    private Dictionary<string, CallbackEventBooster> callbackProfiles;


    public override void Init()
    {
        base.Init();
        this.boostes = new Dictionary<BoosterType, CallbackEventBooster>();
        this.callbackProfiles = new Dictionary<string, CallbackEventBooster>();
    }

    
    
    #region CALLBACK BOOSTER
    public void AddCallbackBooster(BoosterType type, UnityAction<BoosterCommodity> callback)
    {
        if (this.boostes.ContainsKey(type))
        {
            this.boostes[type].AddListener(callback);
        }
        else
        {
            CallbackEventBooster unityEvent = new CallbackEventBooster();
            unityEvent.AddListener(callback);
            this.boostes.Add(type, unityEvent);
        }
        BoosterCommodity booster = UserBoosters.Instance.GetBoosterCommodity(type);
        if (booster!=null)
            this.ChangeBoosterValue(booster.type, booster);
    }

    public void RemoveCallbackBooster(BoosterType type, UnityAction<BoosterCommodity> callback)
    {
        if (this.boostes.ContainsKey(type))
        {
            this.boostes[type].RemoveListener(callback);
        }
    }
    // invoke callbacks
    public void ChangeBoosterValue(BoosterType type, BoosterCommodity booster)
    {
        if (this.boostes.ContainsKey(type))
        {
            this.boostes[type].Invoke(booster);
        }
    }
    #endregion
    
     #region BOOSTER

     
     public BoosterCommodity SetBooster(BoosterType type, long value, string where, int level = 1)
     {
         BoosterCommodity commodity = UserBoosters.Instance.SetValueBooster(type, value);
         if (commodity != null)
         {
             this.ChangeBoosterValue(commodity.type, commodity);
             
         }
         return commodity;
     }

     /// <summary>
     /// Source: booster nào đó
     /// </summary>
     /// <param name="booster">Loại booster</param>
     /// <param name="where">Vị trí add(wheel,game, ...)</param>
     /// <param name="level"></param>
     /// <returns></returns>
    public BoosterCommodity AddBooster(BoosterCommodity booster, string from, string where,bool isCallback = true, int level = 0)
    {
        BoosterCommodity commodity = UserBoosters.Instance.AddValueBooster(booster);
        if (commodity != null)
        {
            if (isCallback)
            {
                this.ChangeBoosterValue(commodity.type, commodity);
            }

            //UserBehaviorDatas.Instance.SourceInCome(booster.type, booster.GetValue(), where);
            //UserBehaviorDatas.Instance.LogResourceFirebase(SourceType.Source, from, booster.type.ToString(), booster.type.ToString(), booster.GetValue(), commodity.GetValue(), where);

        }
        return commodity;
    }
     /// <summary>
     /// Source: booster nào đó
     /// </summary>
     /// <param name="type">Loại booster</param>
     /// <param name="value">Gia tri booster</param>
     /// <param name="where">Vị trí add(wheel,game, ...)</param>
     /// <param name="level"></param>
     /// <returns></returns>
    public BoosterCommodity AddBooster(BoosterType type, long value, string from, string where, bool isCallback = true, int level = 1)
    {
        BoosterCommodity commodity = UserBoosters.Instance.AddValueBooster(type, value);
        if (commodity != null)
        {
            if (isCallback)
            {
                this.ChangeBoosterValue(commodity.type, commodity);
            }

            //UserBehaviorDatas.Instance.SourceInCome(type, value,where);
            //UserBehaviorDatas.Instance.LogResourceFirebase(SourceType.Source, from, type.ToString(), type.ToString(), value, commodity.GetValue(), where);

        }
        return commodity;
    }

     int LongToInt(long value)
     {
         if (value > int.MaxValue)
         {
             return int.MaxValue;
         }

         return (int) value;
     }
     /// <summary>
     /// Source: booster nào đó
     /// </summary>
     /// <param name="boosters">Loại booster</param>
     /// <param name="where">Vị trí add(wheel,game, ...)</param>
     /// <param name="level"></param>
     /// <returns></returns>
    public List<BoosterCommodity> AddBoosters(List<BoosterCommodity> boosters, string from, string where, bool isCallback = true, int level = 1)
    {
        List<BoosterCommodity> temps = new List<BoosterCommodity>();
        foreach (var b in boosters)
        {
            temps.Add(this.AddBooster(b, from, where, isCallback, level));
        }

        return temps;
    }
     /// <summary>
     /// SINK: booster nào đó
     /// </summary>
     /// <param name="type">Loại booster</param>
     /// <param name="value">Gia tri booster</param>
     /// <param name="where">Vị trí use(wheel,game, ...)</param>
     /// <param name="level"></param>
     /// <returns></returns>
    public bool UseBooster(BoosterType type, long value, string from, string where,bool isCallback = true, int level = 1)
    {
        BoosterCommodity after = UserBoosters.Instance.GetBoosterCommodity(type);
        BoosterCommodity commodity = UserBoosters.Instance.UseBooster(type, value);
        if (commodity != null)
        {
            if (isCallback)
            {
                this.ChangeBoosterValue(commodity.type, commodity);
            }
            //UserBehaviorDatas.Instance.SourceOutCome(type, value, where);
            //FalconLogAnalytics.ResourceLog(this.GetCurrentMatch(), 1, type.ToString(), type.ToString(), type.ToString(), this.LongToInt(value));

            //if (commodity.type == BoosterType.CASH)
            //    UserBehaviorDatas.Instance.UseGem(value);


            return true;
        }
        return false;
    }

     public bool UseBooster(BoosterCommodity booster, string from, string where, bool isCallback = true, int level = 1)
     {
         return this.UseBooster(booster.type, booster.GetValue(), from, where, isCallback, level);
     }
     /// <summary>
     /// Source: booster nào đó
     /// </summary>
     /// <param name="boosters">Loại booster</param>
     /// <param name="where">Vị trí add(wheel,game, ...)</param>
     /// <param name="level"></param>
     /// <returns></returns>
    public bool UseBoosters(List<BoosterCommodity> boosters,string from, string where, bool isCallback = true, int level = 1)
    {
        if (this.IsCanUseBoosters(boosters))
        {
            foreach (BoosterCommodity booster in boosters)
            {
                this.UseBooster(booster.type, booster.GetValue(), from, where, isCallback, level);
            }
            return true;
        }
        return false;
    }

    public bool IsCanUseBooster(BoosterCommodity booster)
    {
        return IsCanUseBooster(booster.type, booster.GetValue());
    }

    public bool IsCanUseBooster(BoosterType type, long value)
    {
        return UserBoosters.Instance.IsHasBooster(type, value);
    }
    public bool IsCanUseBoosters(List<BoosterCommodity> boosters)
    {
        foreach (BoosterCommodity booster in boosters)
        {
            if (!this.IsCanUseBooster(booster.type, booster.GetValue()))
            {
                return false;
            }
        }
        return true;
    }

    public long GetValueBooster(BoosterType type)
    {
        BoosterCommodity b = UserBoosters.Instance.GetBoosterCommodity(type);
        if (b != null)
            return b.GetValue();
        return 0;
    }
    #endregion
}

public static class LogSourceWhere
{
    public const string COIN_WIN_GAME = "Coin_Win_Game";
    public const string COIN_DOUBLE_WIN_GAME = "Double_Win_Game";
    public const string COIN_FREE_COIN = "Free_Coin";
    public const string SHOP_BUY = "Shop_Buy";
    public const string OPEN_BAG = "Open_Bag";
    public const string OPEN_BAG_END = "Open_Bag_EndGame";
    public const string SHOP_FREE_COIN = "Shop_Free_Coin";
    public const string SHOP_BUY_PACKAGE = "Shop_Buy_Package";
    
    public const string BAG_WIN_GAME = "Bag_Win_Game";
    public const string NEED_MORE_COIN = "Need_More_Coin";

    public const string BATTLE_PASS_FREE = "Battle_Pass_Free";
    public const string BATTLE_PASS_PRO = "Battle_Pass_Pro";
    public const string OPEN_BAG_BULLEYE = "Open_Bag_Bulleye";
    public const string OPEN_BAG_WHEEL = "Open_Bag_Wheel";

    public const string WHEELWINBOOSTER = "Wheel_Win_Booster";
    public const string CLAIM_PRO_FREEENTRY = "Claim_Pro_FreeEntry";

    public const string OPEN_BAG_TOURNAMENT = "Open_Bag_Tournamente";

    public const string COIN_WIN_ROUND_TOURNAMENT = "COIN_WIN_ROUND_TOURNAMENT";

    public const string COIN_WIN_TOURNAMENT = "COIN_WIN_TOURNAMENT";
    public const string GEM_WIN_TOURNAMENT = "GEM_WIN_TOURNAMENT";

    public const string COIN_WINSTEAK = "Coin_WinSteak";
    public const string GEM_WINSTEAK = "Gem_WinSteak";
    public const string BAG_WINSTEAK = "Bag_WinSteak";
    public const string GLOVE_WINSTEAK = "Glove_WinSteak";

    public const string ADS_BONUS = "Ads_Bonus";


    public const string TOURNAMENT_MONTHLY = "TOURNAMENT_MONTHLY";
    public const string QUICKFIRE_RANKING = "QUICKFIRE_RANKING";
    public const string STAREVENT_RANKING = "STAREVENT_RANKING";


    public const string PASS = "PASS";
    public const string PASS_QUICKFIRE = "PASS_QUICKFIRE";
    public const string PASS_STAREVENT = "PASS_STAREVENT";

}

public class LogSinkWhere
{
    public const string OPEN_BAG = "Open_Bag";
    public const string JOIN_ROOM = "Play_Room";
    public const string UPGRADE_CARD = "Upgrade_Card";
    public const string SHOP_BUY = "Shop_Buy";
    public const string SHOP_BUY_BAG = "Shop_Buy_Bag";
    public const string SHOP_BUY_CUE = "Shop_Buy_Cue";
    public const string SHOP_BUY_CARD = "Shop_Buy_Card";
    public const string SHOP_BUY_DEAL_CUE = "Shop_Buy_Deal_Cue";
    public const string NEED_MORE_COIN = "Need_More_Coin";
    public const string NEED_MORE_CARD = "Need_More_Card";


    public const string BUY_CHARACTER = "Buy_Character";
    public const string BUY_SKIN = "Buy_Skin";

    public const string JOIN_BULLEYE = "Join_Bulleyes";
    public const string CONTINUE_QUICKFIRE = "Continue_Quickfire";

    public const string BUY_VIP = "Buy_Vip";
    public const string BUY_VIP_FULL = "Buy_Vip_Full";

    public const string PLAY_TOURNAMENT = "PLAY_TOURNAMENT";
    public const string TRYAGAIN_TOURNAMENT = "TRYAGAIN_TOURNAMENT";

    public const string START_WINSTEAK = "Start_WinSteak";
    public const string PLAY_WINSTEAK = "Play_WinSteak";
    public const string RESET_WINSTEAK = "Reset_WinSteak";
    public const string CONTINUE_WINSTEAK = "Continue_WinSteak";
}