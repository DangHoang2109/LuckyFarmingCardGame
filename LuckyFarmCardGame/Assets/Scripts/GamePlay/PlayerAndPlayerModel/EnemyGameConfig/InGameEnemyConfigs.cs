using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InGameAI;

/// <summary>
/// Config:
/// Temp
/// Thông số các enemy: HP + damage theo từng round
/// </summary>
[CreateAssetMenu(menuName = "Configs/InGameEnemyConfigs", fileName = "InGameEnemyConfigs")]
public class InGameEnemyConfigs : ScriptableObject
{
    #region Singleton
    private static InGameEnemyConfigs _instance;

    public static InGameEnemyConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<InGameEnemyConfigs>("Configs/Games/InGameEnemyConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    [TabGroup("Info")]
    public InGameEnemyInfoConfigs _infoConfigs;
    [TabGroup("Stat")]
    public InGameEnemyStatConfigs _statConfigs;
    [TabGroup("Map")]
    public InGameMapConfigs _mapConfigs;

    #endregion Data Prop

    #region Getter
    #endregion Getter

#if UNITY_EDITOR
    #region VALIDATE
    private void OnValidate()
    {
        for (int i = 0; i < _mapConfigs._configs.Count; i++)
        {
            for (int j = 0; j < _mapConfigs._configs[i]._waveConfigs.Count; j++)
            {
                InGameEnemyWaveConfig wave = _mapConfigs._configs[i]._waveConfigs[j];
                wave._wave = j;
            }
        }
    }
    #endregion VALIDATE
#endif
}

#region Map and Stage
[System.Serializable]
public class InGameMapConfigs
{
    public List<InGameMapConfig> _configs;
    public InGameMapConfig GetMapConfig(int mapID) => _configs.Find(x=>x._id == mapID);
}
[System.Serializable]
public class InGameMapConfig
{
    public int _id;
    public string _name;
    public List<InGameEnemyWaveConfig> _waveConfigs;

    public InGameEnemyWaveConfig GetWaveConfig(int wave)
    {
        return _waveConfigs.Find(x => x._wave == wave);
    }
    public InGameEnemyStatConfig GetEnemyStat(int enemyID)
    {
        InGameEnemyStatConfig stat = InGameEnemyStatConfigs.Instance.GetStatOfEnemy(enemyID);
        if(stat == null)
        {
            Debug.LogError("NO STAT CONFIG WITH IF " + enemyID);
        }
        return stat;
    }
}
[System.Serializable]
public class InGameEnemyWaveConfig
{
    public int _wave; //số round
    public List<int> _enemyIDsInRound; //số enemy trong ROUND NÀY

    public int AmountEnemy => _enemyIDsInRound.Count;
    public bool _isBonusStage = false; 
    public bool IsBonusStage => _isBonusStage;
    public InGameEnemyStatConfig GetEnemyStat(int index)
    {
        if (index >= 0 && index < _enemyIDsInRound.Count)
        {
            int enemyID = _enemyIDsInRound[index];
            InGameEnemyStatConfig stat = InGameEnemyStatConfigs.Instance.GetStatOfEnemy(enemyID);
            return stat;
        }
        Debug.LogError("OUT OF CONFIG");
        return null;
    }
}
#endregion Map and Stage

#region Stats and Level of Enemy
[System.Serializable]
public class InGameEnemyStatConfigs
{
    /// <summary>
    /// THis should be change to the contain of many InGameEnemyStatConfig for enemy of many level
    /// </summar
    [TableList]
    public List<InGameEnemyStatConfig> _configs;
    public static InGameEnemyStatConfigs Instance => InGameEnemyConfigs.Instance._statConfigs;

    public InGameEnemyStatConfig GetStatOfEnemy(int enemyID)
    {
        return _configs.Find(x => x.enemyID == enemyID);
    }
}
[System.Serializable]
public class InGameEnemyStatConfig
{
    [TableColumnWidth(50, Resizable = true)]
    public int enemyID;
    [NonSerialized]
    private InGameEnemyInfoConfig _info;
    public InGameEnemyInfoConfig Info
    {
        get
        {
            if (_info == null)
                _info = InGameEnemyInfoConfigs.Instance.GetInfo(this.enemyID);
            return _info;
        }
    }

    [Header("SKILL")]
    [TableList]
    public List<InGameEnemySkillConfig> _skills;

    public int MaxHP => this.Info?.enemyBaseMaxHP ?? 0 ;
    public string EffectDes => this.Info?.enemyEffectDes;
}

[System.Serializable]
public class InGameEnemySkillConfig
{
    //[TableColumnWidth(20, Resizable = false)]
    public EnemySkillID _skillID;
    public float _statValue; //Damage, Heal, Shield, Amount Foes Spawn...
    public int _statIntervall; //How many turn cast this, if do it every turn, let it 0
    public int _statID; //ID Does to spawn

    public int StatAsInt => (int)_statValue;
}
#endregion


#region Enemy Info
[System.Serializable]
public class InGameEnemyInfoConfigs
{
    public List<InGameEnemyInfoConfig> _configs;
    public static InGameEnemyInfoConfigs Instance => InGameEnemyConfigs.Instance._infoConfigs;

    public InGameEnemyInfoConfig GetInfo(int enemyId)
    {
        return _configs.Find(x => x.enemyID == enemyId);
    }
}
[System.Serializable]
public class InGameEnemyInfoConfig
{
    public int enemyID;
    public string enemyName;
    public string enemyDescription;

    [Header("STAT BASE")]
    [Space(10f)]
    public int enemyBaseMaxHP;
    public string enemyEffectDes;

    [Header("SKIN AND ANIM")]
    [Space(10f)]
    public CSkeletonAnimator _animator; //must contain the skeleton Graphic in here
}
#endregion