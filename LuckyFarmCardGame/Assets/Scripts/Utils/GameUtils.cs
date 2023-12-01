using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class GameUtils
{
    public const string KEY_CONCENT_IRONSOURCE = "KEY_CONCENT_IRONSOURCE";
    public const long QUADRILLION =  1000000000000000; 
    public const long QUADRILLION_DIVIDE_10 =  QUADRILLION / 10; 
    public const long TRILLION =  1000000000000; 
    public const long TRILLION_DIVIDE_10 =  TRILLION / 10; 
    public const long BILLION =   1000000000; 
    public const long BILLION_DIVIDE_10 =   BILLION / 10; 
    public const long MILLION =   1000000; 
    public const long MILLION_DIVIDE_10 =   MILLION / 10; 
    public const long THOUSAND =  1000; 
    public const long THOUSAND_DIVIDE_10 =  THOUSAND / 10; 
    
    // 1000 1000000 1000000000 1000000000000
    public static readonly string[] DIGIT_LETTERS = { "K", "M", "B", "T", "P" };

    public static string ToStringFormated(this float value)
    {
        return  value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
    }
    public static int GetWeekOfYear(this DateTime time, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        ///https://stackoverflow.com/questions/2136487/calculate-week-of-month-in-net
        ///
        System.Globalization.GregorianCalendar _gc = new System.Globalization.GregorianCalendar();

        return _gc.GetWeekOfYear(time, System.Globalization.CalendarWeekRule.FirstDay, firstDayOfWeek);
    }

    public static float Random => UnityEngine.Random.Range(0f, 1f);
    public static T Last<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }
    public static string ConvertFloatToTimeLargestTwoUnit(double second)
    {
        //if (second >= TimeSpan.MaxValue.TotalSeconds)
        //{
        //    CosinaVeryBigTimeSpan timeVeryBig = CosinaVeryBigTimeSpan.FromSeconds(second);
        //    return timeVeryBig.ToStringBig2Digit();
        //}

        string format = second < 3600 ? "m\\m' 'ss\\s" : "h\\h' 'mm\\m";
        if (second >= 86400)
            format = "d\\d' 'h\\h";

        return ConvertFloatToTime(second, format);
    }
    public static T GetRandom<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            return default;

        return array[UnityEngine.Random.Range(0, array.Length)];
    }
    public static T GetRandom<T>(this List<T> array)
    {
        if (array == null || array.Count == 0)
            return default;

        return array[UnityEngine.Random.Range(0, array.Count)];
    }
    
    public static T GetRandomSafe<T>(this List<T> array)
    {
        if (array == null || array.Count == 0)
            return default;

        return array[UnityEngine.Random.Range(0, array.Count)];
    }
    /// <summary>
    /// RETURN: empty list | list equal to or less than 'takeCount' picked randomly
    /// </summary>
    public static List<T> GetRandomSafe<T>(this List<T> array, int takeCount)
    {
        List<T> result = new List<T>(takeCount);
        if (array == null || array.Count == 0)
            return result;

        // if the list has not many elements, we shuffle it then take some.
        if(array.Count <= (takeCount * 2))
        {
            result.AddRange(array);
            result.Shuffle();
            return result.Take(takeCount).ToList();
        }

        List<int> indexes = new List<int>(takeCount);
        int r;
        for(int i = 0; i < takeCount; ++i)
        {
            do
            {
                r = UnityEngine.Random.Range(0, array.Count);
            }
            while (indexes.Contains(r));
            indexes.Add(r);
            result.Add(array[r]);
        }

        return result;
    }
    /// <summary>
    /// RETURN: List empty | IEnum equal to or less than 'takeCount' picked randomly
    /// </summary>
    public static IEnumerable<T> QueryRandom<T>(this IEnumerable<T> source, int takeCount)
    {
        if (source == null || !source.Any())
            return new List<T>();

        List<T> result = new List<T>(source);
        result.Shuffle();
        return result.Take(takeCount);
    }
    
    public static string DebugList<T>(this IList<T> list)
    {
        string r = "";
        for (int i = 0; i < list.Count; i++)
        {
            r = string.Format("{0} {1}", r, list[i]);
        }

        return r;
    }
    public static string AppendString(string baseString, params string[] addStrings)
    {
        for (int i = 0; i < addStrings.Length; i++)
        {
            baseString = string.Format("{0}{1}", baseString, addStrings[i]);
        }

        return baseString;
    }
    /// <summary>
    /// it is not really correctly (the sub value) but it works reliably, far low risk than the former function <br></br>
    /// WARNING! Not parse negative number  (it is not hard but I just don't like to do it :D )
    /// </summary>
    public static string FormatMoney(long num)
    {
       
        long pre, sub;
        string key;

        // if else looks ridiculous but it's performance is sure better than conversion to string
        //      then count then extract string, and it is not reliably if you convert back to the number 
        if (num >= QUADRILLION)
        {
            pre = num / QUADRILLION;
            sub = (num % QUADRILLION) / QUADRILLION_DIVIDE_10;
            key = DIGIT_LETTERS[4];
        }
        else if (num >= TRILLION)
        {
            pre = num / TRILLION;
            sub = (num % TRILLION) / TRILLION_DIVIDE_10;
            key = DIGIT_LETTERS[3];
        }
        else if (num >= BILLION)
        {
            pre = num / BILLION;
            sub = (num % BILLION) / BILLION_DIVIDE_10;
            key = DIGIT_LETTERS[2];
        }
        else if (num >= MILLION)
        {
            pre = num / MILLION;
            sub = (num % MILLION) / MILLION_DIVIDE_10;
            key = DIGIT_LETTERS[1];
        }
        else if (num >= THOUSAND)
        {
            pre = num / THOUSAND;
            sub = (num % THOUSAND) / THOUSAND_DIVIDE_10;
            key = DIGIT_LETTERS[0];
        }
        else
        {
            pre = num;
            sub = 0;
            key = string.Empty;
        }

        if (sub == 0)
        {
            return pre.ToString() + key;
        }
        else
        {
            return pre.ToString() + $".{sub}{key}";
        }
    }

    public static string FormatDot(int point, string separator = ".")
    {
        bool isNegative = false;
        if (point < 0)
        {
            point = -point;
            isNegative = true;
        }

        string result = point.ToString();
        int index = result.Length - 1;
        int split = 0;
        while (index > 0)
        {
            split++;
            if (split % 3 == 0)
            {
                result = result.Insert(index, "" + separator);
                split = 0;
            }
            index--;
        }
        if (isNegative)
            result = "-" + result;
        return result;
    }
    public static string FormatMoneyDot(long money, string separator = ".")
    {
        bool isNegative = false;
        if (money < 0)
        {
            money = -money;
            isNegative = true;
        }

        string result = money.ToString();
        int index = result.Length - 1;
        int split = 0;
        while (index > 0)
        {
            split++;
            if (split % 3 == 0)
            {
                result = result.Insert(index, "" + separator);
                split = 0;
            }
            index--;
        }
        if (isNegative)
            result = "-" + result;
        return result;
    }
    public static long GetCurrentTimeSeconds()
    {
        Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        return (long)unixTimestamp;
    }

    public static string ConvertFloatToTime(double second, string format = "hh':'mm':'ss")
    {
        double totalSeconds = second;
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

       
        return time.ToString(format);
    }
    public static TimeSpan ConvertFloatToTimeSpan(double second)
    {
        double totalSeconds = second;
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);


        return time;
    }
    public static DateTime UnixTimeToDateTime(long unixtime)
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixtime);
        return dtDateTime;
    }
    public static long DateTimeToUnixTime(DateTime time)
    {
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (time.ToUniversalTime() - unixStart).Ticks;
        return (long)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
    }
    public static long ConvertValue_Cash_To_Coin(long cash, float bonus = 1f)
    {
        return RoundUpValue((long)(bonus * (cash) * GameDefine.RATE_CASH_TO_COIN), (long)100);
    }

    /// <summary>
    /// Convert giá trị coin thành cash
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="minReturn">Thấp nhất return ra giá trị này, khi lượng amount yêu cầu bé hơn 1 rate</param>
    /// <returns></returns>
    public static long ConvertValue_Coin_To_Cash(long coin, long minReturn = 1)
    {
        return coin / GameDefine.RATE_CASH_TO_COIN < minReturn ? minReturn : coin / GameDefine.RATE_CASH_TO_COIN;
            //RoundValue(coin / GameDefine.RATE_CASH_TO_COIN, 0);
    }

    public static long ConvertValue_Coin_To_Dollar(long coin)
    {
        return RoundValue(coin / GameDefine.RATE_DOLLAR_TO_COIN, 0);
    }

    public static long ConvertValue_Cash_To_Dollar(long cash)
    {
        return RoundValue(cash / GameDefine.RATE_DOLLAR_TO_CASH, 0);
    }

    public static long ConvertValue_Dollar_To_Cash(long dollar, float bonus = 1f)
    {
        return RoundUpValue((long)(bonus * (RoundUpValue(dollar, 0)) * GameDefine.RATE_DOLLAR_TO_CASH), (long)50);
    }

    public static long ConvertValue_Dollar_To_Coin(long dollar, float bonus = 1f)
    {
        return RoundUpValue((long)(bonus * (RoundUpValue(dollar, 0)) * GameDefine.RATE_DOLLAR_TO_COIN), (long)100);
    }

    public static Vector3 ConvertPosInCanvasToRatio(Vector3 pos, Vector3 posLeftBot, Vector3 posRightTop)
    {
        Vector3 ratio = Vector3.zero;
        ratio.x = (pos.x - posLeftBot.x) / (posRightTop.x - posLeftBot.x);
        ratio.y = (pos.y - posLeftBot.y) / (posRightTop.y - posLeftBot.y);
        return ratio;
    }

    public static Vector3 ConvertRatioInCanvasToPos(Vector3 ratio, Vector3 posLeftBot, Vector3 posRightTop)
    {
        Vector3 pos = Vector3.zero;
        pos.x = ratio.x * (posRightTop.x - posLeftBot.x) + posLeftBot.x;
        pos.y = ratio.y * (posRightTop.y - posLeftBot.y) + posLeftBot.y;
        return pos;
    }

    #region Number
    public static long RoundUpValue(long value, long divder)
    {
        long addOn = (value % divder == 0 ? 0 : divder - value % divder);
        return value + addOn;
    }
    public static long RoundUpValue(long value, int places)
    {
        long bound = (long)Mathf.Pow(10, places);

        //check nếu value không lớn hơn hệ số mũ places => lỗi, làm tròn tới places gần nhất
        long addOn = (value % bound == 0 ? 0 : bound - value % bound);
        return value + addOn;
    }
    public static int RoundUpValue(int value, int places)
    {
        int bound = (int)Mathf.Pow(10, places);

        //check nếu value không lớn hơn hệ số mũ places => lỗi, làm tròn tới places gần nhất
        int addOn = (value % bound == 0 ? 0 : bound - value % bound);
        return value + addOn;
    }
    //Làm tròn, lên hay xuống tùy theo phần dư
    public static long RoundValue(long value, int places)
    {
        long bound = (long)Mathf.Pow(10, places);

        long exceed = value % bound;

        if (exceed == 0) return value;

        if (exceed < bound / 2)
            return value - exceed;

        if (exceed > bound / 2)
            return value + (bound - exceed);

        return value;
    }
    /// <summary>
    /// Alway random no matter a < b or b < a
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int SafeRandom(int a, int b, bool isExcludeMax = true)
    {
        if (a == b)
            return a;
        else if (a < b)
            return UnityEngine.Random.Range(a, isExcludeMax ? b : b + 1);
        else
            return UnityEngine.Random.Range(b, isExcludeMax ? a : a + 1);
    }

    public static long RoundValue(float value, int places)
    {
        long longConverted = (long)value;
        //convert về long gần nhất rồi round như long
        float frac = value - longConverted;

        if (frac >= 0.5) longConverted++;

        return RoundValue(longConverted, places);
    }

    public static long RoundDownValue(long value, int places)
    {
        long bound = (long)Mathf.Pow(10, places);

        //check nếu value không lớn hơn hệ số mũ places => lỗi, làm tròn tới places gần nhất
        long addOn = (value % bound == 0 ? 0 : value % bound);
        return value - addOn;
    }
    #endregion
    
    public static float ConvertLongToFloat(long value)
    {
        float result = 0f;
        if (value > float.MaxValue)
        {
            result = float.MaxValue;
        }
        else
        {
            result = (float) value;
        }
        return result;
    }
    public static int ConvertLongToInt(long value)
    {
        int result = 0;
        if (value > int.MaxValue)
        {
            result = int.MaxValue;
        }
        else
        {
            result = (int) value;
        }
        return result;
    }


    public static bool CheckInstallSource()
    {
        string installSource = Application.installerName;
        if (!installSource.Contains("com.android.vending")) {
            //show dialog thông báo và click ok thì mở tới playstore của game.
            //Cài ở store ngoài
            return false;
        }

        return true;
    }


