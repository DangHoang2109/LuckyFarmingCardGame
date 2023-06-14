using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoSingleton<InGameUIManager>
{



    public void OnClickDrawCard()
    {
        InGameManager.Instance.OnDrawCard();
    }
    public void OnClickEndTurn()
    {
        //pull the pallet card
        InGameManager.Instance.OnUserEndTurn();
    }
    public void OnClickShowCardQuickthrough()
    {
        GameManager.Instance.OnShowDialog<Instruction_CardEffectQuickthroughDialog>("Dialogs/Instruction_CardEffectQuickthroughDialog");
    }
}
