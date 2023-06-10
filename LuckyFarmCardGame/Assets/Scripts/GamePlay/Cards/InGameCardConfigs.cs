using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<InGameCardConfig> _configs;
    public InGameCardEffectConfigs _skillConfigs;
    #endregion Data Prop

    #region Getter
    public InGameCardConfig GetCardConfig(int id)
    {
        return _configs.Find(x => x._cardID == id);
    }
    #endregion Getter
}

[System.Serializable]
public class InGameCardConfig
{
    public int _cardID;
    public string _cardName;
    public string _cardDescription;

    [Space(5f)]
    public InGameBaseCardEffectID _skillID;
    public InGameCardEffectConfig SkillConfig => InGameCardEffectConfigs.Instance.GetSkillConfig(this._skillID);

    [Space(5f)]
    public Sprite _sprCardArtwork;
    public Sprite _sprCardBackground;


}

[System.Serializable]
public class InGameCardEffectConfigs
{

    #region Singleton
    private static InGameCardEffectConfigs _instance;

    public static InGameCardEffectConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = InGameCardConfigs.Instance._skillConfigs;
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<InGameCardEffectConfig> _configs;
    #endregion Data Prop

    #region Getter
    public InGameCardEffectConfig GetSkillConfig(InGameBaseCardEffectID id)
    {
        return _configs.Find(x => x._skillID == id);
    }
    #endregion Getter
}
[System.Serializable]
public class InGameCardEffectConfig
{
    public InGameBaseCardEffectID _skillID;

    [Space(5f)]
    public Sprite _sprCardEffect;
    public string _cardEffectDescription;
}
