using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfoPopup : BaseInfoPopup
{
    public UnityEngine.UI.Image _imgCardIcon;

    public virtual void SetCardIcon(Sprite spr)
    {
        if (_imgCardIcon != null)
            _imgCardIcon.sprite = spr;
    }
    public static CardInfoPopup ShowCardDialog() 
    {
        CardInfoPopup d = GameManager.Instance.OnShowDialog<CardInfoPopup>("Dialogs/CardInfoPopup");
        d.Clear();
        return d;
    }
}
