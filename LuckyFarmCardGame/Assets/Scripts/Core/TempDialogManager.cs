using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDialogManager : MonoSingleton<TempDialogManager>
{
    public List<string> pathDialogs;
    public Dictionary<string, BaseDialog> dialogRegisters = new Dictionary<string, BaseDialog>();
    public Transform panel;

    public BaseDialog CreateDialog(string path)
    {
        if (!this.dialogRegisters.ContainsKey(path))
        {
            BaseDialog prefab = LoaderUtility.Instance.GetAsset<BaseDialog>(path);
            if (prefab != null)
            {
                return CreateDialog(path, prefab);
            }
        }

        return null;
    }

    public BaseDialog CreateDialog(string path, BaseDialog prefab)
    {
        if (prefab != null)
        {
            var dialog = Instantiate(prefab, this.transform);
            if (dialog != null)
            {
                dialog.gameObject.name = dialog.name.Replace("(Clone)", "");
                dialog.gameObject.SetActive(false);
                this.dialogRegisters.Add(path, dialog);
                return dialog;
            }
        }

        return null;
    }

    public T GetDialog<T>(string path) where T : BaseDialog
    {
        if (this.dialogRegisters.ContainsKey(path))
        {
            return (T)this.dialogRegisters[path];
        }
        else
        {
            return (T)this.CreateDialog(path);
        }

        return null;
    }
    public IEnumerator PreloadDialog()
    {
        string folderPreload = "GUI/Dialogs/Preloads";
        var temps = Resources.LoadAll<BaseDialog>("GUI/Dialogs/Preloads");
        foreach (var dialog in temps)
        {
            string path = folderPreload + $"/{dialog.name}";
            this.CreateDialog(path, dialog);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    public List<BaseDialog> GetOnlineDialogs()
    {
        List<BaseDialog> result = new List<BaseDialog>();

        BaseDialog temp;
        foreach (Transform child in this.panel)
        {
            if (!child.gameObject.activeSelf)
                continue;

            if ((temp = child.GetComponent<BaseDialog>()) == null)
                continue;

            result.Add(temp);
        }

        return result;
    }
}
