using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusStageConfigs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class BonusStageEffectID
{
    public const int RandomLeft = -2;
    public const int RandomRight = -1;

    public const int AddNewCard = 0;
    public const int AddPointToCard = 1;
    public const int DuplicateCard = 2;

    public const int HealFullHP = 3;
    public const int IncreaseMaxHP = 4;
    public const int ReceiveGem = 5;
}
public class BonusStageEffectAct
{
    public virtual float ValueAmount { get; set; }
    public int ValueAsInt => (int)ValueAmount;
    /// <summary>
    /// For card bonus: which is the id
    /// </summary>
    public virtual int ID { get; set; }
    public int MainPlayerID => InGameManager.Instance.MainUserPlayer.SeatID;
    public virtual void OnCreated()
    {

    }
    public virtual void OnChosed()
    {

    }
}
public class BonusStageEffectAct_HealFullHP : BonusStageEffectAct
{
    public override void OnChosed()
    {
        base.OnChosed();
        //heal full HP of main player
        InGameManager.Instance.OnPlayerHealFullHP(this.MainPlayerID);
    }
}
public class BonusStageEffectAct_IncreaseMaxHP : BonusStageEffectAct
{
    public override void OnCreated()
    {
        base.OnCreated();
        this.ValueAmount = 0.1f * (InGameManager.Instance.MainUserPlayer?.MaxHP ?? 0); //increase 10% of maxHP
    }
    public override void OnChosed()
    {
        base.OnChosed();
        //heal full HP of main player
        InGameManager.Instance.OnPlayerIncreaseMaxHP(this.MainPlayerID, this.ValueAsInt);
    }
}
public class BonusStageEffectAct_ReceiveGem : BonusStageEffectAct
{
    public override void OnCreated()
    {
        base.OnCreated();
        this.ValueAmount = 10; //receive 10 gem
    }
    public override void OnChosed()
    {
        base.OnChosed();
        Debug.LogError("RECEIVE GEM!");
    }
}
public class BonusStageEffectAct_AddNewCard : BonusStageEffectAct
{
    public override void OnCreated()
    {
        base.OnCreated();
        this.ValueAmount = 1; //receive one new card
        //this.ID = InGameCardConfigs.Instance.GetCardConfig
    }
    public override void OnChosed()
    {
        base.OnChosed();
        //heal full HP of main player
        InGameManager.Instance.OnPlayerIncreaseMaxHP(this.MainPlayerID, this.ValueAsInt);
    }
}