#if UNITY_EDITOR
    public static List<T> ParseEnumToList<T>()
    {
        return Enum.GetValues(typeof(T))
                          .Cast<T>()
                          .ToList<T>();
    }
    public static List<String> LoadAllNameFolderInFolder(string path)
    {
        if (path != "")
        {
            if (path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }
        }
        List<GameObject> results = new List<GameObject>();


        DirectoryInfo dirInfo = new DirectoryInfo(path);

        DirectoryInfo[] folders = dirInfo.GetDirectories();
        if (folders.Length > 0)
        {
            List<string> _allFilePaths = new List<string>();
            foreach (DirectoryInfo folder in folders)
            {
                _allFilePaths.Add(folder.Name);
            }
            return _allFilePaths;
        }

        return null;
    }

    public static List<T> LoadAllAssetsInFolder<T>(string path, List<string> patterns) where T : UnityEngine.Object
    {
        if (path != "")
        {
            if (path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }
        }
        List<T> results = new List<T>();


        DirectoryInfo dirInfo = new DirectoryInfo(path);

        DirectoryInfo[] folders = dirInfo.GetDirectories();
        if (folders.Length > 0)
        {
            List<string> _allFilePaths = new List<string>();
            foreach (DirectoryInfo folder in folders)
            {
                foreach (var item in patterns)
                {
                    string[] _temp = Directory.GetFiles(folder.FullName, item, SearchOption.AllDirectories);
                    _allFilePaths.AddRange(_temp);
                }
            }

            foreach (var item in _allFilePaths)
            {
                string fullPath = item.Replace(@"\", "/");
                string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");

                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath) as T;

                if (asset != null)
                {
                    results.Add(asset);
                }
            }
        }
        else
        {
            List<string> _allFilePaths = new List<string>();
            foreach (var item in patterns)
            {
                string[] _temp = Directory.GetFiles(path, item, SearchOption.AllDirectories);
                _allFilePaths.AddRange(_temp);
            }
            foreach (var item in _allFilePaths)
            {
                string fullPath = item.Replace(@"\", "/");
                //string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");

                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath) as T;

                if (asset != null)
                {
                    results.Add(asset);
                }
            }
        }

        return results;
    }
    public static T LoadAssetInFolder<T>(string path, string extention) where T : UnityEngine.Object
    {
        string fullPath = path + extention;
        T results = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);

        return results;
    }

    public static List<T> LoadAllAssetInFolder<T>(string path, string extention) where T : UnityEngine.Object
    {
        List<T> results = new List<T>();

        string[] temps = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach(string t in temps)
        {
           
            T result = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(t);
            if (result != null)
            {

                results.Add(result);
            }
        }

        return results;
    }

    public static void ClearLogConsole()
    {
#if UNITY_EDITOR
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
        
#endif
    }

