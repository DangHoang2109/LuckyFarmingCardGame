using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(menuName = "Configs/InGameMissionGoalCardConfigs", fileName = "InGameMissionGoalCardConfigs")]
public class InGameMissionGoalCardConfigs : ScriptableObject
{
    #region Singleton
    private static InGameMissionGoalCardConfigs _instance;

    public static InGameMissionGoalCardConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<InGameMissionGoalCardConfigs>("Configs/Games/InGameMissionGoalCardConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<InGameMissionGoalCardConfig> _configs;
    #endregion Data Prop

    #region Getter
    public List<InGameMissionGoalCardConfig> GetRandomConfigs(int amount)
    {
        List<InGameMissionGoalCardConfig> res = new List<InGameMissionGoalCardConfig>();

        List<InGameMissionGoalCardConfig> available = new List<InGameMissionGoalCardConfig>(this._configs);
        for (int i = 0; i < amount; i++)
        {
            if (available.Count == 0)
                break;

            int randIndex = Random.Range(0, available.Count);
            res.Add(available[randIndex]);

            available.RemoveAt(randIndex);
        }

        return res;
    }
    public InGameMissionGoalCardConfig GetRandomConfig()
    {
        return new InGameMissionGoalCardConfig(this._configs.GetRandom());
    }
    public InGameMissionGoalCardConfig GetRandomConfig(List<InGameMissionGoalCardConfig> ObsoleteCards)
    {
        List<InGameMissionGoalCardConfig> available = new List<InGameMissionGoalCardConfig>(this._configs);
        available = available.Where(x => ObsoleteCards.IndexOf(x) == -1).ToList() ;
        return new InGameMissionGoalCardConfig(available.GetRandom());
    }
    #endregion Getter
}

[System.Serializable]
public class InGameMissionGoalCardConfig
{
    public int _id;

    public List<InGame_CardDataModelWithAmount> _requirement;

    public InGameMissionGoalCardConfig()
    {

    }
    public InGameMissionGoalCardConfig(InGameMissionGoalCardConfig c)
    {
        this._id = c._id;
        this._requirement = new List<InGame_CardDataModelWithAmount>(c._requirement);
    }
}

//public class InGameCardWithAmountPair
//{
//    public int _cardID;
//    public int _amount;

//    public InGameCardWithAmountPair()
//    {

//    }
//    public InGameCardWithAmountPair(InGameCardWithAmountPair c)
//    {
//        this._cardID = c._cardID;
//        this._amount = c._amount;
//    }
//}
