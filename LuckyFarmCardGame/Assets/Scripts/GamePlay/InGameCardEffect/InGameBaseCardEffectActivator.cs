using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseCardEffectActivator
{
    public virtual InGameBaseCardEffectID ID => InGameBaseCardEffectID.NONE_EFFECT;
    public int _cardID;
    protected int _cardLevel;
    public int CardLevel => this._cardLevel;

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
    protected InGameCardLevel _currentLevelConfig;
    public InGameCardLevel CurrentLevelConfig
    {
        get
        {
            if (_currentLevelConfig == null || this._currentLevelConfig._level != this._cardLevel)
            {
                if (InGameCardLevelsConfigs.Instance.TryGetCardLevelConfig(this._cardID, this._cardLevel, out InGameCardLevel l))
                    _currentLevelConfig = l;
                else
                    Debug.LogError($"NOT FOUNT {_cardID} {CardLevel}");
            }
            return _currentLevelConfig;
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
        InGameManager.Instance.ShowNotificationCardAction($"Card effect: {string.Format(this.EffectConfig?._cardEffectDescription, this.CurrentLevelConfig._stat)}");
    }
    public virtual void SetIDAndHost(int id,InGameBasePlayerItem host)
    {
        this._cardID = id;
        this._host = host;
    }
    public virtual void UpdateCardLevel(int cardLevel)
    {
        this._cardLevel = cardLevel;
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
        InGameManager.Instance.OnTellControllerToRevealTopCard(this.CurrentLevelConfig._stat);
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
        this._host?.AttackSingleUnit(this.CurrentLevelConfig._stat);
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
        this._host?.AttackAllUnit(this.CurrentLevelConfig._stat);
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
        this._host?.DefenseCreateShield(this.CurrentLevelConfig._stat);
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
        this._host?.Heal(this.CurrentLevelConfig._stat);
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
