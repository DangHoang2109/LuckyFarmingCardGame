using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BaseCardVisual : MonoBehaviour
{
    protected int _cardID;
    public int CardID => _cardID;
    [Header("Card Artwork")]
    public Image _imgArt;
    public Image _imgBackground;
    [Header("Card Effect Icon")]
    public Image _imgEffectIcon;
    public GameObject _gEffectIcon;
    [Header("Card Type Icon")]
    public Image _imgArtIcon;
    public GameObject _gTypeIcon;


    public BaseCardVisual SetCardIDAndDisplayAllVisual(int cardID)
    {
        this._cardID = cardID;

        InGameCardConfig cardConfig = InGameCardConfigs.Instance.GetCardConfig(cardID);
        if(cardConfig!= null)
        {
            this.DisplayArtwork(cardConfig._sprCardArtwork)
                .DisplayEffect(cardConfig.SkillConfig?._sprCardEffect)
                .DisplayBackground(cardConfig._sprCardBackground);
        }
        return this;
    }
    public BaseCardVisual DisplayArtwork(Sprite spr)
    {
        if(_imgArt!= null)
            this._imgArt.sprite = spr;
        if (_imgArtIcon != null)
            this._imgArtIcon.sprite = spr;
        return this;
    }
    public BaseCardVisual DisplayBackground(Sprite spr)
    {
        if (_imgBackground != null)
            this._imgBackground.sprite = spr;
        return this;
    }
    public BaseCardVisual DisplayEffect(Sprite spr)
    {
        if (_imgEffectIcon != null)
        {
            _imgEffectIcon.gameObject.SetActive(spr != null);
            this._imgEffectIcon.sprite = spr;
        }
        return this;
    }

}
