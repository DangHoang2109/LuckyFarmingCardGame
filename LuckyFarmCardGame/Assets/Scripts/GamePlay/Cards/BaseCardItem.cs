using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Coffee.UIExtensions;
public class BaseCardItem : MonoBehaviour
{
    #region Prop on Editor
    [SerializeField]
    protected BaseCardVisual _visual;
    public BaseCardVisual CardVisual => _visual;

    public UIDissolve _uIDissolveBackgroundEffect;
    public UIShiny _uIShinyAvatarEffect;

    public InGameBasePlayerItem _host;

    #endregion Prop on Editor

    #region Data
    protected int _cardID;
    public int CardID => _cardID;
    private InGameCardConfig _cardConfig;
    public InGameCardConfig CardConfig
    {
        get
        {
            if (_cardConfig == null)
                _cardConfig = InGameCardConfigs.Instance.GetCardConfig(this._cardID);
            return _cardConfig;
        }
    }

    protected InGame_CardDataModel cardModel;
    #endregion Data

    protected void OnDisable()
    {
        this._host = null;
    }

    public BaseCardItem ParseInfo(InGame_CardDataModel card)
    {
        cardModel = card;
        this._cardID = card._id;
        this.CardVisual?.SetCardIDAndDisplayAllVisual(CardConfig);

        return this;
    }
    public BaseCardItem ParseHost(InGameBasePlayerItem host)
    {
        this._host = host;
        return this;
    }
    public BaseCardItem OnPullingToBagEffect()
    {
        return this.OnShinyEffect(OnPullingAnimComplete);
        void OnPullingAnimComplete()
        {
            Transform tfDesOrb = InGamePlayerUpgradePalletAnim.Instance._tfTo;
            VFXManager.Instance.ShowFX(id: VFXGameID.orbCardExp, amount: 1, _desTransform: tfDesOrb, _startTransform: this.transform, _pathType: PoolPathType.BASIC_CURVE, _onCompleteAllCb: DestroyMe);
            HideMe();
        }
    }
    public BaseCardItem OnDestroyingEffect()
    {
        return this.OnDissolveEffect(OnDestroyAnimComplete);
        void OnDestroyAnimComplete()
        {
            DestroyMe();
        }
    }
    private void HideMe()
    {
        this.gameObject.SetActive(false);
    }
    private void DestroyMe()
    {
        Destroy(this.gameObject);
    }
    public BaseCardItem OnDissolveEffect(System.Action cb)
    {
        //Find this on dissolve Component, it is suck, but the package only allow this and not have any callback
        float dissolveDuration = 1f;
        this._uIDissolveBackgroundEffect?.Play(reset:true);

        Cosina.Components.Invoker.Invoke(OnDissolveComplete, delay: dissolveDuration);
        return this;

        void OnDissolveComplete()
        {
            cb?.Invoke();
        }
    }
    public BaseCardItem OnShinyEffect(System.Action cb)
    {
        //Find this on dissolve Component, it is suck, but the package only allow this and not have any callback
        float shinyDuration = 1f;
        this._uIShinyAvatarEffect?.Play(reset: true);

        Cosina.Components.Invoker.Invoke(OnShinyComplete, delay: shinyDuration);
        return this;

        void OnShinyComplete()
        {
            cb?.Invoke();
        }
    }
}


[System.Serializable]
public class InGame_CardDataModel : ICloneable
{
    public int _id;
    public InGameBaseCardEffectID _effect;
    public int _coinPoint;
    public int _amount = 1;
    public int _currentLevel;

    protected InGameBaseCardEffectActivator _effectActivator;
    public InGameBaseCardEffectActivator EffectActivator => _effectActivator;

    protected BaseCardItem _cardContainer;
    public BaseCardItem CardContainer => _cardContainer;

    public InGameMainPlayerItem _host;

    protected InGameCardConfig _cardConfig;
    public InGameCardConfig CardConfig
    {
        get
        {
            if (_cardConfig == null)
                _cardConfig = InGameCardConfigs.Instance.GetCardConfig(_id);
            return _cardConfig;
        }
    }
    protected InGameCardLevel _currentLevelConfig;
    public InGameCardLevel CurrentLevelConfig
    {
        get
        {
            if (_currentLevelConfig == null || this._currentLevelConfig._level != this._currentLevel)
            {
                if (InGameCardLevelsConfigs.Instance.TryGetCardLevelConfig(this._id, this._currentLevel, out InGameCardLevel l))
                    _currentLevelConfig = l;
                else
                    Debug.LogError($"NOT FOUNT {_id} {_currentLevel}");
            }
            return _currentLevelConfig;
        }
    }

    public InGame_CardDataModel()
    {

    }
    public InGame_CardDataModel(InGame_CardDataModel c)
    {
        this._id = c._id;
    }
    public object Clone()
    {
        return new InGame_CardDataModel(this);
    }

    public InGame_CardDataModel SetCardID(int id, InGameCardConfig cardConfig)
    {
        this._id = id;
        this._effect = cardConfig._skillID;
        this._coinPoint = cardConfig?._gamePointOfCard ?? 0;

        CreateEffectActivator();
        ParseUIInfo();
        return this;
    }
    public InGame_CardDataModel SetCardItemContainer(BaseCardItem cardItem)
    {
        this._cardContainer = cardItem;
        return this;
    }
    public InGame_CardDataModel SetHost(InGameBasePlayerItem h)
    {
        this._host = h as InGameMainPlayerItem;
        return this;
    }
    public InGame_CardDataModel SetCurrentLevel(int currentLevel)
    {
        this._currentLevel = currentLevel;
        return this;
    }
    #region These function should in cardItem
    protected void CreateEffectActivator()
    {
        this._effectActivator = InGameUtils.CreateCardEffectActivator(skillID: this._effect, cardID: this._id, host: this._host, cardModel: this);
    }
    public void OnDrawedFromDeck()
    {
        this.EffectActivator?.ActiveEffectWhenDrawed();
    }
    public void OnPlacedToPallet()
    {
        this.EffectActivator?.ActiveEffectWhenPlaceToPallet();
    }
    public void OnDestroyedByConflict()
    {
        this.EffectActivator?.ActiveEffectWhenDestroyed();
        this.CardContainer?.OnDestroyingEffect();
    }
    public void OnPulledToBag()
    {
        this.EffectActivator?.ActiveEffectWhenPulledToBag();
        this.CardContainer?.OnPullingToBagEffect();
    }
    #endregion
    public string GetSkillDescribe()
    {
        return string.Format(this.CardConfig?._cardSkillDescription, this.EffectActivator.GetStat()).VerifyInvisibleSpace();//this.CurrentLevelConfig._stat
    }
    private void ParseUIInfo()
    {

    }
    private void RefreshUI()
    {

    }



}