#endif
}

public static class InGameUtils
{
    public static int RollIndex(int currentIndex, int maxIndexExclude, int minIndex = 0)
    {
        currentIndex++;
        if (currentIndex >= maxIndexExclude)
            currentIndex = minIndex;
        return currentIndex;
    }

    public static string DebugListCardInGame(this List<InGame_CardDataModelLevels> cards)
    {
        string debug = "";
        for (int i = 0; i < 6; i++)
        {
            InGame_CardDataModelLevels c = cards.Find(x => x._cardID == i);
            if (c != null)
            {
                debug = string.Format($"{debug}Card {i} - {c._currentCard} - {c._currentLevel} \n");
            }
            else
            {
                debug = string.Format($"{debug}Card {i} - {0} - {1}\n");
            }
        }
        UnityEngine.Debug.Log(debug);
        return debug;
    }
    public static string DebugDicCardInGame(this Dictionary<int, InGame_CardDataModelLevels> cards)
    {
        string debug = "";
        for (int i = 0; i < 6; i++)
        {
            if (cards.TryGetValue(i, out InGame_CardDataModelLevels c))
            {
                debug = string.Format($"{debug}Card {i} - {c._currentCard} - {c._currentLevel} \n");
            }
            else
            {
                debug = string.Format($"{debug}Card {i} - {0} - {1}\n");
            }
        }
        UnityEngine.Debug.Log(debug);
        return debug;
    }
    public static InGameBaseCardEffectActivator CreateCardEffectActivator(InGameBaseCardEffectID skillID,int cardID, InGameBasePlayerItem host, InGame_CardDataModel cardModel)
    {
        try
        {
            InGameBaseCardEffectActivator _effectActivator = Activator.CreateInstance(EnumUtility.GetStringType(skillID)) as InGameBaseCardEffectActivator;
            _effectActivator?.SetIDAndHost(cardID, host, cardModel);
            return _effectActivator;
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
            throw;
        }
    }
    public static InGame_CardDataModel CreateCardDataModel(int id)
    {
        InGameCardConfig cardConfig = InGameCardConfigs.Instance.GetCardConfig(id);
        int currentCardLevel = InGameManager.Instance.GetPlayerCardCurrentLevel(id);

        return new InGame_CardDataModel().SetHost(InGameManager.Instance.MainUserPlayer).SetCardID(id, cardConfig).SetCurrentLevel(currentCardLevel);
    }
}