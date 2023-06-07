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
        ParseUIInfo();
        return this;
    }
    private void ParseUIInfo()
    {

    }
    private void RefreshUI()
    {

    }



}