using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingBonusDialog : BaseDialog
{
    public List<PickingBonusCardItem> _items;
    List<BonusStageEffectAct> acts;
    System.Action _onCompleteChose;
    public void ParseData(List<BonusStageEffectAct> acts, System.Action onCompleteChose)
    {
        this.acts = acts;
        _onCompleteChose = onCompleteChose;
        for (int i = 0; i < acts.Count; i++)
        {
            _items[i].ParseData(acts[i], OnClickChose);
        }
    }
    void OnClickChose(BonusStageEffectAct act)
    {
        act.OnChosed();
        ClickCloseDialog();
        _onCompleteChose?.Invoke();
    }
    public void OnClickChoseBoth()
    {
        foreach (var item in this.acts)
        {
            item.OnChosed();
        }

        ClickCloseDialog();
        _onCompleteChose?.Invoke();
    }
    public static PickingBonusDialog OnShow()
    {
        return GameManager.Instance.OnShowDialog<PickingBonusDialog>("Dialogs/PickingBonusDialog");
    }
}
