using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickingBonusDialog : BaseDialog
{
    //public List<PickingBonusCardItem> _items;
    List<BonusStageEffectAct> acts;
    System.Action _onCompleteChose;
    [Header("TextDesInButton")]
    public List<TextMeshProUGUI> btnDes;

    [Header("Card Bonus")]
    public BaseCardVisual _cardVisual;
    public TextMeshProUGUI _tmpCardSkillDescription;

    public void ParseData(List<BonusStageEffectAct> acts, System.Action onCompleteChose)
    {
        this.acts = acts;
        _onCompleteChose = onCompleteChose;

        for (int i = 0; i < acts.Count; i++)
        {
            btnDes[i].SetText(acts[i].DescriptionTop);
        }

        BonusStageEffectAct cardAct = acts.Find(x => BonusStageEffectAct.IsCardBonus(x.EffectID));
        if(cardAct != null)
            ParseCardUI(cardAct);

        void ParseCardUI(BonusStageEffectAct act)
        {
            InGame_CardDataModel c = InGameUtils.CreateCardDataModel(act.ID);

            _cardVisual.SetCardIDAndDisplayAllVisual(act.ID);
            this._tmpCardSkillDescription.SetText(c.GetSkillDescribe());
        }
    }
    /// <summary>
    /// paramsIndex Should be 0 - 1
    /// 0 - 1 is index of act if user pick one only
    /// </summary>
    /// <param name="paramsIndex"></param>
    public void OnClickButtonChose(int paramsIndex)
    {
        if (paramsIndex >= 0 && paramsIndex < this.acts.Count)
            OnClickChose(acts[paramsIndex]);
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
