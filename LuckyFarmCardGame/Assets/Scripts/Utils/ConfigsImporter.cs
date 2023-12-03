using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConfigsImporter : MonoBehaviour
{
    public static string GetPath(string fileName)
    {
        string path = "Assets/Editor/Config/{0}";
        return string.Format(path, fileName);
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Cosinas/Editor/ImportEnemyConfig")]
    public static void ImportEnemyConfig()
    {
        string path = GetPath("enemy_data.csv");
        System.IO.StreamReader reader = new System.IO.StreamReader(path);
        string json = reader.ReadToEnd();
        List<string> rowConfigs = json.Split('\n').ToList();
        string[] element;

        //parse privilege config
        {
            InGameEnemyInfoConfigs inGameEnemyInfoConfigs = InGameEnemyConfigs.Instance._infoConfigs;
            List<InGameEnemyInfoConfig> configs = inGameEnemyInfoConfigs._configs;
            InGameEnemyInfoConfig config;

            int startIndex = 1;
            int endIndex = 14;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (string.IsNullOrEmpty(rowConfigs[i]) || string.IsNullOrWhiteSpace(rowConfigs[i]))
                    continue;

                try
                {
                    element = rowConfigs[i].Split(',');
                    int itemIndex = i - startIndex;

                    if (itemIndex >= configs.Count)
                    {
                        config = new InGameEnemyInfoConfig();
                        configs.Add(config);
                    }
                    else
                    {
                        config = configs[itemIndex];
                    }

                    config.enemyID = int.Parse(element[0]);
                    config.enemyName = element[1];
                    config.enemyBaseMaxHP = int.Parse(element[3]);
                    //config.enemyEffectDes = element[5];
                }
                catch (System.Exception e)
                {
                    if (e is System.ArgumentOutOfRangeException)
                    {
                        Debug.LogError(i);
                    }
                    throw;
                }
            }
        }

        reader.Close();
    }
#endif 
}
