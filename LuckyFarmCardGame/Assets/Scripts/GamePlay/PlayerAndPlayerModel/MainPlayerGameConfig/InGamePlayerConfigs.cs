using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Config:
/// Thông số các main player character: max HP, damage, deck ref, avatar
/// Mở rộng scale up len các chức nghiệp như void tyrant: Assasin, Mage, Hero...
/// </summary>
[CreateAssetMenu(menuName = "Configs/InGamePlayerConfigs", fileName = "InGamePlayerConfigs")]
public class InGamePlayerConfigs : ScriptableObject
{
    #region Singleton
    private static InGamePlayerConfigs _instance;

    public static InGamePlayerConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<InGamePlayerConfigs>("Configs/Games/InGamePlayerConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<InGamePlayerConfig> _character;
    #endregion Data Prop

    #region Getter
    public InGamePlayerConfig GetCharacterConfig(int id)
    {
        return _character.Find(x => x._characterID == id);
    }
    #endregion Getter

#if UNITY_EDITOR
    #region VALIDATE
    private void OnValidate()
    {

    }
    #endregion VALIDATE
#endif
}
[System.Serializable]
public class InGamePlayerConfig
{
    public int _characterID; //XY - X: class (see in CharacterClassConfig - define later) - Y: gender 0: male 1:female
    public string _name;
    public int _maxHP;
    public int _baseDamage;
    public Sprite _icon;
    public string _deckLink;

    public InGameDeckConfig DeckConfig
    {
        get
        {
            return InGameDeckConfigs.Instance.GetDeckByName(_deckLink);
        }
    }
} 
