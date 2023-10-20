using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseCardEffectActivator
{
    public virtual InGameBaseCardEffectID ID => InGameBaseCardEffectID.NONE_EFFECT;

    protected InGameCardEffectConfig _effectConfig;
    public InGameCardEffectConfig EffectConfig
    {
        get
        {
            if (_effectConfig == null)
                _effectConfig = InGameCardEffectConfigs.Instance.GetSkillConfig(this.ID);
            return _effectConfig;
        }
    }

    public virtual void ActiveEffectWhenDrawed()
    {
        Debug.Log($"CARD {(int)ID}: activate effect {ID} when drawed");
    }
    public virtual void ActiveEffectWhenPlaceToPallet()
    {
        Debug.Log($"CARD {(int)ID}: activate effect {ID} when place to pallet");
    }
    public virtual void ActiveEffectWhenDestroyed()
    {
        Debug.Log($"CARD {(int)ID}: activate effect {ID} when destroyed");
    }
    public virtual void ActiveEffectWhenPulledToBag()
    {
        Debug.Log($"CARD {(int)ID}: activate effect {ID} when pulled to bag");
    }
    public virtual void ShowingNotification()
    {
        InGameManager.Instance.ShowNotificationCardAction($"Card effect: {this.EffectConfig?._cardEffectDescription}");
    }
}
public enum InGameBaseCardEffectID
{
    [Type(typeof(InGameCardEffectActivator_NoneEffect))]
    NONE_EFFECT = 0,
    [Type(typeof(InGameCardEffectActivator_DrawCard))]
    DRAW_CARD = 1,
    [Type(typeof(InGameCardEffectActivator_RevealTop))]
    REVEAL_TOP_DECK = 2,
    [Type(typeof(InGameCardEffectActivator_NoneEffect))]
    NONE_EFFECT_1 = 3,
    [Type(typeof(InGameCardEffectActivator_NoneEffect))]
    NONE_EFFECT_2 = 4,
    [Type(typeof(InGameCardEffectActivator_NoneEffect))]
    NONE_EFFECT_3 = 5,

}
public class InGameCardEffectActivator_NoneEffect : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.NONE_EFFECT;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        InGameManager.Instance.OnTellControllerContinueTurn();

    }
    public override void ActiveEffectWhenDestroyed()
    {
        base.ActiveEffectWhenDestroyed();
    }
    public override void ActiveEffectWhenPulledToBag()
    {
        base.ActiveEffectWhenPulledToBag();
    }
    public override void ShowingNotification()
    {
    }
}
public class InGameCardEffectActivator_DrawCard : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.DRAW_CARD;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        InGameManager.Instance.OnTellControllerToDrawCards(1);

    }
    public override void ActiveEffectWhenDestroyed()
    {
        base.ActiveEffectWhenDestroyed();
    }
    public override void ActiveEffectWhenPulledToBag()
    {
        base.ActiveEffectWhenPulledToBag();
    }
    public override void ShowingNotification()
    {
    }
}
public class InGameCardEffectActivator_RevealTop : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.REVEAL_TOP_DECK;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        ShowingNotification();
        InGameManager.Instance.OnTellControllerToRevealTopCard(1);
    }
    public override void ActiveEffectWhenDestroyed()
    {
        base.ActiveEffectWhenDestroyed();
    }
    public override void ActiveEffectWhenPulledToBag()
    {
        base.ActiveEffectWhenPulledToBag();
    }
    public override void ShowingNotification()
    {
        base.ShowingNotification();
    }
}



