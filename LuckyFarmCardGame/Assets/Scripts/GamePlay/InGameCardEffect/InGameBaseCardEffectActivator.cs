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
    public InGameBasePlayerItem _host;

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
    public virtual void SetHost(InGameBasePlayerItem host)
    {
        this._host = host;
    }
}
public enum InGameBaseCardEffectID
{
    [Type(typeof(InGameCardEffectActivator_Heal))]
    HEAL = 0, //NONE_EFFECT
    [Type(typeof(InGameCardEffectActivator_DrawCard))]
    DRAW_CARD = 1,
    [Type(typeof(InGameCardEffectActivator_RevealTop))]
    REVEAL_TOP_DECK = 2,
    [Type(typeof(InGameCardEffectActivator_AttackSingleUnit))]
    ATTACK_SINGLE_UNIT = 3,
    [Type(typeof(InGameCardEffectActivator_AttackAllUnit))]
    ATTACK_ALL_UNIT = 4,
    [Type(typeof(InGameCardEffectActivator_Defense))]
    DEFENSE = 5,
    [Type(typeof(InGameCardEffectActivator_NoneEffect))]
    NONE_EFFECT = 6, //CURRENTLY NOT IMPLEMENT, OR WILL BE REPLACE WITH EFFECT NONE
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
public class InGameCardEffectActivator_AttackSingleUnit : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.ATTACK_SINGLE_UNIT;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        ShowingNotification();
        this._host?.AttackSingleUnit();
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

public class InGameCardEffectActivator_AttackAllUnit : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.ATTACK_ALL_UNIT;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        ShowingNotification();
        this._host?.AttackAllUnit();
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
public class InGameCardEffectActivator_Defense : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.DEFENSE;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        ShowingNotification();
        this._host?.DefenseCreateShield();
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
public class InGameCardEffectActivator_Heal : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.HEAL;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        ShowingNotification();
        this._host?.Heal();
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
