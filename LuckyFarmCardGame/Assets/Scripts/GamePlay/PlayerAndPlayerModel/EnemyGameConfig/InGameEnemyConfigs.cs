using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public InGameEnemyInfoConfigs _infoConfigs;
    public List<InGameEnemyWaveConfig> _waveConfigs;

    #endregion Data Prop

    #region Getter
    public InGameEnemyWaveConfig GetWaveConfig(int wave)
    {
        return _waveConfigs.Find(x => x._wave == wave);
    }
    #endregion Getter

#if UNITY_EDITOR
    #region VALIDATE
    private void OnValidate()
    {
        for (int i = 0; i < _waveConfigs.Count; i++)
        {
            InGameEnemyWaveConfig wave = _waveConfigs[i];
            wave._wave = i;
        }
    }
    #endregion VALIDATE
#endif
}
[System.Serializable]
public class InGameEnemyWaveConfig
{
    public int _wave; //số round
    public List<InGameEnemyStatConfig> _enemyInRound; //số enemy trong ROUND NÀY

    public int AmountEnemy => _enemyInRound.Count;
    public InGameEnemyStatConfig GetEnemyStat(int index)
    {
        if(index >= 0 && index < _enemyInRound.Count)
        {
            return _enemyInRound[index];
        }
        Debug.LogError("OUT OF CONFIG");
        return null;
    }
}
[System.Serializable]
public class InGameEnemyStatConfig
{
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

    public int Damage => this.Info?.enemyBaseDamage ?? 0;
    public int Shield => this.Info?.enemybaseShield ?? 0;
    public int Heal => this.Info?.enemybaseHeal ?? 0;

    public int MaxHP => this.Info?.enemyBaseMaxHP ?? 0 ;
    public string EffectDes => this.Info?.enemyEffectDes;
}
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
    public int enemyBaseDamage, enemybaseShield, enemybaseHeal;
    public int enemyBaseMaxHP;
    public string enemyEffectDes;

    [Header("SKIN AND ANIM")]
    [Space(10f)]
    public CSkeletonAnimator _animator; //must contain the skeleton Graphic in here
}
#endregion