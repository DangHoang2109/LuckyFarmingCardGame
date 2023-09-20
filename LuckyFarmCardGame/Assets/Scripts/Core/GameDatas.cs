using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDatas
{
    public UserDatas userDatas;

    /// <summary>
    /// Dùng để tính thời gian offline khi user login
    /// </summary>
    public long lastTimeOnline;

    public static GameDatas Instance
    {
        get
        {
            return GameDataManager.Instance.GameDatas;
        }
    }

    public GameDatas()
    {
    }
    public static GameDatas Create()
    {
        return new GameDatas()
        {
            userDatas = UserDatas.Create(),
        };
    }
    /// <summary>
    /// Gọi khi user lần đầu vào game
    /// Tạo 1 user
    /// </summary>
    public void CreateUser()
    {
        this.userDatas.CreateUser();
        CreateNoAdsUser();
    }

    public void SetTimeWhenUserLosrFocus(bool save = true)
    {
        long currentTimeStamp = System.DateTime.Now.ToFileTime();
        lastTimeOnline = currentTimeStamp;

        this.userDatas.OnFocusApp();

        if(save)
            Save();
    }
    public double GetTotalSecondOffline()
    {
        if (lastTimeOnline <= 0)
            return 0;

        System.DateTime data = System.DateTime.FromFileTime(lastTimeOnline);
        if (data != null)
        {
            System.TimeSpan _timeSpanDIf = System.DateTime.Now.Subtract(data);
            double timeOffline = System.Math.Max(_timeSpanDIf.TotalSeconds, 0); //nếu second âm thì trả về 0

            return timeOffline;
        }
        return 0;
    }

    /// <summary>
    /// Gọi khi mở game
    /// </summary>
    public void OpenGame()
    {
        double totalOffline = GetTotalSecondOffline();
        Debug.Log($"Total time offine {totalOffline}");

        this.userDatas.OpenGame(totalOffline);
    }


    #region No Ads Membership
    //======== NO ADS PACKAGE ============

    private System.Action<bool> OnBuyNoAds;

    public bool isTrialed;

    public const double TIME_TRIAL_NOADS = 3 * 24 * 60 * 60; //3d

    public double _timeNoAds;
    public long _datePurchaseNoAdsPackage;
    public bool isPermanentNoAds;

    //[JsonIgnore]
    public bool IsNoAdsMembership =>
        //!CPSocialManager.Instance.IsBanned &&
        (isPermanentNoAds || CheckTimeMembership(out _));// || IddleGameManager.Instance.CurrentGameData.IsThisEventNoAds);

    private void CreateNoAdsUser()
    {
        _timeNoAds = -1;
        isPermanentNoAds = false;
        isTrialed = false;
    }

    public void AssignCallbackNoAds(System.Action<bool> onBuyNoAds)
    {
        this.OnBuyNoAds += onBuyNoAds;
    }
    public void UnAssignCallbackNoAds(System.Action<bool> onBuyNoAds)
    {
        this.OnBuyNoAds -= onBuyNoAds;
    }
    /// <summary>
    /// WARNING: YOU SHOULD BE CAREFULL. time == -1 mean this package is permanent membership
    /// </summary>
    /// <param name="timeMembership"></param>
    public void BuyAllNoAdsMembership(double timeMembership = -1)
    {
        if (timeMembership == -1)
        {
            Debug.Log("THIS USER IS BUYING NO ADS PERMANENT MEMBERSHIP");

            //if (!isPermanentNoAds)
            //    LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.BECOME_NOADS_PERMANENT);

            this.isPermanentNoAds = true;
        }
        else
        {
            if (!this.isPermanentNoAds)
            {
                this._timeNoAds += timeMembership;
                this._datePurchaseNoAdsPackage = DateTime.Now.ToFileTime();

                //LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.BECOME_NOADS_WITH_TIME);
            }
        }
        OnBuyNoAds?.Invoke(true);
        Save();
    }

    public bool CheckTimeMembership(out double _timeRemainNoAds)
    {
        if (_timeNoAds <= 0)
        {
            _timeRemainNoAds = 0;
            return false;
        }

        DateTime _datePurchase = DateTime.FromFileTime(_datePurchaseNoAdsPackage);
        DateTime _dateNow = DateTime.UtcNow;

        TimeSpan difference = _dateNow.Subtract(_datePurchase);

        _timeRemainNoAds = difference.TotalSeconds;

        ///Lỗi user set system time lùi
        if (_timeRemainNoAds <= 0)
            return false;

        return _timeNoAds - difference.TotalSeconds > 0;
    }

    public void RemoveNoAdsMembership(bool isRemovePermanenr = false)
    {
        this._timeNoAds = -1;
        if (isRemovePermanenr)
            this.isPermanentNoAds = false;


        if (!this.IsNoAdsMembership)
        {
            //hết toàn bộ no ads
            //foreach (UserGameData d in this.dicDatas.Values)
            //{
            //    d.Booster.OnUserRemoveNoAdsAll();
            //}
        }
        Save();
    }
    //======== NO ADS PACKAGE ============
    #endregion

    public void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }
}
