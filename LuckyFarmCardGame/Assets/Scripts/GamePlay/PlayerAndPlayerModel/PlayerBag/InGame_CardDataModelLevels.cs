﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_CardDataModelLevels
{
    public int _cardID;
    public int _currentCard;
    public int _currentLevel = 1;

    public int CurrentGoal => CurrentLevelConfig?._require ?? 0;

    protected InGameCardLevel _currentLevelConfig;
    public InGameCardLevel CurrentLevelConfig
    {
        get
        {
            if (_currentLevelConfig == null || this._currentLevelConfig._level != this._currentLevel)
            {
                if (InGameCardLevelsConfigs.Instance.TryGetCardLevelConfig(this._cardID, this._currentLevel, out InGameCardLevel l))
                    _currentLevelConfig = l;
                else
                    Debug.LogError($"NOT FOUNT {_cardID} {_currentLevel}");
            }
            return _currentLevelConfig;
        }
    }
    public InGame_CardDataModelLevels()
    {
        _currentCard = 0;
    }
    public InGame_CardDataModelLevels(int id, int startingCard =0, int startingLevel= 1)
    {
        this._cardID = id;
        this._currentCard = startingCard;
        this._currentLevel = startingLevel;
    }
    public InGame_CardDataModelLevels(InGame_CardDataModelLevels c)
    {
        this._cardID = c._cardID;
        this._currentCard = c._currentCard;
        this._currentLevel = c._currentLevel;
    }
    public InGame_CardDataModelLevels(InGame_CardDataModel palletCard, int startingCard = 0)
    {
        this._cardID = palletCard._id;
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
        return this._currentCard > CurrentLevelConfig?._require;
    }
    public void LevelUp()
    {
        //trừ card đi bằng require
        //hiện chưa có thì - bằng level cũ +1
        //level 1->2 thì cần 2 card -> trừ 2
        this._currentCard = Mathf.Clamp(this._currentCard - (CurrentLevelConfig?._require ?? 0), 0, int.MaxValue);
        this._currentLevel += 1;

    }
}