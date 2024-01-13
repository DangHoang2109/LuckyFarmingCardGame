using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/BonusStageConfigs", fileName = "BonusStageConfigs")]
public class BonusStageConfigs : ScriptableObject
{
    #region Singleton
    private static BonusStageConfigs _instance;

    public static BonusStageConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<BonusStageConfigs>("Configs/Games/BonusStageConfigs");
            }
            return _instance;
        }
    }

    #endregion Singleton

    #region Data Prop
    public List<BonusEffectConfig> _configs;
    public Dictionary<BonusStageEffectID, BonusEffectConfig> _dictConfig;
    #endregion Data Prop

    #region Getter
    private void CheckDict()
    {
        if(_dictConfig == null)
        {
            _dictConfig = new Dictionary<BonusStageEffectID, BonusEffectConfig>();
            foreach (var item in _configs)
            {
                _dictConfig.Add(item._id, item);
            }
        }
    }
    public List<BonusStageEffectAct> GenerateBonusStageActivator(List<int> ids)
    {
        CheckDict();

        List < BonusStageEffectAct > res = new List <BonusStageEffectAct >();
        foreach (var id in ids)
        {
            BonusStageEffectID effID = (BonusStageEffectID)id;

            if(id < 0)
            {
                if (effID == BonusStageEffectID.RandomLeft)
                    effID = RandomLeft();
                else if (effID == BonusStageEffectID.RandomRight)
                    effID = RandomRight();
            }
            BonusStageEffectAct act = Activator.CreateInstance(EnumUtility.GetStringType(effID)) as BonusStageEffectAct;
            if(act != null)
            {
                act.EffectID = effID;
                if(_dictConfig.TryGetValue(effID, out BonusEffectConfig c))
                {
                    act.DescriptionTop = c.descriptionTop;
                    act.DescriptionPerk = c.descriptionPerk;
                    act.Icon = c._icon;
                    res.Add(act);

                    act.OnCreated();
                }
            }
        }
        return res;

        BonusStageEffectID RandomLeft()
        {
            return (BonusStageEffectID)(UnityEngine.Random.Range(0, 3));
        }
        BonusStageEffectID RandomRight()
        {
            return (BonusStageEffectID)(UnityEngine.Random.Range(10, 12));
        }
    }
    #endregion Getter 

}
[System.Serializable]
public class BonusEffectConfig
{
    public BonusStageEffectID _id;
    public string descriptionTop, descriptionPerk;
    public Sprite _icon;
}
public enum BonusStageEffectID
{
    RandomLeft = -2,
    RandomRight = -1,

    [Type(typeof(BonusStageEffectAct_AddNewCard))]
    AddNewCard = 0,
    [Type(typeof(BonusStageEffectAct_AddPointExistCard))]
    AddPointToCard = 1,
    [Type(typeof(BonusStageEffectAct_DuplicateExistCard))]
    DuplicateCard = 2,

    [Type(typeof(BonusStageEffectAct_HealFullHP))]
    HealFullHP = 10,
    [Type(typeof(BonusStageEffectAct_IncreaseMaxHP))]
    IncreaseMaxHP = 11,
    [Type(typeof(BonusStageEffectAct_ReceiveGem))]
    ReceiveGem = 12,
}
public class BonusStageEffectAct
{
    public virtual BonusStageEffectID EffectID { get; set; }

    public static bool IsCardBonus(BonusStageEffectID effID) => (int)effID < 10;
    public virtual float ValueAmount { get; set; }
    public int ValueAsInt => (int)ValueAmount;
    /// <summary>
    /// For card bonus: which is the id
    /// </summary>
    public virtual int ID { get; set; }
    public string DescriptionTop { get; set; }
    public string DescriptionPerk { get; set; }
    public Sprite Icon { get; set; }
    protected InGameMainPlayerItem MainPlayer => InGameManager.Instance.MainUserPlayer;
    protected int MainPlayerID => MainPlayer.SeatID;
    public virtual void OnCreated()
    {

    }
    public virtual void OnChosed()
    {
        Debug.Log("On Chosen " + this.EffectID);
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
        this.ValueAmount = 0.1f * (MainPlayer?.MaxHP ?? 0); //increase 10% of maxHP
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

        InGameManager mng = InGameManager.Instance;
        List<InGameCardConfig> allBonusCard = InGameCardConfigs.Instance.GetAllBonusCardConfigs();
        List<InGameCardConfig> notOwning = allBonusCard.Where(x => !mng.IsOwningThisCardIDInDeck(x._cardID)).ToList();
        //id ko được trùng với card đã có
        if(notOwning.Count > 0)
        {
            this.ID = notOwning.GetRandom()._cardID;
        }
    }
    public override void OnChosed()
    {
        base.OnChosed();
        //heal full HP of main player
        InGameManager.Instance.OnPlayerAddNewCard(this.ID, this.ValueAsInt);
    }
}
public class BonusStageEffectAct_DuplicateExistCard : BonusStageEffectAct
{
    public override void OnCreated()
    {
        base.OnCreated();
        this.ValueAmount = 1; //receive one new card

        List<int> Owning = InGameManager.Instance.GetAllCardBonusOwning();
        //id ko được trùng với card đã có
        if (Owning.Count > 0)
        {
            this.ID = Owning.GetRandom();
        }
    }
    public override void OnChosed()
    {
        base.OnChosed();
        //heal full HP of main player
        InGameManager.Instance.OnPlayerAddNewCard(this.ID, this.ValueAsInt);
    }
}
public class BonusStageEffectAct_AddPointExistCard : BonusStageEffectAct
{
    public override void OnCreated()
    {
        base.OnCreated();
        this.ValueAmount = 3; //receive one new card

        List<int> Owning = InGameManager.Instance.GetAllCardBonusOwning();
        //id ko được trùng với card đã có
        if (Owning.Count > 0)
        {
            this.ID = Owning.GetRandom();
        }
    }
    public override void OnChosed()
    {
        base.OnChosed();
        //heal full HP of main player
        InGameManager.Instance.OnPlayerAddCardPoint(this.ID, this.ValueAsInt);
    }
}