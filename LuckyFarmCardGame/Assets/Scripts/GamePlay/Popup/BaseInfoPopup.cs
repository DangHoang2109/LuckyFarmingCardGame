using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BaseInfoPopup : BaseDialog
{
    public TextMeshProUGUI _tmpTitle, _tmpDescription;
    public RectTransform Rect => this.panel.transform as RectTransform;
    public virtual BaseInfoPopup Clear()
    {
        _tmpTitle.gameObject.SetActive(false);
        _tmpDescription.gameObject.SetActive(false);
        this.panel.transform.position = Vector3.zero;
        return this;
    }
    public virtual void ParseData(string title,string description)
    {
        if (_tmpTitle != null)
        {
            _tmpTitle.gameObject.SetActive(!string.IsNullOrEmpty(title));
            _tmpTitle.SetText(title);
        }
        if (_tmpDescription != null)
        {
            _tmpDescription.gameObject.SetActive(!string.IsNullOrEmpty(description));
            _tmpDescription.SetText(description);
        }
    }
    public virtual void SetPosition(Vector3 localPosition)
    {
        this.panel.transform.localPosition = localPosition;
    }
    public static BaseInfoPopup ShowDialog()
    {
        return GameManager.Instance.OnShowDialog<BaseInfoPopup>("Dialogs/BaseInfoPopup").Clear();
    }
}
