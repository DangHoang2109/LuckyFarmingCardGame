using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseCardEffectActivator
{
    public virtual InGameBaseCardEffectID ID => InGameBaseCardEffectID.NONE_EFFECT;
    public int _cardID;
    protected int _cardLevel;
    public int CardLevel => this._cardLevel;

    protected InGameCardConfig _cardConfig;
    public InGameCardConfig CardConfig
    {
        get
        {
            if (_cardConfig == null)
                _cardConfig = InGameCardConfigs.Instance.GetCardConfig(_cardID);
            return _cardConfig;
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
        //Debug.Log($"CARD {(int)ID}: activate effect {ID} when drawed");
    }
    public virtual void ActiveEffectWhenPlaceToPallet()
    {
        //Debug.Log($"CARD {(int)ID}: activate effect {ID} when place to pallet");
    }
    public virtual void ActiveEffectWhenDestroyed()
    {
        //Debug.Log($"CARD {(int)ID}: activate effect {ID} when destroyed");
    }
    public virtual void ActiveEffectWhenPulledToBag()
    {
        //Debug.Log($"CARD {(int)ID}: activate effect {ID} when pulled to bag");
    }
    public virtual void ShowingNotification()
    {
        Debug.Log("Show noti " + string.Format(this.CardConfig?._cardSkillDescription, this.CurrentLevelConfig._stat));
        InGameManager.Instance.ShowNotificationCardAction($"Card effect: {string.Format(this.CardConfig?._cardSkillDescription, this.CurrentLevelConfig._stat)}");
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
    public virtual float GetStat() { return 0f; }
    public virtual int GetStatAsInt() => ((int)GetStat());

}
public enum InGameBaseCardEffectID
{
    [Type(typeof(InGameCardEffectActivator_NoneEffect))]
    NONE_EFFECT = -1, //CURRENTLY NOT IMPLEMENT, OR WILL BE REPLACE WITH EFFECT NONE
    [Type(typeof(InGameCardEffectActivator_Heal))]
    HEAL = 0, //NONE_EFFECT
    [Type(typeof(InGameCardEffectActivator_DrawCard))]
    DRAW_CARD_N_ATTACK_SINGLE = 1,
    [Type(typeof(InGameCardEffectActivator_RevealTop))]
    REVEAL_TOP_DECK = 2,
    [Type(typeof(InGameCardEffectActivator_AttackSingleUnit))]
    ATTACK_SINGLE_UNIT = 3,
    [Type(typeof(InGameCardEffectActivator_AttackAllUnit))]
    ATTACK_ALL_UNIT = 4,
    [Type(typeof(InGameCardEffectActivator_Defense))]
    DEFENSE = 5,

    [Type(typeof(InGameCardEffectActivator_DrainHP))]
    DRAIN_HP = 6,
    [Type(typeof(InGameCardEffectActivator_ReduceDmgNAttackSingle))]
    REDUCE_DMG_N_ATTACK_SINGLE = 7,
    [Type(typeof(InGameCardEffectActivator_AttackAllNStunFront))]
    ATTACK_ALL_N_STUN_FRONT = 8,
    [Type(typeof(InGameCardEffectActivator_MultiplierDamage))]
    MULTIPLIER_DMG = 10,
    [Type(typeof(InGameCardEffectActivator_Invulnerable))]
    INVULNERABLE = 11,
    [Type(typeof(InGameCardEffectActivator_MultiplierCardEffect))]
    MULTIPLIER_CARD_EFF = 14,
    [Type(typeof(InGameCardEffectActivator_ReduceDamage))]
    REDUCE_DMG = 15,
    [Type(typeof(InGameCardEffectActivator_ReorderCards))]
    REORDER_CARDS = 17,
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
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.DRAW_CARD_N_ATTACK_SINGLE;

    public override void ActiveEffectWhenDrawed()
    {
        base.ActiveEffectWhenDrawed();
    }
    public override void ActiveEffectWhenPlaceToPallet()
    {
        base.ActiveEffectWhenPlaceToPallet();
        this._host?.ForceDrawCard(1);
        this._host?.AttackSingleUnit(GetStatAsInt());
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
    public override float GetStat()
    {
        return this.CurrentLevelConfig._stat * _host.BaseDamagePerTurn;
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
        this._host?.RevealCard(GetStatAsInt());
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
    public override float GetStat()
    {
        return this.CurrentLevelConfig._stat;
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
        this._host?.AttackSingleUnit(GetStatAsInt());
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
    public override float GetStat()
    {
        return this.CurrentLevelConfig._stat * _host.BaseDamagePerTurn;
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
        this._host?.AttackAllUnit(GetStatAsInt());
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
    public override float GetStat() => ((int)this.CurrentLevelConfig?._stat * _host?.BaseDamagePerTurn) ?? 0;
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
        this._host?.DefenseCreateShield(this.GetStatAsInt());
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
    public override float GetStat() => ((int)(this.CurrentLevelConfig?._stat * _host?.BaseShieldPerAdd));
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
        this._host?.Heal(this.GetStatAsInt());
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
    public override float GetStat() => ((int)(this.CurrentLevelConfig?._stat * _host?.BaseHPPerHeal));
}
public class InGameCardEffectActivator_DrainHP : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.DRAIN_HP;

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
public class InGameCardEffectActivator_ReduceDmgNAttackSingle : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.REDUCE_DMG_N_ATTACK_SINGLE;

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
public class InGameCardEffectActivator_AttackAllNStunFront : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.ATTACK_ALL_N_STUN_FRONT;

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
public class InGameCardEffectActivator_MultiplierDamage : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.MULTIPLIER_DMG;

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
public class InGameCardEffectActivator_Invulnerable : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.INVULNERABLE;

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
public class InGameCardEffectActivator_MultiplierCardEffect : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.MULTIPLIER_CARD_EFF;

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
public class InGameCardEffectActivator_ReduceDamage : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.REDUCE_DMG;

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
public class InGameCardEffectActivator_ReorderCards : InGameBaseCardEffectActivator
{
    public override InGameBaseCardEffectID ID => InGameBaseCardEffectID.REORDER_CARDS;

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