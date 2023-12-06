using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PickingBonusCardItem : MonoBehaviour
{
    public TextMeshProUGUI _tmpDescriptionTop;
    public Button _btnChose;

    public bool _isCardBonus;
    [Space(5f)]
    public GameObject _gCard;
    public BaseCardVisual _cardVisual;

    [Space(5f)]
    public GameObject _gPerk;
    public Image _perkIcon;
    public TextMeshProUGUI _tmpPerkDescription;

    System.Action<BonusStageEffectAct> _onClickChose;
    BonusStageEffectAct _act;
    public void ParseData(BonusStageEffectAct act, System.Action<BonusStageEffectAct> onClickChose)
    {
        _onClickChose = onClickChose;
        _act = act;
        this._tmpDescriptionTop.text = act.DescriptionTop;
        if (this._isCardBonus)
        {
            _cardVisual.SetCardIDAndDisplayAllVisual(act.ID);
            InGameCardConfig _cardConfig = InGameCardConfigs.Instance.GetCardConfig(act.ID);
        }
        else
        {
            this._perkIcon.sprite = act.Icon;
            this._tmpPerkDescription.text = act.DescriptionPerk;
        }
    }
    public void ClickChose()
    {
        _onClickChose?.Invoke(_act);
    }
}
