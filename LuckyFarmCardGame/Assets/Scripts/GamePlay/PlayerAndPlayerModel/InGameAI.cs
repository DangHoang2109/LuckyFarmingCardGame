﻿using System.Collections;
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
        public InGameBasePlayerItem _player;
        public BaseInGamePlayerDataModel PlayerInfo => _player?.PlayerModel;

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
        public AILooker(InGameBasePlayerItem p)
        {
            this._player = p;
        }

        public LookingMessage Look()
        {
            LookingMessage msg = new LookingMessage();

            //lấy tỉ lệ conflict card tiếp theo
            msg._conflictPalletPercent = InGameController.GetPalletConflictChance();
            //lấy pallet hiện tại của bản thân
            msg._myPalletCurrentCards = InGameController.GetCurrentPalletIDs();
            msg._totalCoinGainFromThisPallet = InGameController.GetPalletTotalCoin();

            msg._otherPlayerInfoList = InGameManager.GetOtherPlayerLookingInfos();
            msg._otherPlayerInfoDic = new Dictionary<int, OtherPlayerLookingInfo>();
            foreach (OtherPlayerLookingInfo info in msg._otherPlayerInfoList)
            {
                msg._otherPlayerInfoDic.Add(info._playerID, info);
            }
            //lấy card trên top deck
            msg._topDeckCardEffectID = InGameController.GetTopDeckEffect();
            msg._topDeckCardID = InGameController.GetTopDeckID();

            //có conflict khum?
            msg._willPalletConflictIfDraw = InGameController.GetResultPalletConflictIfThisCardJoin(msg._topDeckCardID);
            //this info will be get when that action come 
            msg._coinNeedToSpentIfPalletConflict = 0;
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
        /// Tỉ lệ lá tiếp theo gây ra pallet conflict
        /// </summary>
        public float _conflictPalletPercent;

        /// <summary>
        /// Thông tin về card trên top nếu draw
        /// </summary>
        public int _topDeckCardID;
        public InGameBaseCardEffectID _topDeckCardEffectID;

        /// <summary>
        /// Pallet có conflict ko nếu draw top card?
        /// </summary>
        public bool _willPalletConflictIfDraw;
        public int _coinNeedToSpentIfPalletConflict;
        /// <summary>
        /// Tình trạng pallet hiện tại của bản thân
        /// </summary>
        public List<int> _myPalletCurrentCards;
        public int _totalCoinGainFromThisPallet;
        /// <summary>
        /// Tình trạng hiện tại của các player khác
        /// </summary>
        public Dictionary<int, OtherPlayerLookingInfo> _otherPlayerInfoDic;

        public List<OtherPlayerLookingInfo> _otherPlayerInfoList;
    }
    public class OtherPlayerLookingInfo
    {
        public int _playerID;
        public Dictionary<int, InGame_CardDataModelWithAmount> _bagDic;
        public List<InGame_CardDataModelWithAmount> _bagList;
        public int _totalCard;
        public int _currentCoin;
    }

    /// <summary>
    /// Class giúp AI đưa ra những quyết định dựa theo tình trạng bàn hiện tại, giao tiếp với các class khác qua DecidingMesssage
    /// </summary>
    public class AIDecider
    {
        public InGameBasePlayerItem _player;
        public BaseInGamePlayerDataModel PlayerInfo => _player?.PlayerModel;

        protected LookingMessage _msg;

        public AIDecider() { }
        public AIDecider(InGameBasePlayerItem bot)
        {
            this._player = bot;
        }

        public DecidingMesssage Decide(LookingMessage _cMsg)
        {
            _msg = _cMsg;
            DecidingMesssage dMsg = new DecidingMesssage();
            float randResult = 0;

            //check nếu card tiếp theo gây ra conflict quá cao thì endturn
            randResult = GameUtils.Random;
            dMsg._endTurn = randResult <= _msg._conflictPalletPercent;
            if (dMsg._endTurn)
                return dMsg;

            //nếu lỡ bắt buộc draw, kết quả draw có gây conflict ko?
            dMsg._willPalletConflictIfDraw = !dMsg._endTurn && _msg._willPalletConflictIfDraw;
            dMsg._willSpentTheCoin = DecideToUseCoinIfWeAfford();

            //nếu không endturn => BẮT BUỘC draw card tiếp theo
            //nếu card tiếp theo có effect tương tác: pull card hoặc destroy card thì phải decide là tương tác với ai và card tương tác là card nào
            switch (_msg._topDeckCardEffectID)
            {
                case InGameBaseCardEffectID.NONE_EFFECT:
                    dMsg._interactWithOther = false;
                    break;
                case InGameBaseCardEffectID.ROLL_DICE:
                    dMsg._interactWithOther = false;
                    break;
                case InGameBaseCardEffectID.DRAW_CARD:
                    dMsg._interactWithOther = false;
                    break;
                case InGameBaseCardEffectID.REVEAL_TOP_DECK:
                    dMsg._interactWithOther = false;
                    break;
                case InGameBaseCardEffectID.DESTROY_OTHERS_CARD:
                    dMsg._interactWithOther = true;
                    dMsg._playerIDInteractWith = DecidePlayerToInteractDestroy();
                    dMsg._cardIDInteractWith = DecideCardToInteractDestroy(dMsg._playerIDInteractWith);
                    break;
                case InGameBaseCardEffectID.PULL_CARD_FR_BAG_TO_PALLET:
                    dMsg._interactWithOther = true;
                    dMsg._playerIDInteractWith = this._player.ID;
                    dMsg._cardIDInteractWith = DecideCardToInteractPulling();
                    break;
                default:
                    break;
            }

            return dMsg;


            int DecideCardToInteractPulling()
            {
                //kiểm tra bag của bản thân
                Dictionary<int, InGame_CardDataModelWithAmount> myBagDic = this.PlayerInfo._dictionaryBags;
                List<InGame_CardDataModelWithAmount> myBagList = new List<InGame_CardDataModelWithAmount>(this.PlayerInfo._bag);

                //nếu bag empty => return -1;
                if (myBagList == null || myBagList.Count == 0)
                    return -1;

                if(myBagDic != null)
                {
                    List<InGameBaseCardEffectID> idealCardEffect = new List<InGameBaseCardEffectID>() { InGameBaseCardEffectID.NONE_EFFECT, InGameBaseCardEffectID.REVEAL_TOP_DECK, InGameBaseCardEffectID.DESTROY_OTHERS_CARD };
                    idealCardEffect.Shuffle();

                    //list các card kéo lên sẽ ko gây conflict
                    List<int> notCauseConflictCards = new List<int>();

                    foreach (InGame_CardDataModelWithAmount cardDataModelWithAmount in myBagList)
                    {
                        if (!_msg._myPalletCurrentCards.Contains(cardDataModelWithAmount._cardID))
                            notCauseConflictCards.Add(cardDataModelWithAmount._cardID);
                    }

                    //nếu mọi card đang sở hữu đều gây conflict => pull thằng nào effect k gây hại nhất
                    if(notCauseConflictCards.Count == 0)
                    {
                        foreach (InGameBaseCardEffectID card in idealCardEffect)
                        {
                            InGame_CardDataModelWithAmount c = myBagList.Find(x => InGameUtils.GetActivatorEffectID(x._cardID) == card);
                            if (c != null)
                                return c._cardID;
                        }
                        //Mấy thằng không gây hại mình ko có trong bag, giờ phải return thằng nào mình có, khác pulling nếu có thể
                        return myBagList.GetRandom()._cardID;
                    }
                    //nếu có card ko gây conflict, trong những thằng không gây conflict, pull thằng effect xịn nhất => coin thấp nhất
                    else
                    {
                        notCauseConflictCards.Shuffle();
                        foreach (int cardID in notCauseConflictCards)
                        {
                            InGameBaseCardEffectID effID = InGameUtils.GetActivatorEffectID(cardID);
                            if (idealCardEffect.Contains(effID))
                                return cardID;
                        }
                        //ideal card ko có trong bag thì pull thằng nào trong not cause cũng được
                        return notCauseConflictCards.GetRandom();
                    }
                }

                return 0;
            }

            int DecidePlayerToInteractDestroy()
            {
                List<OtherPlayerLookingInfo> infos = new List<OtherPlayerLookingInfo>(_msg._otherPlayerInfoList);
                if(infos.Count == 1)
                    return infos[0]._playerID;

                //sort player có nhiều card amount nhất mà phang, nếu tie thì lấy thằng nhiều coin hơn, tie nữa thì random
                infos.OrderByDescending(x=>x._totalCard);

                if (infos[0]._totalCard == infos[1]._totalCard)
                {
                    if (infos[0]._currentCoin > infos[1]._currentCoin)
                        return infos[0]._playerID;
                    else if (infos[0]._currentCoin < infos[1]._currentCoin)
                        return infos[1]._playerID;
                    else
                        return infos[Random.Range(0, 1)]._playerID;
                }
                else
                    return infos[0]._playerID;
            }

            int DecideCardToInteractDestroy(int idPlayerDestroy)
            {
                //ưu tiên destroy card có effect xịn tránh trường hợp opp pull lên
                //nếu không có thì destroy card bất kì
                List<InGameBaseCardEffectID> idealCardEffect = new List<InGameBaseCardEffectID>() { InGameBaseCardEffectID.NONE_EFFECT, InGameBaseCardEffectID.REVEAL_TOP_DECK, InGameBaseCardEffectID.DESTROY_OTHERS_CARD };
                idealCardEffect.Shuffle();

                OtherPlayerLookingInfo playerDestroy = _msg._otherPlayerInfoDic[idPlayerDestroy];
                if(playerDestroy != null)
                {
                    foreach (InGameBaseCardEffectID card in idealCardEffect)
                    {
                        InGame_CardDataModelWithAmount c = playerDestroy._bagList.Find(x => InGameUtils.GetActivatorEffectID(x._cardID) == card);
                        if (c != null)
                            return c._cardID;
                    }
                    return playerDestroy._bagList.GetRandom()._cardID;
                }
                return -1;
            }

            ///Nếu đủ coin thì có chi ra cứu pallet ko
            bool DecideToUseCoinIfWeAfford()
            {
                //nếu conflict, có chi coin ko?
                //nếu coin ko đủ cứu => ko chi
                //nếu coin đủ => 
                //1. Nếu trong pallet có card trên goal => cứu
                //2. Nếu ko có card trong goal, nhưng coin thu về từ pallet lớn hơn coin chi ra => cứu
                //3. Nếu cả 2 card ko => random 10-20% cứu
                //1.
                foreach (int cardID in _msg._myPalletCurrentCards)
                {
                    if (this.PlayerInfo.IsCardListedInGoal(cardID))
                    {
                        return true;
                    }
                }

                //2.
                float chanceToUseCoin = 0.1f;
                if (GameUtils.Random <= chanceToUseCoin)
                    return true;

                return false;
            }
        }

        public void EndTurn()
        {
            this._msg = null;
        }
    }
    /// <summary>
    /// Class message AI Decider return ra gửi cho Executor giúp nó thực thi theo ý nó từ những thông tin này
    /// </summary>
    public class DecidingMesssage
    {
        /// <summary>
        /// Không draw nữa mà end turn luôn
        /// </summary>
        public bool _endTurn;

        /// <summary>
        /// nếu card tiếp theo có effect tương tác: pull card hoặc destroy card 
        /// phải decide là tương tác với ai và card tương tác là card nào
        /// </summary>
        public bool _interactWithOther;
        public int _playerIDInteractWith;
        public int _cardIDInteractWith;

        /// <summary>
        /// Pallet có conflict ko nếu draw top card?
        /// </summary>
        public bool _willPalletConflictIfDraw;
        public bool _willSpentTheCoin;
    }

    public class AIExecutor
    {
        protected InGameBasePlayerItem _player;
        public BaseInGamePlayerDataModel PlayerInfo => _player?.PlayerModel;

        protected DecidingMesssage _msg;

        Queue<ExecutionAction> _actions;

        public bool deQueue;

        public AIExecutor(InGameBasePlayerItem _player)
        {
            this._player = _player;
            _actions = new Queue<ExecutionAction>();
            deQueue = false;
        }

        public void SetDecision(DecidingMesssage _msg)
        {
            this._msg = _msg;
            try
            {
                //chờ vài giây
                _actions.Enqueue(new ExecuteThink(this._msg, _player, Random.Range(0.8f, 1.2f)));

                if (_msg._endTurn)
                {
                    _actions.Enqueue(new ExecuteEndTurn(this._msg, _player));
                }
                else
                {
                    //Chờ vài giây
                    _actions.Enqueue(new ExecuteDrawCard(this._msg, _player));
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
                _actions.Dequeue().Do();
            }
        }
        public bool IsHasAction()
        {
            return this._actions.Count > 0;
        }


        public void CheckActionInterractCardIfNeed()
        {
            //Chờ vài giây
            //logic ở đây đang sai, khi card flip xong, callback gọi thì mới execute cái này
            if (_msg._interactWithOther)
            {
                _actions.Enqueue(new ExecuteThink(this._msg, _player, _thinkTime: 3f));
                _actions.Enqueue(new ExecuteChoseCard(this._msg, _player));
            }
        }
        public void CheckActionSpentCoinIfNeed(int coinNeedSpending)
        {
            //logic ở đây đang sai, khi dice roll xong, callback gọi thì mới execute cái này
            if (_msg._willSpentTheCoin && PlayerInfo.IsCanUseGameCoin(coinNeedSpending))
            {
                _actions.Enqueue(new ExecuteThink(this._msg, _player, _thinkTime: 3f));
                _actions.Enqueue(new ExecuteUseCoin(this._msg, _player));
            }
        }
        public void EndTurn()
        {
            _msg = null;
            this._actions.Clear();
            deQueue = false;
        }
    }

    #region Execution Action
    protected abstract class ExecutionAction
    {
        protected DecidingMesssage _msg;

        protected InGameBasePlayerItem _player;

        protected float m_ThinkTime = 0;

        public ExecutionAction()
        { }
        public ExecutionAction(DecidingMesssage _msg, InGameBasePlayerItem _player)
        {
            this._msg = _msg;
            this._player = _player;
        }
        /// <summary>
        /// Thực thi action
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// Cần chờ trước khi thực thi action này
        /// </summary>
        /// <returns></returns>
        public virtual bool Preparing() { return true; }
    }

    protected class ExecuteThink : ExecutionAction
    {
        public ExecuteThink(DecidingMesssage _msg, InGameBasePlayerItem _player) : base(_msg, _player)
        {
            m_ThinkTime = Random.Range(1.0f, 2.0f);
        }

        public ExecuteThink(DecidingMesssage _msg, InGameBasePlayerItem _player, float _thinkTime) : base(_msg, _player)
        {
            m_ThinkTime = _thinkTime;
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

    protected class ExecuteDrawCard : ExecutionAction
    {
        public ExecuteDrawCard(DecidingMesssage _msg, InGameBasePlayerItem _player) : base(_msg, _player)
        {
        }

        public ExecuteDrawCard(DecidingMesssage _msg, InGameBasePlayerItem _player, float _thinkTime) : base(_msg, _player)
        {
        }
        public override void Do() 
        {
            InGameManager.Instance.OnDrawCard();
        }
        public override bool Preparing()
        {
            return true;
        }
    }

    protected class ExecuteChoseCard : ExecutionAction
    {
        public ExecuteChoseCard(DecidingMesssage _msg, InGameBasePlayerItem _player) : base(_msg, _player)
        {
        }

        public ExecuteChoseCard(DecidingMesssage _msg, InGameBasePlayerItem _player, float _thinkTime) : base(_msg, _player)
        {
        }
        public override void Do()
        {
            InGameManager.Instance.OnBotClickChoseToggleBagUIItem(_msg._playerIDInteractWith, _msg._cardIDInteractWith);
        }
        public override bool Preparing()
        {
            return true;
        }
    }

    protected class ExecuteUseCoin : ExecutionAction
    {
        public ExecuteUseCoin(DecidingMesssage _msg, InGameBasePlayerItem _player) : base(_msg, _player)
        {
        }

        public ExecuteUseCoin(DecidingMesssage _msg, InGameBasePlayerItem _player, float _thinkTime) : base(_msg, _player)
        {
        }
        public override void Do()
        {
            InGameManager.Instance.OnUserDecideToUseGameCoin();
        }
        public override bool Preparing()
        {
            return true;
        }
    }

    protected class ExecuteEndTurn : ExecutionAction
    {
        public ExecuteEndTurn(DecidingMesssage _msg, InGameBasePlayerItem _player) : base(_msg, _player)
        {
        }

        public ExecuteEndTurn(DecidingMesssage _msg, InGameBasePlayerItem _player, float _thinkTime) : base(_msg, _player)
        {
        }
        public override void Do()
        {
            InGameManager.Instance.OnUserEndTurn();
        }
        public override bool Preparing()
        {
            return true;
        }
    }

    #endregion Execution Action
}
