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

        public bool deQueue;

        public AIExecutor(InGameBotPlayerItem _player)
        {
            this._enemyItem = _player;
            _actions = new Queue<ExecutionAction>();
            deQueue = false;
        }
        public void Decide(LookingMessage _cMsg)
        {
            _msg = _cMsg;

            if(this._allActions == null || _allActions.Count == 0)
            {
                List<ExecutionAction> actions = new List<ExecutionAction>();
                foreach (var skillID in EnemyStat._skills)
                {
                    actions.Add(CreateAction(skillID, this._enemyItem, _msg));
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
                _actions.Enqueue(new ExecuteThink(_enemyItem, Random.Range(0.8f, 1.2f)));
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

        public void Execute()
        {
            if (_actions.Count == 0)
                return;

            if (_actions.Peek().Preparing())
            {
                ExecutionAction act = _actions.Dequeue();
                if(act.IsCanDo())
                    act.Do();
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
        protected LookingMessage _lMsg;
        protected InGameEnemySkillConfig _skillConfig;
        public float m_ThinkTime = 0;

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
        public abstract void Do();

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

        public virtual void SetLookingMessage(LookingMessage msg)
        {
            this._lMsg = msg;
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
        public override void Do() { }
        public override bool Preparing()
        {
            m_ThinkTime -= Time.deltaTime;
            if (m_ThinkTime <= 0)
                return true;
            return false;
        }
    }

    public class ExecuteEndTurn : ExecutionAction
    {
        public ExecuteEndTurn(InGameBotPlayerItem p) : base(p) { }

        public override void Do()
        {
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
    }
    public class ExecuteAttackMainPlayer : ExecutionAction
    {
        public ExecuteAttackMainPlayer(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem _player) : base(skillConfig, _player)
        {
        }
        public ExecuteAttackMainPlayer(InGameBotPlayerItem p) : base(p) { }
        public override void Do()
        {
            Debug.Log("act skill: attack player");
            this._bot.AttackSingleUnit(this._skillConfig.StatAsInt);
        }
        public override bool Preparing()
        {
            return true;
        }
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
        public override void Do()
        {
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

        public override void Do()
        {
            Debug.Log("act skill: create shield");
            this._bot.DefenseCreateShield(this._skillConfig.StatAsInt);
        }
        public override bool Preparing()
        {
            return true;
        }
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

        public override void Do()
        {
            this._bot.AttackSingleUnit(this._skillConfig.StatAsInt);
            this._bot.Heal(this._skillConfig.StatAsInt);
        }
        public override bool Preparing()
        {
            return true;
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

        public override void Do()
        {
            Debug.Log("act skill: multiplier damage");
            //trừ count down
            if (countDown <= 0)
            {
                this._bot.SetMultiplierDamage(this._skillConfig._statValue, this._skillConfig._statIntervall);
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
    }
    #endregion Execution Action

    public static ExecutionAction CreateAction(InGameEnemySkillConfig skillConfig, InGameBotPlayerItem host, LookingMessage lMsg)
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

        action?.SetLookingMessage(lMsg);
        return action;
    }
}

