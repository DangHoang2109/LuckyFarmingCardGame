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

    #endregion Prop on Editor

    #region Data
    protected int _cardID;
    public int CardID => _cardID;
    #endregion Data



    public BaseCardItem ParseInfo(int id)
    {
        this._cardID = id;

        this.CardVisual?.SetCardIDAndDisplayAllVisual(id);

        return this;
    }
    public BaseCardItem OnPullingToBagEffect()
    {
        return this.OnShinyEffect(OnPullingAnimComplete);
        void OnPullingAnimComplete()
        {
            Destroy(this.gameObject);
        }
    }
    public BaseCardItem OnDestroyingEffect()
    {
        return this.OnDissolveEffect(OnDestroyAnimComplete);
        void OnDestroyAnimComplete()
        {
            Destroy(this.gameObject);
        }
    }
    public BaseCardItem OnDissolveEffect(System.Action cb)
    {
        //Find this on dissolve Component, it is suck, but the package only allow this and not have any callback
        float dissolveDuration = 1f;
        this._uIDissolveBackgroundEffect.Play(reset:true);

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
        this._uIShinyAvatarEffect.Play(reset: true);

        Cosina.Components.Invoker.Invoke(OnShinyComplete, delay: shinyDuration);
        return this;

        void OnShinyComplete()
        {
            Destroy(this.gameObject);
        }
    }
}


[System.Serializable]
public class InGame_CardDataModel : ICloneable
{
    public int _id;

    protected InGameBaseCardEffectActivator _effectActivator;
    public InGameBaseCardEffectActivator EffectActivator => _effectActivator;

    protected BaseCardItem _cardContainer;
    public BaseCardItem CardContainer => _cardContainer;


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

    public InGame_CardDataModel SetCardID(int id)
    {
        this._id = id;
        CreateEffectActivator();
        ParseUIInfo();
        return this;
    }
    public InGame_CardDataModel SetCardItemContainer(BaseCardItem cardItem)
    {
        this._cardContainer = cardItem;
        return this;
    }
    #region These function should in cardItem
    protected void CreateEffectActivator()
    {
        this._effectActivator = System.Activator.CreateInstance(EnumUtility.GetStringType(GetActivatorEffectID(this._id))) as InGameBaseCardEffectActivator;

        InGameBaseCardEffectID GetActivatorEffectID(int cardID)
        {
            return (InGameBaseCardEffectID)cardID;
        }
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
    private void ParseUIInfo()
    {

    }
    private void RefreshUI()
    {

    }



}
