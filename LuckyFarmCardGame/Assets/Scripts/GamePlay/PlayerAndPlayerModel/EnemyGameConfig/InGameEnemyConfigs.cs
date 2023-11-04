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
    public string enemyName;
    public string enemyDescription;

    public int enemyDamage;
    public int enemyMaxHP;
}