using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CardUpgradedInfoUIItem : CardPopupUIItem
{
    public TextMeshProUGUI _tmpCardLevel, _tmpCardDescription;
    public void ParseData(InGame_CardDataModelLevels cardModelLevels)
    {
        ParseData(cardModelLevels._cardID);

        _tmpCardLevel.SetText($"Level {(cardModelLevels?.CurrentLevelConfig?._level ?? 0)}");

        string des = cardModelLevels?.CardConfig?.SkillConfig?._cardEffectDescription;
        if (!string.IsNullOrEmpty(des))
        {
            string statUp = $"{cardModelLevels?.CacheOldStat} -> {cardModelLevels?.CurrentStat}";
            _tmpCardDescription.text = string.Format(des, statUp);
        }
    }
    public override void ParseData(int cardID)
    {
        base.ParseData(cardID);

    }
}
