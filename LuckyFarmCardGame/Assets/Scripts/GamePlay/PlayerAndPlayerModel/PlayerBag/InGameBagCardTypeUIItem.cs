using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InGameBagCardTypeUIItem : MonoBehaviour
{
    #region Prop on Editor
    public Image _imgIconCard;
    public TextMeshProUGUI _tmpValueWithMax;

    [Space(5f)]
    public Color _colorHexEnoughRequire;
    public Color _colorHexNotEnoughRequire;

    [Space(5f)]
    [SerializeField] protected Toggle _tglChosingInEffect;
    #endregion  Prop on Editor

    #region Data
    protected int _cardID; public int CardID => _cardID;
    protected InGameBasePlayerItem _hostPlayer;
    protected InGameBasePlayerBagVisual _hostBagVisual;
    public bool IsOwnedByMainUser => _hostPlayer?.IsMainPlayer ?? false;

    protected int _maxValue;

    #endregion Data


    public InGameBagCardTypeUIItem SetHostPlayer(InGameBasePlayerItem p)
    {
        this._hostPlayer = p;
        Debug.Log("HAS COMMENT THIS, CONSIDER MOVE LOGIC TO MAIN PLAYER ONLY");
        //this._hostBagVisual = _hostPlayer?.BagVisual;
        return this;
    }
    public InGameBagCardTypeUIItem UpdateValue(int currentValue)
    {
        this._tmpValueWithMax.SetText($"{currentValue}/{(IsOwnedByMainUser ? _maxValue : "??")}");
        if(this.IsOwnedByMainUser)
            _tmpValueWithMax.color = _maxValue > 0 ? (currentValue >= _maxValue ? this._colorHexEnoughRequire: this._colorHexNotEnoughRequire) : Color.white;
        return this;
    }
    public InGameBagCardTypeUIItem SetMaxValue(int maxValue)
    {
        _maxValue = maxValue;
        return this;
    }
    public InGameBagCardTypeUIItem SetCardType(int cardID, Sprite _spr = null)
    {
        _cardID = cardID;

        if (_spr != null)
            this._imgIconCard.sprite = _spr;
        else
            this._imgIconCard.sprite = InGameCardConfigs.Instance.GetCardConfig(cardID)?._sprCardArtwork;
        return this;
    }

    public InGameBagCardTypeUIItem EnableToggleForEffectStage(bool isOnToggle)
    {
        _tglChosingInEffect.isOn = false;

        if (_tglChosingInEffect != null)
            this._tglChosingInEffect.enabled = isOnToggle;

        return this;
    }
    /// <summary>
    /// Called by toggle event 
    /// </summary>
    /// <param name="isOn"></param>
    public void ToggleEvent_OnChangeChosingValue(bool isOn)
    {
        _hostBagVisual?.ToggleEvent_OnChangeChosingValue(this._cardID,isOn);
    }

    /// <summary>
    /// Bot click chọn toggle item này
    /// </summary>
    public void ClickToggleFromMManager()
    {
        this._tglChosingInEffect.isOn = !this._tglChosingInEffect.isOn;
    }
}
