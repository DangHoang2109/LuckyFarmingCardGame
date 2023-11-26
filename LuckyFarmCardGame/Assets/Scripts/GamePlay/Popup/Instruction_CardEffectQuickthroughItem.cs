using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Instruction_CardEffectQuickthroughItem : MonoBehaviour
{
    #region Prop on Editor
    public Image _imgIconCard, _imgIconEffect;
    public TextMeshProUGUI _tmpCardEffectDescription;

    #endregion  Prop on Editor

    public void ParseCard(InGameCardConfig config)
    {
        InGameBaseCardEffectID _id = config._skillID;
        if(config != null)
        {
            if(_imgIconEffect != null)
            {
                _imgIconEffect.sprite = config._sprCardEffect;
            }
            if (_tmpCardEffectDescription != null)
            {
                _tmpCardEffectDescription.text = config.GetBaseLevelDescription();
            }
            if (_imgIconCard != null)
            {
                if(config != null)
                    _imgIconCard.sprite = config._sprCardArtwork;
            }
        }
    }
}
