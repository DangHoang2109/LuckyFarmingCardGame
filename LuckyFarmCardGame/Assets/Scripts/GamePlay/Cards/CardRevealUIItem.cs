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
}
