using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CardUpgradedInfoUIItem : CardPopupUIItem
{
    public TextMeshProUGUI _tmpCardLevel, _tmpCardDescription;
    public RectTransform Rect => this.transform as RectTransform;
    [SerializeField] public float Prefered_Min_Height = 170f;
    public void ParseData(InGame_CardDataModelLevels cardModelLevels)
    {
        ParseData(cardModelLevels._cardID);

        _tmpCardLevel.SetText($"Level {(cardModelLevels?.CurrentLevelConfig?._level ?? 0)}");

        string des = cardModelLevels?.CardConfig?._cardDescription;
        if (!string.IsNullOrEmpty(des))
        {
            string statUp = $"{cardModelLevels?.CacheOldStat} -> {cardModelLevels?.CurrentStat}";
            _tmpCardDescription.text = string.Format(des, statUp);
        }

        this.Rect.sizeDelta = new Vector2(this.Rect.sizeDelta.x, Mathf.Max(_tmpCardDescription.preferredHeight, Prefered_Min_Height));
    }
    public override void ParseData(int cardID)
    {
        base.ParseData(cardID);

    }
}
