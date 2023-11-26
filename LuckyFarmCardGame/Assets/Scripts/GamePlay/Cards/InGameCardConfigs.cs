using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "Configs/InGameCardConfigs", fileName = "InGameCardConfigs")]
public class InGameCardConfigs : ScriptableObject
{
    #region Singleton
    private static InGameCardConfigs _instance;

    public static InGameCardConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<InGameCardConfigs>("Configs/Games/InGameCardConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    [TabGroup("Card Info")]
    [TableList]
    public List<InGameCardConfig> _configs;
    //[TabGroup("Skill Info")]
    //public InGameCardEffectConfigs _skillConfigs;
    [TabGroup("Card Level Upgrade")]
    public InGameCardLevelsConfigs _levelsConfigs;
    #endregion Data Prop

    #region Getter
    public InGameCardConfig GetCardConfig(int id)
    {
        return _configs.Find(x => x._cardID == id);
    }
    public List<InGameCardConfig> GetAllBonusCardConfigs()
    {
        return new List<InGameCardConfig>(_configs.FindAll(x => x._isBonusTier));
    }
    #endregion Getter

#if UNITY_EDITOR
    #region VALIDATE
    private void OnValidate()
    {
        foreach (var item in _levelsConfigs._configs)
        {
            for (int i = 0; i < item.levels.Count; i++)
            {
                InGameCardLevel level = item.levels[i];
                level._ID = item._ID;
                level._level = i + 1;
                level._require = level._level+1;
                level._stat = i == 0 ? 1 : item.levels[i - 1]._stat+1;
            }
        }
    }
    #endregion VALIDATE
#endif
}

[System.Serializable]
public class InGameCardConfig
{
    [TabGroup("General")] public int _cardID;
    [TabGroup("General")] public string _cardName;
    [TabGroup("General")] [TextArea] public string _cardDescription; //show as "1x player damage"
    [TabGroup("General")] [TextArea] public string _cardSkillDescription; //translate the 1x player damage to damage

    [Space(5f)]
    [TabGroup("General")] public int _gamePointOfCard;

    [TabGroup("Skill And Tier")]
    public InGameBaseCardEffectID _skillID;
    [TabGroup("Skill And Tier")]
    public Sprite _sprCardEffect;
    [TabGroup("Skill And Tier")]
    public bool _isBonusTier;
    //public InGameCardEffectConfig SkillConfig => InGameCardEffectConfigs.Instance.GetSkillConfig(this._skillID);

    public InGameCardLevelsConfig LevelsConfig => InGameCardLevelsConfigs.Instance.GetCardLevelsConfig(this._cardID);

    [TabGroup("Art")] public Sprite _sprCardArtwork;
    [TabGroup("Art")] public Sprite _sprCardBackground;

    /// <summary>
    /// Take the card description,
    /// Player stat will be describe as 1x player damage/stat
    /// </summary>
    /// <returns></returns>
    public string GetBaseLevelDescription()
    {
        if(InGameCardLevelsConfigs.Instance.TryGetCardLevelConfig(this._cardID, level: 1, out InGameCardLevel lvConfig))
        {
            return string.Format(this._cardDescription, $"{lvConfig._stat}x");
        }
        return this._cardDescription;
    }
}

#region Card Effect

//[System.Serializable]
//public class InGameCardEffectConfigs
//{

//    #region Singleton
//    private static InGameCardEffectConfigs _instance;

//    public static InGameCardEffectConfigs Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = InGameCardConfigs.Instance._skillConfigs;
//            }
//            return _instance;
//        }
//    }

//    #endregion Singleton

//    #region Data Prop
//    public List<InGameCardEffectConfig> _configs;
//    #endregion Data Prop

//    #region Getter
//    public InGameCardEffectConfig GetSkillConfig(InGameBaseCardEffectID id)
//    {
//        return _configs.Find(x => x._skillID == id);
//    }
//    #endregion Getter
//}
//[System.Serializable]
//public class InGameCardEffectConfig
//{
//    public InGameBaseCardEffectID _skillID;

//    [Space(5f)]
//    public Sprite _sprCardEffect;
//    public string _cardEffectDescription;


//}
#endregion Card Effect

#region Card Level ups
[System.Serializable]
public class InGameCardLevelsConfigs
{

    #region Singleton
    private static InGameCardLevelsConfigs _instance;

    public static InGameCardLevelsConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = InGameCardConfigs.Instance._levelsConfigs;
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<InGameCardLevelsConfig> _configs;
    #endregion Data Prop

    #region Getter
    public bool TryGetCardLevelsConfig(int id, out InGameCardLevelsConfig levels)
    {
        levels = GetCardLevelsConfig(id);
        return levels != null;
    }
    public InGameCardLevelsConfig GetCardLevelsConfig(int id)
    {
        return this._configs.Find(x => x._ID == id);
    }
    public bool TryGetCardLevelConfig(int id, int level, out InGameCardLevel levelConfig)
    {
        if(TryGetCardLevelsConfig(id, out InGameCardLevelsConfig levels))
        {
            levelConfig = levels.GetLevelConfig(level);
            return levelConfig != null;
        }
        levelConfig = null;
        return false;
    }

    #endregion Getter
}
[System.Serializable]
public class InGameCardLevelsConfig
{
    public int _ID;

    [Space(5f)]
    public List<InGameCardLevel> levels;

    public InGameCardLevel GetLevelConfig(int level)
    {
        return levels.Find(x => x._level == level);
    }
}
[System.Serializable]
public class InGameCardLevel
{
    public int _ID;
    public int _level;

    [Space(5f)]
    ///số card copy cần CÓ HƠN HOẶC BẰNG để upgrade lên đến
    public int _require;
    ///số HP heal, số damage gây ra, số shield add vào.... <summary>
    /// Stat có thể ở dạng int dùng trực tiếp : HP, Shield...
    /// Stat cũng có thể ở dạng float để nhân với character stat: Damage
    /// </summary>
    public float _stat; 
}
#endregion Card Level ups