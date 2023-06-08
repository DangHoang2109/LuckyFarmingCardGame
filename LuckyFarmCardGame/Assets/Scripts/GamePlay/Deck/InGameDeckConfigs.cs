using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/InGameDeckConfigs", fileName = "InGameDeckConfigs")]
public class InGameDeckConfigs : ScriptableObject
{
    #region Singleton
    private static InGameDeckConfigs _instance;

    public static InGameDeckConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<InGameDeckConfigs>("Configs/Games/InGameDeckConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<InGameDeckConfig> _configs;
    #endregion Data Prop

    #region Getter
    public InGameDeckConfig GetStandardDeck()
    {
        return _configs.Find(x => x._id == 0);
    }
    #endregion Getter
}

[System.Serializable]
public class InGameDeckConfig
{
    public int _id;
    public List<InGame_CardDataModelWithAmount> _deckContain;

    public InGameDeckConfig()
    {

    }
    public InGameDeckConfig(InGameDeckConfig d)
    {
        this._id = d._id;
        this._deckContain = new List<InGame_CardDataModelWithAmount>(d._deckContain);
    }
}