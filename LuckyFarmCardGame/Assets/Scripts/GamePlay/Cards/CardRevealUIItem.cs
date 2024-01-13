using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRevealUIItem : CardPopupUIItem
{
    public TMPro.TextMeshProUGUI _tmpIndex;

    public override void ParseData(int cardID)
    {
        base.ParseData(cardID);
    }
    public void SetIndex(int i) => _tmpIndex?.SetText(i.ToString());

    public void OnClickItem()
    {
        InGameCardConfig config = InGameCardConfigs.Instance.GetCardConfig(this.cardID);

        CardInfoPopup d = CardInfoPopup.ShowCardDialog();
        d.ParseData(title: config._cardName, description: InGameUtils.CreateCardDataModel(cardID).GetSkillDescribe());
        d.SetCardIcon(config._sprCardArtwork);
        d.SetPosition(Vector3.zero);
    }
}
