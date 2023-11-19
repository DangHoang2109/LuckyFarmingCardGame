using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameDataManager : MonoSingleton<GameDataManager>
{
    private GameDatas gameDatas;
    private UserBoosters boosters;

#if UNITY_EDITOR
    [SerializeField]
    public string hoangGetOldData;

    [ContextMenu("GetPref")]
    private void GetPref()
    {
        hoangGetOldData = PlayerPrefs.GetString(GameDefine.USER_INFO_DATA);
    }
    [ContextMenu("SetPref")]
    private void SetPref()
    {
        PlayerPrefs.SetString(GameDefine.USER_INFO_DATA, hoangGetOldData);
        PlayerPrefs.Save();
    }
#endif

    public GameDatas GameDatas
    {
        get
        {
            if(this.gameDatas == null)
            { }
            return this.gameDatas;
        }
    }
    public UserBoosters Boosters
    {
        get
        {
            if(this.boosters == null)
            {

            }
            return this.boosters;
        }
    }

    public YieldInstruction _waitFor5s;
    private Coroutine _saving = null;
    /// <summary>
    /// Bắt đầu load thông tin user
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnLoadData()
    {
        yield return new WaitForEndOfFrame();
        this._waitFor5s = new WaitForSeconds(5f);
        ///Load user info
        this.LoadUserData();
        while (this.gameDatas == null)
        {

            yield break;
        }
        yield return new WaitForEndOfFrame();
        ///Load tiếp booster
        ///
        this.LoadBoosterData();
        while (this.boosters == null)
        {

            yield break;
        }
        yield return new WaitForEndOfFrame();


        this.OpenGame();

    }

    private bool _isCompletedLoadScene = false;
    public bool isCompletedLoadScene => this._isCompletedLoadScene;
    public static UnityAction<bool> callbackCompleteScene;

    /// <summary>
    /// Load thông tin user
    /// </summary>
    private void LoadUserData()
    {
        try
        {
            if (PlayerPrefs.HasKey(GameDefine.USER_INFO_DATA))
            {
                string jsonData = PlayerPrefs.GetString(GameDefine.USER_INFO_DATA);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    this.gameDatas = JsonUtility.FromJson<GameDatas>(jsonData);
                }
                else
                {
                    Debug.LogError("CAN NOT PARSE USER DATA: " + jsonData);
                    return;
                }
            }
            else
            {
                //Create New User;
                this.CreateUser();

            }
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// Lưu thông tin user data
    /// </summary>
    public void SaveUserData()
    {
        if (gameDatas != null)
            gameDatas.SetTimeWhenUserLosrFocus(false);

        string jsonData = JsonUtility.ToJson(this.gameDatas);
        PlayerPrefs.SetString(GameDefine.USER_INFO_DATA, jsonData);
        if (this._saving == null)
        {
            _saving = this.StartCoroutine(this.DelaySaveData());
        }
    }
    /// <summary>
    /// Load thông tin booster
    /// </summary>
    private void LoadBoosterData()
    {
        try
        {
            if (PlayerPrefs.HasKey(GameDefine.USER_BOOSTER_DATA))
            {
                string jsonData = PlayerPrefs.GetString(GameDefine.USER_BOOSTER_DATA);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    this.boosters = JsonUtility.FromJson<UserBoosters>(jsonData);
                }
                else
                {
                    Debug.LogError("CAN NOT PARSE BOOSTER DATA: " + jsonData);
                    return;
                }
            }
            // else
            // {
            // // CreateUser() had initialized boosters   
            // }
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// Lưu thông tin booster
    /// </summary>
    public void SaveBoosterData()
    {
        string jsonData = JsonUtility.ToJson(this.boosters);
        PlayerPrefs.SetString(GameDefine.USER_BOOSTER_DATA, jsonData);
        if (this._saving == null)
        {
            this.StartCoroutine(this.DelaySaveData());
        }
    }
    private IEnumerator DelaySaveData()
    {
        yield return _waitFor5s;
        PlayerPrefs.Save();
        this._saving = null;
    }
    private bool IsAbleToFocus()
    {
        //if (GameManager.Instance.GetScene() == null || GameManager.Instance.GetScene().name == SceneName.LOADING)
        //    return false;

        if (LoadingManager.Instance.IsLoading)
            return false;

        if (this.gameDatas == null)
            return false;

        if (!isCompletedLoadScene)
            return false;

        return true;
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!IsAbleToFocus())
            return;

        if (!focus)
        {
            //UserBehaviorDatas.Instance?.FocusApp();
            GameDatas.SetTimeWhenUserLosrFocus();
            this.ForceSave();
        }
    }

    private void OnApplicationQuit()
    {
        if (!IsAbleToFocus())
            return;
        //UserBehaviorDatas.Instance.QuitApp();
        GameDatas.SetTimeWhenUserLosrFocus();

        PlayerPrefs.Save();
        this._saving = null;
    }

    private void ForceSave()
    {
        PlayerPrefs.Save();
        if (this._saving != null)
        {
            this.StopCoroutine(this._saving);
            this._saving = null;
           
        }
    }
    /// <summary>
    /// Tạo mới user
    /// </summary>
    private void CreateUser()
    {
        this.gameDatas = GameDatas.Create() ;
        this.boosters = new UserBoosters();
        this.boosters.CreateUser();
        this.gameDatas.CreateUser();

        this.SaveUserData();
        this.SaveBoosterData();
    }

    public void OpenGame()
    {
        //LanguageManager.Instance.OnLoadData();

        this.gameDatas.OpenGame();

        _isCompletedLoadScene = true;
        if (callbackCompleteScene != null)
        {
            callbackCompleteScene.Invoke(this._isCompletedLoadScene);
        }

        //CPSocialManager.Instance.OpenApp();
    }
    
    #region Calendar
    public void AddCallendars()
    {
        new System.Globalization.GregorianCalendar();
        new System.Globalization.PersianCalendar();
        new System.Globalization.UmAlQuraCalendar();
        new System.Globalization.ThaiBuddhistCalendar();
    }
    
    #endregion Calendar
}
