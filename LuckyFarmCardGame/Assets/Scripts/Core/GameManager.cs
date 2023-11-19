using System;
using System.Collections;
using System.Collections.Generic;
//using Cosina.DataManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
#if UNITY_EDITOR
    public static bool isApplicationQuit = false;
#endif

    private List<BaseDialog> baseDialogs;
    private Dictionary<string, BaseDialog> tempDialog;

    public event System.Action<BaseDialog> OnADialogSummoned;
    public event System.Action<BaseDialog> OnADialogClosed;

    private void OnEnable()
    {
        //SaveManager.Instance.RegisterCallbackOnDataLoaded(() =>
        //{
        //    Debug.Log("Load Data DOne");
        //    SaveManager.Instance.SaveDataInstantly();
        //});
    }

    public override void Init()
    {
        base.Init();
        this.baseDialogs = new List<BaseDialog>();
        this.tempDialog = new Dictionary<string, BaseDialog>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Application.targetFrameRate = 60;
        // TODO! !!!!
        Application.runInBackground = true;
    }

    private void OnLoadDataDone()
    {
    }
    #region DIALOG
    public T GetDialogImadiate<T>(string path) where T : BaseDialog
    {
        if (!this.tempDialog.ContainsKey(path))
        {
            T target = TempDialogManager.Instance.GetDialog<T>(path);
            if (target == null)
            {
                Debug.LogError("CAN NOT LOAD DATA BY PATH: " + path);
                return null;
            }

            if (target != null)
            {
                this.tempDialog.Add(path, target);

                target.gameObject.SetActive(false);
                return target;
            }
        }
        else
        {
            BaseDialog dialog = this.tempDialog[path];
            dialog.gameObject.SetActive(false);
            return (T)dialog;
        }
        return null;
    }
    public T OnShowDialog<T>(string path, object data = null, UnityAction callback = null, bool isSkipAnimationShow = false) where T : BaseDialog
    {
        if (!this.tempDialog.ContainsKey(path))
        {
            T target = TempDialogManager.Instance.GetDialog<T>(path);
            if (target == null)
            {
                Debug.LogError("CAN NOT LOAD DATA BY PATH: " + path);
                return null;
            }

            if (target != null)
            {
                target.gameObject.SetActive(true);

                target.transform.localScale = Vector3.one;
                target.transform.localPosition = Vector3.zero;
                target.transform.SetAsLastSibling();
                target.OnShow(data, callback, isSkipAnimationShow);
                if (!this.baseDialogs.Contains(target))
                {
                    this.baseDialogs.Add(target);
                }
                this.tempDialog.Add(path, target);

                //No, in here it must call Summon-ed, 
                target.OnShowed += OnADialogSummoned;

                //cũ của Khang
                //this.OnADialogSummoned?.Invoke(target);

                return target;
            }

        }
        else
        {
            BaseDialog dialog = this.tempDialog[path];
            if (dialog.gameObject.activeSelf)
                return null;

            dialog.gameObject.SetActive(true);
            dialog.transform.localScale = Vector3.one;
            dialog.transform.localPosition = Vector3.zero;
            dialog.transform.SetAsLastSibling();
            dialog.OnShow(data, callback, isSkipAnimationShow);
            if (!this.baseDialogs.Contains(dialog))
            {
                this.baseDialogs.Add(dialog);
            }

            //this.OnADialogSummoned?.Invoke(dialog);
            dialog.OnShowed += OnADialogSummoned;
            return (T)dialog;
        }
        return null;
    }

    public T InstantDialog<T>(string path, object data = null, UnityAction callback = null) where T : BaseDialog
    {

        if (!this.tempDialog.ContainsKey(path))
        {
            T target = TempDialogManager.Instance.GetDialog<T>(path);
            if (target == null)
            {
                Debug.LogError("CAN NOT LOAD DATA BY PATH: " + path);
                return null;
            }

            if (target != null)
            {
                target.gameObject.SetActive(true);

                target.transform.localScale = Vector3.one;
                target.transform.localPosition = Vector3.zero;
                target.transform.SetAsLastSibling();
                this.baseDialogs.Add(target);
                this.tempDialog.Add(path, target);

                this.OnADialogSummoned?.Invoke(target);
                return target;
            }

        }
        else
        {
            BaseDialog dialog = this.tempDialog[path];
            if (dialog.gameObject.activeSelf)
                return null;

            dialog.gameObject.SetActive(true);
            dialog.transform.localScale = Vector3.one;
            dialog.transform.localPosition = Vector3.zero;
            dialog.transform.SetAsLastSibling();
            this.baseDialogs.Add(dialog);

            this.OnADialogSummoned?.Invoke(dialog);
            return (T)dialog;
        }
        return null;
    }

    public void OnHideDialog(BaseDialog dialog)
    {
        dialog.OnHide();
        if (this.baseDialogs.Contains(dialog))
        {
            this.baseDialogs.Remove(dialog);
        }
        OnADialogClosed?.Invoke(dialog);
    }

    public void CloseDialog<T>() where T : BaseDialog
    {
        foreach (BaseDialog dialog in this.baseDialogs)
        {
            if (dialog is T)
            {
                this.OnHideDialog(dialog);
                return;
            }
        }
    }
    public bool IsDialog<T>() where T : BaseDialog
    {
        foreach (BaseDialog dialog in this.baseDialogs)
        {
            if (dialog is T)
            {
                return true;
            }

        }
        return false;
    }

    public bool TryGetActiveDialog<T>(out T dialog) where T : BaseDialog
    {
        dialog = this.baseDialogs.Find(x => x is T && x.gameObject.activeInHierarchy) as T;
        return dialog != null;
    }

    public void CloseAllDialog()
    {
        List<BaseDialog> dialogs = new List<BaseDialog>(this.baseDialogs);

        foreach (BaseDialog dialog in dialogs)
        {
            dialog.OnCloseDialog();
        }
    }

    public BaseDialog GetActiveDialog()
    {
        foreach (BaseDialog dialog in this.baseDialogs)
        {
            if (dialog.gameObject.activeInHierarchy)
                return dialog;
        }
        return null;
    }

    [System.Obsolete("Use TempDialogManager.GetOnlineDialogs  ")]
    public List<BaseDialog> GetActiveDialogs()
    {
        return this.baseDialogs;
    }

    #endregion

