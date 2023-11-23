using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CardUpgradedInfoUIItem : CardPopupUIItem
{
    public TextMeshProUGUI _tmpCardDescription;
    public void ParseData(InGame_CardDataModelLevels cardModelLevels)
    {
        ParseData(cardModelLevels._cardID);
        //_tmpCardDescription.text = cardModelLevels.
    }
    public override void ParseData(int cardID)
    {
        base.ParseData(cardID);

    }
}
