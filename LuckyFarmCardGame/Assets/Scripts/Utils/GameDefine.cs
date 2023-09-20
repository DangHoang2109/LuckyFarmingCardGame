using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefine
{
    public const string USER_BOOSTER_DATA = "USER_BOOSTER_DATA";
    public const string USER_INFO_DATA = "USER_INFO_DATA";
    public const string USER_RATED = "USER_RATED"; //user đã rate (1-4 star hoặc 5 star) sẽ không hiện rate lại nữa

    public const long COIN_DEFAULT = 15000;

    public const long CASH_DEFAULT = 50;
    public const long CUP_DEFAULT = 0;
    public const long BAG_DEFAULT = 0;

    public const long COIN_WATCH_FREE = 500;
    public const double TIME_FREE_COIN = 300f; //thời gian đợi free coin

    public const double TIME_FREE_BAG = 14400f; //thời gian đợi mở bag free
    public const float TIME_OPEN_BAG_END = 5.8f; //5 giay
    
    public const double TIME_TOTAL_A_DAY = 86400; //tổng số giây của 24h

    public const int NUM_NEWBIE_MATCH = 3; // số ván bot thả cho player toàn thắng khi mới tạo acc, đếm từ 0 => X + 1 ván tổng cộng
    public const float AFFECT_WINRATE_BY_STAT = 0.05f; //Mỗi stat main player thua bot khiến winrate tổng bị hạ xuống 5%

    public const long RATE_CASH_TO_COIN = 1500;
    public const long RATE_DOLLAR_TO_CASH = 100;

    public const long RATE_DOLLAR_TO_COIN = RATE_CASH_TO_COIN * RATE_DOLLAR_TO_CASH;

    public const int ROOM_FRIST_AI = 0;
    public const int ROOM_PRACTICLE = -1;
    public const int AMOUNT_GAMES_BOOST_MAIN = 4;//7;
    public const float DIVIDER_IN_GAMES_BOOST_MAIN = 3;

    public const string ID_CUE_DEFAULT = "default";
    public const string DISCORD_LINK = "https://discord.gg/S8EAPvGvFj";
}

public static class SceneName
{
    public const string LOADING = "LoadingScene";
    public const string HOME = "HomeScene";
    public const string GAME = "PoolGame";
}

/// <summary>
/// Thứ tự layer được sắp xếp từ thấp đến cao
/// </summary>
public static class LayerName
{
    public const string Scene = "Scene";
    public const string Game = "Game";
    public const string Popup = "Popup";
}
public static class SocialDefine
{
    public const string FAN_PAGE = "https://www.facebook.com/cosina.games";
    public const string TERNS_OF_SERVICE = "https://cosinagames.github.io/policy.html";
    public const string POLICY = "https://cosinagames.github.io/policy.html";
    public const string SUPPORT = "https://www.facebook.com/cosina.games";
}