using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BaseCardItem : MonoBehaviour
{

}


[System.Serializable]
public class InGame_CardDataModel : ICloneable
{
    public int _id;

    protected InGameBaseCardEffectActivator _effectActivator;
    public InGameBaseCardEffectActivator EffectActivator => _effectActivator;

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
    }
    public void OnPulledToBag()
    {
        this.EffectActivator?.ActiveEffectWhenPulledToBag();
    }
#endregion
    private void ParseUIInfo()
    {

    }
    private void RefreshUI()
    {

    }



}