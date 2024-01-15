using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class InGameAI
{
    /// <summary>
    /// Class giúp AI "nhìn" trên bàn và thu thập thông tin hiện tại, giao tiếp với các class khác qua LookingMessage
    /// </summary>
    public class AILooker
    {
        public InGameBotPlayerItem _enemyItem;
        public BaseInGameEnemyDataModel EnemyModel => _enemyItem?.EnemyModel;
        public InGameEnemyStatConfig EnemyStat => _enemyItem?.StatConfig;
        public InGameEnemyInfoConfig EnemyInfo => _enemyItem?.InfoConfig;

        protected InGameManager _ingameManager;
        public InGameManager InGameManager
        {
            get
            {
                if (_ingameManager == null)
                    _ingameManager = InGameManager.Instance;
                return _ingameManager;
            }
        }

        protected CardGameController _gameController;
        public CardGameController InGameController
        {
            get
            {
                if (_gameController == null)
                    _gameController = InGameManager?.GameController;
                return _gameController;
            }
        }

        public AILooker()
        {

        }
        public AILooker(InGameBotPlayerItem p)
        {
            this._enemyItem = p;
        }

        public LookingMessage Look()
        {
            LookingMessage msg = new LookingMessage();

            msg._mainPlayerInfo = InGameManager.GetMainPlayerLookingInfos();
            msg._currentTotalAmountOfEnemy = InGameManager.EnemysAlive.Count;
            return msg;
        }

        public void EndTurn()
        {
            
        }
    }
    /// <summary>
    /// Class message AI Looker return ra gửi cho Decider giúp nó đưa ra quyết định
    /// </summary>
    public class LookingMessage
    {
        /// <summary>
        /// Tình trạng hiện tại của Main player
        /// </summary>
        public OtherPlayerLookingInfo _mainPlayerInfo;
        public int _currentTotalAmountOfEnemy;
    }
    public class OtherPlayerLookingInfo
    {
        public int _playerID;
    }

    /// <summary>
    /// Quyết định và thực thi lượt
    /// </summary>
    public class AIExecutor
    {
        protected InGameBotPlayerItem _enemyItem;
        public BaseInGamePlayerDataModel PlayerInfo => _enemyItem?.PlayerModel;
        public BaseInGameEnemyDataModel EnemyModel => _enemyItem?.EnemyModel;
        public InGameEnemyStatConfig EnemyStat => _enemyItem?.StatConfig;
        public InGameEnemyInfoConfig EnemyInfo => _enemyItem?.InfoConfig;

        protected LookingMessage _msg;

        List<ExecutionAction> _allActions;
        Queue<ExecutionAction> _actions;
        public Queue<ExecutionAction> GetAction() => _actions;
        public bool deQueue;

        public AIExecutor(InGameBotPlayerItem _player)
        {
            this._enemyItem = _player;
            _actions = new Queue<ExecutionAction>();
            deQueue = false;
        }
        public void Decide()
        {
            if(this._allActions == null || _allActions.Count == 0)
            {
                List<ExecutionAction> actions = new List<ExecutionAction>();
                foreach (var skillID in EnemyStat._skills)
                {
                    actions.Add(CreateAction(skillID, this._enemyItem));
                }
                actions.Add(new ExecuteEndTurn(this._enemyItem));
                _allActions = actions;
            }
        }
        public void SetDecision()
        {
            try
            {
                //chờ vài giây
                _actions.Enqueue(new ExecuteThink(_enemyItem, 0.5f));
                foreach (var action in _allActions)
                {
                    this._actions.Enqueue(action);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        public bool IsHasAction()
        {
            return this._actions.Count > 0;
        }
        public void EndTurn()
        {
            _msg = null;
            this._actions.Clear();
            deQueue = false;
        }

        public string GetSkillDescribe()
        {
            string concat = "";

            foreach (var item in _allActions)
            {
                if (item is ExecuteThink || item is ExecuteEndTurn)
                    continue;

                concat = $"{concat} {item.GetDescribe()}.";
            }
            return concat.VerifyInvisibleSpace();
        }
    }

    #region Execution Action
    public enum EnemySkillID
    {
        [Type(typeof(ExecuteAttackMainPlayer))]
        ATTACK_PLAYER = 0,
        [Type(typeof(ExecuteSpawnFoes))]
        SPAWNING_FOES = 1,
        [Type(typeof(ExecuteCreateShield))]
        CREATING_SHIELD = 2,
        [Type(typeof(ExecuteSuckingDrainHP))]
        DRAIN_PLAYER_HP = 3,
        [Type(typeof(ExecuteMultiplierDamage))]
        MULTIPLIER_DAMAGE = 4,
    }
    public abstract class ExecutionAction
    {
        protected InGameBotPlayerItem _bot;
        protected InGameEnemySkillConfig _skillConfig;
        public float m_ThinkTime = 0;
        protected InGameMapStatProgression _progssionConfig;
        public InGameMapStatProgression ProgressionConfig
        {
            get
            {
                if (_progssionConfig == null)
                    _progssionConfig = InGameManager.Instance.MapConfig._waveProgression;
                return _progssionConfig;
            }
        }
        public ExecutionAction()
        { }
        public ExecutionAction(InGameBotPlayerItem _player)
        {
            this._bot = _player;
        }
        public ExecutionAction(InGameEnemySkillConfig skillConfig,InGameBotPlayerItem _player)
        {
            this._bot = _player;
            this._skillConfig = skillConfig;
        }
        /// <summary>
        /// Cần clear lại thông tin gì đó
        /// Gọi vào đầu lượt
        /// </summary>
        /// <returns></returns>
        public virtual void Clear() {  }

        /// <summary>
        /// Thực thi action
        /// </summary>
        public abstract IEnumerator Do();

        /// <summary>
        /// Cần chờ trước khi thực thi action này
        /// </summary>
        /// <returns></returns>
        public virtual bool Preparing() { return true; }

        /// <summary>
        /// Action này có thể thực thi hay ko?
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCanDo() { return !(this._bot?.IsStunning ?? false); }

        public virtual string GetDescribe()
        {
            return "";
        }
        public virtual string GetDescribeNoti()
        {
            return "";
        }
        protected string GetInterval()
        {
            string interval = "each Turn";
            if (_skillConfig._statIntervall > 1)
                interval = $"every {_skillConfig._statIntervall} turns";
            return interval;
        }
    }

    public class ExecuteThink : ExecutionAction
    {
        public ExecuteThink(InGameBotPlayerItem _player) : base(_player)
        {
            m_ThinkTime = Random.Range(1.0f, 2.0f);
        }

        public ExecuteThink(InGameBotPlayerItem _player, float _thinkTime) : base(_player)
        {
            m_ThinkTime = _thinkTime;
        }
        public override void Clear()
        {
            base.Clear();
            m_ThinkTime = Random.Range(1.0f, 2.0f);
        }
        public override bool IsCanDo()
        {
            return true;
        }
        public override IEnumerator Do() { Debug.Log("Turn action: Think"); yield return new WaitForSeconds(m_ThinkTime); }
        public override bool Preparing()
        {
            m_ThinkTime -= Time.deltaTime;
            if (m_ThinkTime <= 0)
                return true;
            return false;
        }
        public override string GetDescribe()
        {
            return "Thinking...";
        }
    }

    public class ExecuteEndTurn : ExecutionAction
    {
        public ExecuteEndTurn(InGameBotPlayerItem p) : base(p) { }

        public override IEnumerator Do()
        {
            yield return new WaitForEndOfFrame(); Debug.Log("Turn action: endturn");
            InGameManager.Instance.OnUserEndTurn();
        }
        public override bool IsCanDo()
        {
            return true;
        }
        public override bool Preparing()
        {
            return true;
        }
        public override string GetDescribe()
        {
            return "End Turn!";
        }
    }
    public class ExecuteAttackMainPlayer : ExecutionAction
    {
        public ExecuteAttackMainPlayer(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem _player) : base(skillConfig, _player)
        {
        }
        public ExecuteAttackMainPlayer(InGameBotPlayerItem p) : base(p) { }
        public override IEnumerator Do()
        {
            InGameManager.Instance.ShowNotificationCardAction(GetDescribeNoti(), this._bot.transform.position);
            this._bot.AttackSingleUnit(Stat());
            yield return new WaitUntil(() => this._bot.IsInIdleAnimState());
        }
        public override bool Preparing()
        {
            return true;
        }
        public override string GetDescribe()
        {
            return $"Attack {Stat()} damage to player";
        }
        public override string GetDescribeNoti()
        {
            return $"Attack {Stat()} damage";
        }
        public int Stat() => (int)(this._skillConfig.StatAsInt * 
            Mathf.Pow(ProgressionConfig._dmgProgression, (InGameManager.Instance.CurrentWaveIndex + 1)) *
            this._bot.MultiplierDamage
            );
    }
    /// <summary>
    /// Spawn creep
    /// </summary>
    public class ExecuteSpawnFoes : ExecutionAction
    {
        int countDown;
        public ExecuteSpawnFoes(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem _player) : base(skillConfig, _player)
        {
            this.countDown = skillConfig._statIntervall;
        }
        public ExecuteSpawnFoes(InGameBotPlayerItem p) : base(p) { }

        public override bool IsCanDo()
        {
            return InGameManager.Instance.EnemysAlive.Count < CardGameController.MAX_ENEMY_ALLOW; //3 is max
        }
        public override IEnumerator Do()
        {
            InGameManager.Instance.ShowNotificationCardAction(GetDescribeNoti(), this._bot.transform.position);

            yield return new WaitForSeconds(0.7f);
            //trừ count down
            if (countDown <= 0)
            {
                //spawn the foes corresponding
                InGameManager.Instance.SpawnCreep(enemyID: this._skillConfig._statID, amount: this._skillConfig.StatAsInt, casterSeatID: _bot.SeatID, turnStunned: 1);
                countDown = this._skillConfig._statIntervall;
            }
            else
            {
                countDown--;
                Debug.Log($"UPDATE COUNT DOWN EFFECT {this._skillConfig._skillID} -{countDown}");
            }
        }
        public override bool Preparing()
        {
            return true;
        }
        public override string GetDescribe()
        {
            string foesName = InGameEnemyConfigs.Instance.GetEnemyInfoConfig(this._skillConfig._statID)?.enemyName;

            return $"Spawn {this._skillConfig.StatAsInt} {foesName} { GetInterval()}";
        }
        public override string GetDescribeNoti()
        {
            string foesName = InGameEnemyConfigs.Instance.GetEnemyInfoConfig(this._skillConfig._statID)?.enemyName;
            return $"Spawn {this._skillConfig.StatAsInt} {foesName}";
        }
    }
    /// <summary>
    /// Tạo khiên thủ
    /// </summary>
    public class ExecuteCreateShield : ExecutionAction
    {
        public ExecuteCreateShield(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem _player) : base(skillConfig, _player)
        {
        }
        public ExecuteCreateShield(InGameBotPlayerItem p) : base(p) { }

        public override IEnumerator Do()
        {
            InGameManager.Instance.ShowNotificationCardAction(GetDescribeNoti(), this._bot.transform.position);

            yield return new WaitForSeconds(0.2f);
            this._bot.DefenseCreateShield(this.Stat());
        }
        public override bool Preparing()
        {
            return true;
        }
        public override string GetDescribe()
        {
            return $"Create {Stat()} shield";
        }
        public override string GetDescribeNoti()
        {
            return $"Create {Stat()} shield";
        }
        public int Stat() => (int)(this._skillConfig.StatAsInt * Mathf.Pow(ProgressionConfig._shieldProgression, (InGameManager.Instance.CurrentWaveIndex + 1)));

    }
    /// <summary>
    /// Hút máu
    /// </summary>
    public class ExecuteSuckingDrainHP : ExecutionAction
    {
        public ExecuteSuckingDrainHP(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem _player) : base(skillConfig, _player)
        {
        }
        public ExecuteSuckingDrainHP(InGameBotPlayerItem p) : base(p) { }

        public override IEnumerator Do()
        {
            InGameManager.Instance.ShowNotificationCardAction(GetDescribeNoti(), this._bot.transform.position);

            yield return new WaitForSeconds(0.2f);

            this._bot.AttackSingleUnit(this._skillConfig.StatAsInt);
            this._bot.Heal(this._skillConfig.StatAsInt);
        }
        public override bool Preparing()
        {
            return true;
        }
        public override string GetDescribe()
        {
            return $"Drain {this._skillConfig.StatAsInt} HP from player and increase same amount";
        }
        public override string GetDescribeNoti()
        {
            return $"Drain {this._skillConfig.StatAsInt} HP";
        }
    }
    /// <summary>
    /// Hút máu
    /// </summary>
    public class ExecuteMultiplierDamage : ExecutionAction
    {
        int countDown;

        public ExecuteMultiplierDamage(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem _player) : base(skillConfig, _player)
        {
            this.countDown = skillConfig._statIntervall;

        }
        public ExecuteMultiplierDamage(InGameBotPlayerItem p) : base(p) { }

        public override IEnumerator Do()
        {
            yield return new WaitForEndOfFrame();

            //trừ count down
            countDown--;

            if (countDown <= 1) //setup cho lượt sau 
            {
                InGameManager.Instance.ShowNotificationCardAction(GetDescribeNoti(), this._bot.transform.position);

                this._bot.SetMultiplierDamage(this._skillConfig._statValue, this._skillConfig._statIntervall-1);
                countDown = this._skillConfig._statIntervall;
            }
        }
        public override bool Preparing()
        {
            return true;
        }
        public override string GetDescribe()
        {
            return $"+{this._skillConfig._statValue}% damage {GetInterval()}";
        }
        public override string GetDescribeNoti()
        {
            return $"Damage +{this._skillConfig._statValue}%";
        }
    }
    #endregion Execution Action

    public static ExecutionAction CreateAction(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem host)
    {
        ExecutionAction action = null;
        switch (skillConfig._skillID)
        {
            case EnemySkillID.ATTACK_PLAYER:
                action = new ExecuteAttackMainPlayer(skillConfig,host);
                break;
            case EnemySkillID.SPAWNING_FOES:
                action = new ExecuteSpawnFoes(skillConfig, host);
                break;
            case EnemySkillID.CREATING_SHIELD:
                action = new ExecuteCreateShield(skillConfig, host);
                break;
            case EnemySkillID.DRAIN_PLAYER_HP:
                action = new ExecuteSuckingDrainHP(skillConfig, host);
                break;
            case EnemySkillID.MULTIPLIER_DAMAGE:
                action = new ExecuteMultiplierDamage(skillConfig, host);
                break;
            default:
                Debug.Log("NOT DEFINE ENEMY SKILL " + skillConfig);
                return null;
        }
        return action;
    }
}

