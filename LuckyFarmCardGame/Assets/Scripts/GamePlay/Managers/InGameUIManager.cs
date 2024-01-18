using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoSingleton<InGameUIManager>
{
    public Transform _tfButtonSummaryRule;
    public void OnClickEndTurn()
    {
        //pull the pallet card
        InGameManager.Instance.OnUserEndTurn();
    }
    public void OnClickShowCardQuickthrough()
    {
        RuleSummaryDialog.ShowDialog();
        //GameManager.Instance.OnShowDialog<Instruction_CardEffectQuickthroughDialog>("Dialogs/Instruction_CardEffectQuickthroughDialog");
    }
}
