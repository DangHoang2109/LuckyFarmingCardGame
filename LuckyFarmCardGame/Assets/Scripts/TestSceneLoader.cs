using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TestSceneLoader : MonoSingleton<TestSceneLoader>
{
    //Will be remove when have joingame datas and joingamemanager
    public static bool _isTutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        this.StartCoroutine(this.LoadGameScene());
        ParseCurrentStat();
    }

    public void OnClickJoinGameNormal()
    {
        _isTutorial = false;
        JoinGame();
    }
    public void OnClickJoinGameTutorial()
    {
        _isTutorial = true;
        JoinTutorialGame();
    }
    void JoinTutorialGame()
    {
        this.StartCoroutine(this.CheckLoadGameSceneDone());
    }
    void JoinGame(System.Action _onDoneCb = null)
    {
        this.StartCoroutine(this.CheckLoadGameSceneDone(_onDoneCb));
    }
    private IEnumerator CheckLoadGameSceneDone(System.Action _onDoneCb = null)
    {
        //yield return new WaitForEndOfFrame();
        //TempSceneManager.Instance.UnLoadScene(SceneName.HOME);
        yield return new WaitForEndOfFrame();
        TempSceneManager.Instance.ShowScene(SceneName.GAME);
        yield return new WaitForEndOfFrame();
        _onDoneCb?.Invoke();
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false); 
    }
    private IEnumerator LoadGameScene(System.Action _onDoneCb = null)
    {
        yield return TempSceneManager.Instance.PreLoadScene(SceneName.GAME);
        _onDoneCb?.Invoke();
    }

    #region Test Upgrade Character
    public TextMeshProUGUI _tmpCharacterStat;
    public int _timeUpgrade;

    float hpProg = 0.1f, dmgProg = 0.15f, healProg = 0.2f, shieldProg = 0.25f;
    public void ClickUpgradeCharHP()
    {
        CurrentPlayerConfig._maxHP = Mathf.CeilToInt(CurrentPlayerConfig._maxHP * (1 + hpProg));
        ParseCurrentStat();
    }
    public void ClickUpgradeCharDmg()
    {
        CurrentPlayerConfig._baseDamage = Mathf.CeilToInt(CurrentPlayerConfig._baseDamage * (1 + dmgProg));
        ParseCurrentStat();
    }
    public void ClickUpgradeCharShield()
    {
        CurrentPlayerConfig._baseShield = Mathf.CeilToInt(CurrentPlayerConfig._baseShield * (1 + shieldProg));
        ParseCurrentStat();
    }
    public void ClickUpgradeCharHeal()
    {
        CurrentPlayerConfig._baseHeal = Mathf.CeilToInt(CurrentPlayerConfig._baseHeal * (1 + healProg));
        ParseCurrentStat();
    }
    public void ClickDegradeChar()
    {
        CurrentPlayerConfig._maxHP = Mathf.FloorToInt(CurrentPlayerConfig._maxHP / (1 + hpProg));
        CurrentPlayerConfig._baseDamage = Mathf.FloorToInt(CurrentPlayerConfig._baseDamage / (1 + dmgProg));
        CurrentPlayerConfig._baseShield = Mathf.FloorToInt(CurrentPlayerConfig._baseShield / (1 + shieldProg));
        CurrentPlayerConfig._baseHeal = Mathf.FloorToInt(CurrentPlayerConfig._baseHeal / (1 + healProg));
        ParseCurrentStat();
    }
    public void ClickResetChar()
    {
        _playerConfig = new InGamePlayerConfig(InGamePlayerConfigs.Instance.GetCharacterConfig(10)); //farmer
        ParseCurrentStat();
    }
    private InGamePlayerConfig _playerConfig;
    public InGamePlayerConfig CurrentPlayerConfig
    {
        get
        {
            if (_playerConfig == null)
                _playerConfig = new InGamePlayerConfig(InGamePlayerConfigs.Instance.GetCharacterConfig(10)); //farmer
            return _playerConfig;
        }
    }
    void ParseCurrentStat()
    {
        string stat = $"HP: {CurrentPlayerConfig._maxHP}\tDmg: {CurrentPlayerConfig._baseDamage}<br>Heal: {CurrentPlayerConfig._baseHeal}\tShield {CurrentPlayerConfig._baseShield}";
        this._tmpCharacterStat.text = stat;
    }
    #endregion
}
