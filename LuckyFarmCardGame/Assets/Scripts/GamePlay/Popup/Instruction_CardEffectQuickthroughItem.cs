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
        InGameCardEffectConfig cardConfig = InGameCardEffectConfigs.Instance.GetSkillConfig(_id);
        if(cardConfig!= null)
        {
            if(_imgIconEffect != null)
            {
                _imgIconEffect.sprite = cardConfig._sprCardEffect;
            }
            if (_tmpCardEffectDescription != null)
            {
                _tmpCardEffectDescription.text = cardConfig._cardEffectDescription;
            }
            if (_imgIconCard != null)
            {
                if(config != null)
                    _imgIconCard.sprite = config._sprCardArtwork;
            }
        }
    }
}
