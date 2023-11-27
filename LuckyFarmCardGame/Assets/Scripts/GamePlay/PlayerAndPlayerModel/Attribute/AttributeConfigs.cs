using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/AttributeConfigs", fileName = "AttributeConfigs")]
public class AttributeConfigs : ScriptableObject
{
    #region Singleton
    private static AttributeConfigs _instance;

    public static AttributeConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<AttributeConfigs>("Configs/Games/AttributeConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<AttributeConfig> _config;
    #endregion Data Prop

    #region Getter
    public AttributeConfig GetConfig(AttributeID id)
    {
        return _config.Find(x => x._id == id);
    }
    public Sprite GetIcon(AttributeID id)
    {
        return GetConfig(id)?._sprIcon;
    }
    #endregion Getter
}
[System.Serializable]
public class AttributeConfig
{
    public AttributeID _id;
    public Sprite _sprIcon;
    public string _name;
    public string _description;
}
public enum AttributeID
{
    NONE = -1,
    SHIELD = 0,
    STUN = 1,
    INVULNERABLE = 2,
}