#if UNITY_EDITOR
    public void OnApplicationQuit()
    {
        isApplicationQuit = true;
    }


#endif


    /// <summary>
    /// Trả về dialog CÒN TRÊN SCENE
    /// HÀM SUPER NGUY HIỂM, hãy đảm bảo bạn muốn get dialog đang được show trên màn hình
    /// NHẤT ĐỊNH CẦN KIỂM TRA NULL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetDialogUnSafe<T>(string path) where T : BaseDialog
    {
        if (this.baseDialogs != null)
        {
            BaseDialog d = baseDialogs.Find(x => x is T);
            if (d != null)
            {
                return d as T;
            }
        }
        return null;
        //return this.OnShowDialog<T>(path);
    }
    public string GetCurrentDialogName()
    {
        if (this.baseDialogs != null && this.baseDialogs.Count > 0)
            return this.baseDialogs[baseDialogs.Count - 1].name;

        return null;
    }
    public bool IsInternet()
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    //TODO: Language
        //    MessageBox.Instance.ShowMessageBox("No Internet", "Internet No Connecting, Pls try again").SetEvent(null);
        //    return false;
        //}
        return true;
    }
    public bool CheckInternet => Application.internetReachability != NetworkReachability.NotReachable;

    private bool _oldInternetConnection = true;

    public static System.Action<bool> _onChanceInternetConnection;
    private void Update()
    {
        bool newInternetConnection = CheckInternet;
        if (_oldInternetConnection != newInternetConnection)
        {
            _oldInternetConnection = newInternetConnection;
            _onChanceInternetConnection?.Invoke(newInternetConnection);
        }
    }
    public void AssignOnChangeInternetConnection(System.Action<bool> cb)
    {
        _onChanceInternetConnection -= cb;
        _onChanceInternetConnection += cb;
        cb?.Invoke(CheckInternet);
    }
}