using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCardCircleItem : BaseCardItem
{
    public void OnClickItem()
    {
        CardInfoPopup d = CardInfoPopup.ShowCardDialog();
        d.ParseData(title: this.CardConfig?._cardName, description: this.cardModel?.GetSkillDescribe().VerifyInvisibleSpace());
        d.SetCardIcon(this.CardConfig._sprCardArtwork);
        d.SetPosition(Vector3.zero);
    }
}
