using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPopupUIItem : MonoBehaviour
{
    public BaseCardVisual _visual;

    public virtual void ParseData(int cardID)
    {
        ParseDisplay(cardID);
    }
    public virtual void ParseDisplay(int cardID)
    {
        _visual.SetCardIDAndDisplayAllVisual(cardID);

    }
}
