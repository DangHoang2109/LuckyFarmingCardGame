﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_CardDataModelLevels
{
    public int _cardID;
    public int _currentCard;
    public int _currentLevel = 1;

    public int CurrentGoal => CurrentLevelConfig?._require ?? 0;

    protected InGame_CardDataModel _cardModel;

    public InGameCardLevel CurrentLevelConfig => _cardModel.CurrentLevelConfig;
    
    public InGameCardConfig CardConfig => _cardModel.CardConfig;

    public InGameCardLevel PreviousLevelConfig
    {
        get
        {
            if (InGameCardLevelsConfigs.Instance.TryGetCardLevelConfig(this._cardID, this._currentLevel-1, out InGameCardLevel l))
                return l;

            Debug.LogError("NOT FOUND PREVIOUS LEVEL " + (_currentLevel - 1));
            return null;
        }
    }
    /// <summary>
    /// This will not use to activate effect
    /// Just use to sync query the card stat
    /// </summary>
    public InGameBaseCardEffectActivator CardEffectActivator => _cardModel.EffectActivator;

    public float CurrentStat => CardEffectActivator?.GetStat() ?? 0;
    /// <summary>
    /// We only have this value if the card has level up atleast 1 time
    /// </summary>
    private float _cacheOldStat;
    public float CacheOldStat => _cacheOldStat;
    public InGame_CardDataModelLevels()
    {
        _currentCard = 0;
    }
    public InGame_CardDataModelLevels(InGame_CardDataModel palletCard, int startingCard = 0)
    {
        this._cardID = palletCard._id;
        this._cardModel = InGameUtils.CreateCardDataModel(palletCard._id);
        this._currentCard = startingCard;
        this._currentLevel = 1;
    }
    public InGame_CardDataModelLevels AddACard(InGame_CardDataModel palletCard, out bool isLevelUp, int amount = 1)
    {
        isLevelUp = false;
        if (this._cardID == palletCard._id)
        {
            this._currentCard += amount;
        }
        //check level UP
        if (isLevelUp = CheckLevelUp())
            LevelUp();

        return this;
    }
    public bool CheckLevelUp()
    {
        return this._currentCard >= CurrentLevelConfig?._require;
    }
    public void LevelUp()
    {
        //trừ card đi bằng require
        //hiện chưa có thì - bằng level cũ +1
        //level 1->2 thì cần 2 card -> trừ 2
        this._currentCard = Mathf.Clamp(this._currentCard - (CurrentLevelConfig?._require ?? 0), 0, int.MaxValue);
        this._currentLevel += 1;

        _cacheOldStat = this.CurrentStat;
        _cardModel?.SetCurrentLevel(_currentLevel);
    }
}
