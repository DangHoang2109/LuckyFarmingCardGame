using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InGamePlayerConfig;

/// <summary>
/// Manage GameState: WinLose, State, Init Match
/// </summary>
public class InGameManager : MonoSingleton<InGameManager>
{
    #region Property in Inspector
    [SerializeField]
    private CardGameController _gameController;
    public CardGameController GameController => _gameController;

    /// <summary>
    /// Seat for 4 players, but not every match will have enough 4 players
    /// The left over will be disable
    /// </summary>
    [SerializeField]
    protected List<InGameBasePlayerItem> _players;

    protected List<BaseInGamePlayerDataModel> _playersModels;

    [Space(5f)]
    [SerializeField] protected InGameTurnNotification _notificator;
    [Space(5f)]
    [SerializeField] protected InGameWaveTracker _waveTracker;
    public InGameTurnNotification Notificator => _notificator;
    #endregion Property in Inspector

    #region Data
    protected int _turnIndex;
    protected int _enemyWaveIndex = 0; //round=wave enemy hiện tại, 1 wave có thể kéo dài trong nhiều round nếu user không diệt được hết enemy
    public int CurrentWaveIndex => _enemyWaveIndex;
    protected GameState _gameState;
    public bool IsPlaying => this._gameState == GameState.PLAYING;

    public List<int> idsMainPlayersAlive; //main player and his coop/pet
    public List<int> idsEnemysAlive; //Enemy currently active

    /// <summary>
    /// Contain map name, theme, wave ,enemy stat...
    /// </summary>
    protected InGameMapConfig _mapConfig;
    public InGameMapConfig MapConfig => _mapConfig;
    public InGameEnemyWaveConfig CurrentWaveConfig { get; set; }

    /// <summary>
    /// có từng revise chưa
    /// </summary>
    protected bool _isContinued = false;
    #endregion Data

    #region Getter
    public InGameBasePlayerItem CurrentTurnPlayer => _players[this._turnIndex];
    public BaseInGamePlayerDataModel CurrentTurnPlayerModel => _players[this._turnIndex]?.PlayerModel;

    public InGameMainPlayerItem MainUserPlayer => _players[0] as InGameMainPlayerItem;
    public InGameBasePlayerItem FrontEnemy
    {
        get
        {
            for (int i = 1; i < _players.Count; i++)
            {
                if (_players[i].gameObject.activeInHierarchy && !_players[i].isDead())
                    return _players[i];
            }
            Debug.LogError("MAY BE THERE IS NO ENEMY LEFT");
            return null;
        }
    }
    public bool IsHaveEnemy => (this.idsEnemysAlive?.Count ?? 0) > 0; //1 is main player model;
    public List<InGameBasePlayerItem> EnemysAlive
    {
        get
        {
            List< InGameBasePlayerItem > enemies = new List< InGameBasePlayerItem >();
            //get all enemy alive, list có thể bị thay đổi nếu có obj dead 
            for (int i = idsEnemysAlive.Count - 1; i >= 0; i--)
            {
                if (TryGetSeatItem(idsEnemysAlive[i], out InGameBasePlayerItem e) && e.IsPlaying)
                    enemies.Add(e);
            }
            return enemies;
        }
    }
    #endregion Getter

    /// <summary>
    /// Call this function firstly
    /// </summary>
    public void PrepareGame()
    {
        this._gameState = GameState.WAITING;

        //Init Game
        InitGame();
        //Assign Game Rule Component

        //Init Game Controller
        GameController?.AddCallback_CardOnGoingDrawed(this.OnACardGoingBeDrawed);
        GameController?.AddCallback_CardPutToPallet(this.OnLetUserActionWhenCardActiveEffectWhenPutToPallet);
        GameController?.AddCallback_PalletConflict(this.OnPalletConflict);
        GameController?.AddCallback_PalletDestroyed(this.OnPalletDestroyed);
        GameController?.AddCallback_PalletPulledByRule(this.OnPalletPulledByRule);
        GameController?.AddCallback_DiceResultShowed(this.DiceResultShowed);

        GameController?.InitGame(this.MainUserPlayer.DeckConfig);

        //Will be remove when have joingame datas and joingamemanager
        if (TestSceneLoader._isTutorial)
            InitTutorialDeck();

        //Start the game
        StartGame();

        //Will be remove when have joingame datas and joingamemanager
        if (TestSceneLoader._isTutorial)
        {
            DoTutorialAction openBag = new DoTutorialAction(100);
            DoActionManager.Instance.AddAction(openBag);
            DoActionManager.Instance.RunningAction();
        }
    }
    public void InitTutorialDeck()
    {
        List<int> cardPlaceOnTop = new List<int>()
        {
            2, //bucklet
            0, //Sword
            4, //magic eye
        };
        GameController?.PlaceCardOnTopDeck(cardPlaceOnTop);
    }

    protected void InitGame()
    {
        ///Get the map Config
        int mapID = 0;
        this._mapConfig = InGameEnemyConfigs.Instance._mapConfigs.GetMapConfig(mapID);

        int amountPlayerJoin = 1;//Random.Range(2, this._players.Count); //pulling this info fromn JoinGameData outside
        //Init player seat
        InitPlayers(amountPlayerJoin);
        //parse wave tracker
        _waveTracker.ParseData(_mapConfig);

        //inti enemy wave 0;
        NewStage(out _);
    }

    protected void InitPlayers(int amountPlayerJoin)
    {
        _playersModels ??= new List<BaseInGamePlayerDataModel>();
        idsMainPlayersAlive ??= new List<int>();
        if (this._players != null && this._players.Count > 0)
        {
            //Pull a random goal Config to this player;
            for (int i = 0; i < this._players.Count; i++)
            {
                _players[i].gameObject.SetActive(i < amountPlayerJoin);
                if (i >= amountPlayerJoin)
                    continue;

                BaseInGameMainPlayerDataModel main = new BaseInGameMainPlayerDataModel();
                InGamePlayerConfig characterConfig = InGamePlayerConfigs.Instance.GetCharacterConfig(10);
                if (characterConfig != null)
                {
                    main.SetStatConfig(characterConfig);
                    main.SetSeatID(id: i, isMain: i == 0);
                    main.StartGame();

                    _players[i].SetAPlayerModel(main);
                    this._playersModels.Add(main);
                    this.idsMainPlayersAlive.Add(main._seatId);
                }
            }
        }
    }

    protected void NewStage(out bool isBonusStage)
    {
        isBonusStage = false;
        //get wave config
        CurrentWaveConfig = _mapConfig.GetWaveConfig(this._enemyWaveIndex);
        if(CurrentWaveConfig != null)
        {
            if (CurrentWaveConfig.IsBonusStage)
            {
                //hide enemy seat
                for (int i = 1; i < this._players.Count; i++)
                {
                    _players[i].gameObject.SetActive(false);
                }

                this.GameController?.GoShrineBonus();
                isBonusStage = true;
            }
            else
            {
                isBonusStage = false;
                InitCreep(CurrentWaveConfig);
            }
        }
        else
        {
            //có thể hết wave rồi, user đã win -> check lại phát
            if (_enemyWaveIndex >= MapConfig._waveConfigs.Count)
                OnEndGame(isWin: true);
        }

        _waveTracker.NewWave(_enemyWaveIndex);
    }
    public void OnCompleteShowShrine()
    {
        List<BonusStageEffectAct> acts = BonusStageConfigs.Instance.GenerateBonusStageActivator(CurrentWaveConfig._enemyIDsInRound);
        if(acts != null && acts.Count == 2)
        {
            PickingBonusDialog.OnShow().ParseData(acts, EndRoundBonus);
        }
    }
    protected void InitCreep(InGameEnemyWaveConfig waveConfig)
    {
        _playersModels ??= new List<BaseInGamePlayerDataModel>();
        idsEnemysAlive ??= new List<int>();

        for (int i = 0; i < waveConfig._enemyIDsInRound.Count; i++)
        {
            int eID = waveConfig._enemyIDsInRound[i];
            InGameEnemyStatConfig enemyStat = this._mapConfig.GetEnemyStat(enemyID: eID);
            if(enemyStat != null)
            {
                InGameBasePlayerItem seat = this._players.Find(x => !x.IsPlaying);
                if (seat == null)
                    break;
                seat.gameObject.SetActive(true);
                int seatID = _players.IndexOf(seat);
                BaseInGameEnemyDataModel enemyModel = new BaseInGameEnemyDataModel();
                enemyModel.SetSeatID(id: seatID, isMain: false);
                enemyModel.SetStatConfig(enemyStat);
                enemyModel.StartGame();

                seat.SetAPlayerModel(enemyModel);
                this._playersModels.Add(enemyModel);
                idsEnemysAlive.Add(enemyModel._seatId);
            }
        }
    }
    public void SpawnCreep(int enemyID, int amount, int casterSeatID, int turnStunned = 0)
    {
        InGameEnemyStatConfig enemyStat = this._mapConfig.GetEnemyStat(enemyID: enemyID);
        if (enemyStat == null)
            return;

        for (int i = 1; i < this._players.Count; i++)
        {
            if (_players[i].IsPlaying)
                continue;

            int configIndex = i - 1;
            if (enemyStat != null)
            {
                InGameBotPlayerItem seat = _players[i] as InGameBotPlayerItem;

                seat.gameObject.SetActive(true);

                BaseInGameEnemyDataModel enemyModel = new BaseInGameEnemyDataModel();
                enemyModel.SetSeatID(id: i, isMain: false);
                enemyModel.SetStatConfig(enemyStat);
                enemyModel.StartGame();

                seat.SetAPlayerModel(enemyModel);
                this._playersModels.Add(enemyModel);
                idsEnemysAlive.Add(enemyModel._seatId);

                int turnStuntruly = casterSeatID >=i ? turnStunned - 1 : turnStunned; //Nếu vị trí của new enemy spawn ra ở phía trước caster thì bớt 1 turn stun để đều với enemy đứng phía sau
                SetStun(seat, turnStuntruly);
                amount--;
            }

            if (amount <= 0)
                break;
        }
    }
    public void SetStun(int isWhoStunned, int turnStunned = 1)
    {
        if (TryGetSeatItem(isWhoStunned, out InGameBasePlayerItem attacked))
        {
            SetStun(attacked, turnStunned);
        }
    }
    public void SetStun(InGameBasePlayerItem seat, int turnStunned = 1)
    {
        seat.SetStun(turnStunned);
    }
    public void StartGame()
    {
        this._gameState = GameState.PLAYING;
        GameManager.Instance.OnShowDialog<BaseDialog>("Dialogs/RuleSummaryDialog");

        OnBeginRound();
    }

    #region Round Action
    public void OnBeginRound()
    {
        bool isBonusStage = false;
        if (!IsHaveEnemy)
        {
            //sinh thêm creep vì datamodel = 1 -> chỉ còn main player
            //clear model creep đã chết
            this._playersModels.RemoveAll(x => x.IsDead());

            _enemyWaveIndex++;
            NewStage(out isBonusStage);
        }

        if (!isBonusStage)
        {
            //bgein turn, luôn là từ player
            //Roll a turn index 
            this._turnIndex = 0;//Random.Range(0, amountPlayerJoin);
            OnBeginTurn();
        }
    }
    public void OnEndRound()
    {
        //begin new round
        OnBeginRound();
    }
    public void EndRoundBonus()
    {
        this.GameController?.QuitShrineBonus();

        _enemyWaveIndex++;
        NewStage(out _);
        this._turnIndex = 0;//Random.Range(0, amountPlayerJoin);
        OnBeginTurn();
    }
    #endregion Round Action

    #region Turn Action
    public void OnBeginTurn()
    {
        if (!IsPlaying)
            return;
        this.GameController?.BeginTurn(CurrentTurnPlayer.IsMainPlayer);
        Debug.Log($"GAME MANGE: Player seat {this._turnIndex} begin turn");
        CurrentTurnPlayer.BeginTurn();
    }
    public void ShowNotificationCardAction(string text)
    {
        this.Notificator?.ShowText(text, this.CurrentTurnPlayer.IsMainPlayer);
    }
    public void ShowNotificationCardAction(string text, Vector3 casterPosition)
    {
        this.Notificator?.ShowText(text, this.CurrentTurnPlayer.IsMainPlayer, casterPosition);
    }
    public void ShowNotificationCardAction(List<string> texts)
    {
        this.Notificator?.ShowText(texts, this.CurrentTurnPlayer.IsMainPlayer);
    }

    public void OnDrawCard()
    {
        if (!IsPlaying)
            return;
        GameController?.OnDrawACard();
    }
    /// <summary>
    /// called by ingame controller when a card is drawing
    /// </summary>
    private void OnACardGoingBeDrawed()
    {
        this.CurrentTurnPlayer?.OnACardGoingBeDrawed();
    }
    public void OnUserEndTurn()
    {
        //pull the card by user choice if he is main player
        //else if creep just end the turn
        if(this.CurrentTurnPlayer.IsMainPlayer)
            GameController?.PullCardFromPalletToUser(OnPullingAnimationComplete);
        else
            OnLogicEndTurn();

        void OnPullingAnimationComplete(List<InGame_CardDataModel> cardsReceive)
        {
            OnUserPullCardFromPalletToBag(cardsReceive);
            OnLogicEndTurn();
        }
    }
    private void OnUserPullCardFromPalletToBag(List<InGame_CardDataModel> cardsReceive)
    {
        if (!IsPlaying)
            return;
        if (cardsReceive != null && cardsReceive.Count > 0)
        {
            this.CurrentTurnPlayer.PullCardToBag(cardsReceive);
        }
    }


    /// <summary>
    /// User dùng coin để thay đổi kết quả dice, hoặc tùy theo game rule
    /// </summary>
    /// <param name="amountUsing"></param>
    public void OnUserDecideToUseGameCoin(int amountUsing)
    {
        if (!IsPlaying)
            return;
        //trừ coin từ user đang dùng
        if (this.CurrentTurnPlayerModel?.SubtractGameCoinIfCan(amountUsing) ?? false)
        {
            this.GameController?.OnUserDecideToUseGameCoin(amountUsing);
        }
    }
    /// <summary>
    /// User dùng coin để thay đổi kết quả dice, hoặc tùy theo game rule
    /// </summary>
    /// <param name="amountUsing"></param>
    public void OnUserDecideToUseGameCoin()
    {
    }
    /// <summary>
    /// Một card có effect đã được đưa vào trong pallet
    /// inform cho user biết để xử lý
    /// </summary>
    public void OnLetUserActionWhenCardActiveEffectWhenPutToPallet(int cardID)
    {
        this.CurrentTurnPlayer?.Action_ACardPutToPallet(cardID);
    }
    /// <summary>
    /// Một card có effect đã được đưa vào trong pallet
    /// inform cho user biết để xử lý
    /// </summary>
    public void OnLetUserActionWhenPalletConflictAndNeedUseCoin(int amountCoinNeeding, int pointAdding)
    {
        this.CurrentTurnPlayer?.Action_DecideAndUseCoin(amountCoinNeeding, pointAdding);
    }
    /// <summary>
    /// Tấn công một unit
    /// </summary>
    /// <param name="idWhoAttacked"></param>
    /// <param name="damage"></param>
    public void OnPlayerAttacking(int idWhoAttacked, int damage) //int idWhoAttacking: only the current turn user will be able to attack
    {
        if (!IsPlaying)
            return;
        if (TryGetSeatItem(idWhoAttacked, out InGameBasePlayerItem attacked))
        {
            attacked.Attacked(damage, this.OnCallbackPlayerDead);
            //create animation
        }
    }
    /// <summary>
    /// Tấn công toàn bộ đối thủ
    /// </summary>
    /// <param name="isEnemySide">Tấn công toàn bộ enemy hay toàn bộ main player character (future when multiplayer or pet)</param>
    /// <param name="damage"></param>
    public void OnPlayerAttackingAllUnit(bool isEnemySide, int damage) //int idWhoAttacking: only the current turn user will be able to attack
    {
        if (!IsPlaying)
            return;
        if (isEnemySide)
        {
            //get all enemy alive, list có thể bị thay đổi nếu có obj dead 
            for (int i = idsEnemysAlive.Count-1; i >=0; i--)
            {
                OnPlayerAttacking(idsEnemysAlive[i], damage);
            }
        }
        else
        {
            for (int i = idsMainPlayersAlive.Count - 1; i >= 0; i--)
            {
                OnPlayerAttacking(idsMainPlayersAlive[i], damage);
            }
        }
        //create animation
    }
    public void OnPlayerHeal(int idHealReceiver, int heal)
    {
        if (!IsPlaying)
            return;
        if (TryGetSeatItem(idHealReceiver, out InGameBasePlayerItem healReceive))
        {
            healReceive.AddHP(heal);
        }
    }
    public void OnPlayerHealFullHP(int idHealReceiver)
    {
        if (!IsPlaying)
            return;
        if (TryGetSeatItem(idHealReceiver, out InGameBasePlayerItem healReceive))
        {
            healReceive.RecoverFullHP();
        }
    }
    public void OnPlayerIncreaseMaxHP(int idHealReceiver, int hpAdd)
    {
        if (!IsPlaying)
            return;
        if (TryGetSeatItem(idHealReceiver, out InGameBasePlayerItem healReceive))
        {
            healReceive.IncreaseMaxHP(hpAdd, isAddToCurrentToo: true);
        }
    }
    public void OnPlayerDefense(int idDefenseder, int def)
    {
        if (!IsPlaying)
            return;
        if (TryGetSeatItem(idDefenseder, out InGameBasePlayerItem defenseder))
        {
            defenseder.AddShield(def);
        }
    }
    public void OnAPlayerDie(int id)
    {
        if (!IsPlaying)
            return;
        //disable the obj
        //if (TryGetSeatItem(id, out InGameBasePlayerItem dead))
        //{
        //    //dead.Dead(OnCallbackPlayerDead);
        //    if (dead.IsMainPlayer)
        //    {
        //        Debug.Log("PLAYER DEAD");
        //        MainPlayerDied();
        //    }
        //}
    }
    public void OnCallbackPlayerDead(InGameBasePlayerItem dead)
    {
        if (!IsPlaying)
            return;

        this.idsMainPlayersAlive.Remove(dead.SeatID);
        this.idsEnemysAlive.Remove(dead.SeatID);

        //remove from the list -> move to remove when no enemy left
        //RemoveCharacter(dead);

        //check none enemy left -> auto end turn
        if (dead.IsMainPlayer && idsMainPlayersAlive.Count == 0)
        {
            Debug.Log("PLAYER DEAD");
            MainPlayerDied();
            return;
        }
        else
            dead.ClearWhenDead();

        //auto endturn will cause error if card has multiple effect like chain
        //if (!IsHaveEnemy)
        //{
        //    OnUserEndTurn();
        //}

    }
    public bool IsOwningThisCardIDInDeck(int cardID)
    {
        return this.GameController.IsOwningThisCardIDInDeck(cardID);
    }
    public List<int> GetAllCardOwning()
    {
        List < InGame_CardDataModelWithAmount > owning = this.GameController.GetAllCardDeckContain();
        List<int> ints = new List<int>();
        foreach (var item in owning)
        {
            ints.Add(item._cardID);
        }
        return ints;
    }
    public List<int> GetAllCardBonusOwning()
    {
        List<InGame_CardDataModelWithAmount> owning = this.GameController.GetAllCardBonusDeckContain();
        List<int> ints = new List<int>();
        foreach (var item in owning)
        {
            ints.Add(item._cardID);
        }
        return ints;
    }
    public void OnPlayerAddNewCard(int cardID, int amount)
    {
        if (!IsPlaying)
            return;
        GameController.AddCardToDeck(cardID, amount);
    }
    public void OnPlayerAddCardPoint(int cardID, int amount)
    {
        if (!IsPlaying)
            return;
        List<InGame_CardDataModel> cardModel = new List<InGame_CardDataModel>();
        InGame_CardDataModel c = InGameUtils.CreateCardDataModel(cardID);
        c._amount = amount;
        cardModel.Add(c);
        MainUserPlayer.PullCardToBag(cardModel);
    }

    private void OnLogicEndTurn()
    {
        CardGameActionController.Instance.AddCallbackWhenFXComplete(cb: () =>
        {
            CurrentTurnPlayer.EndTurn();

            //else: keep roll index, if end, go next round
            if (_turnIndex == this._playersModels.Count - 1)
            {
                this.OnEndRound();
                return;
            }
            else
            {
                //roll to next index, but caution enemy 1 may dead but enemy 2 still alive
                //we can remove this do while when we apply the roll change enemy position system
                this._turnIndex = InGameUtils.RollIndex(this._turnIndex, this._players.Count);
                Debug.Log($"ROLL TURN INDEX TO {this._turnIndex} in {this._players.Count}");
                while (!this._players[_turnIndex].IsActive() && !this._players[_turnIndex].isDead())
                {
                    if (_turnIndex == this._players.Count - 1)
                    {
                        this.OnEndRound();
                        return;
                    }

                    this._turnIndex = InGameUtils.RollIndex(this._turnIndex, this._players.Count);
                    Debug.Log($"ROLL TURN INDEX TO {this._turnIndex} in {this._players.Count}");
                }

                //begin next user turn
                OnBeginTurn();
            }
        });
    }

    private void Update()
    {
        if(IsPlaying)
            CustomUpdate();
    }
    protected virtual void CustomUpdate()
    {
        this.CurrentTurnPlayer?.CustomUpdate();
    }
    #endregion Turn Actions

    #region Pallet behavior
    public void DiceResultShowed(int diceResult, int pointNeeding, bool willBeDestroy)
    {
    }
    /// <summary>
    /// Normally only Main Player will call this
    /// </summary>
    public void ShowConfirmUsingCoin(int amountCoinNeeding, int pointAdding)
    {
    }
    public void OnPalletConflict()
    {

    }
    public void OnPalletDestroyed()
    {
        OnLogicEndTurn();
    }
    public void OnPalletPulledByRule(List<InGame_CardDataModel> cardsReceive)
    {
        OnUserPullCardFromPalletToBag(cardsReceive);
        OnLogicEndTurn();
    }
    #endregion Pallet behavior

    #region End game behavior
    public void MainPlayerDied()
    {
        OnEndGame(isWin: false);
    }
    public void OnEndGame(bool isWin)
    {
        this._gameState = GameState.ENDING;
        EndGameDialog d = EndGameDialog.ShowEndGameDialog();
        d.ParseData(isWin, _isContinued);
    }
    public void OnContinueGame(float percentHPRecover)
    {
        _isContinued = true;
        int hp = (int)(this.MainUserPlayer.MaxHP * percentHPRecover);
        if(hp > 0)
        {
            this.MainUserPlayer.AddHP(hp);
            this._gameState = GameState.PLAYING;
            OnBeginTurn();
        }
        else
        {
            Debug.LogError($"HP ADD ZERO -hp {hp} -percent {percentHPRecover} -mainMax {MainUserPlayer.MaxHP}");
        }
    }
    #endregion End game behavior

    #region Card activator behavior

    private bool _isPushActionContinueTurn = false;
    public void OnTellControllerContinueTurn()
    {
        if (!IsPlaying)
            return;

        if(!_isPushActionContinueTurn)
        {
            CardGameActionController.Instance.AddCallbackWhenFXComplete(cb: () =>
            {
                this.GameController?.ContinueTurn();
                CurrentTurnPlayer?.ContinueTurn();
                _isPushActionContinueTurn = false;
            });
            _isPushActionContinueTurn = true;
        }
    }
    public void OnTellControllerToRollDice()
    {
        if (!IsPlaying)
            return;
        this.GameController?.RollADiceAndCheckPalletCondition();
    }
    public void OnTellControllerToDrawCards(int cardToDraw)
    {
        if (!IsPlaying)
            return;
        for (int i = 0; i < cardToDraw; i++)
        {
            this.OnDrawCard();
        }
    }
    public void OnTellControllerToRevealTopCard(int cardToReveal)
    {
        if (!IsPlaying)
            return;
        this.GameController?.OnRevealTopDeck(cardToReveal);
    }
    #endregion Card activator behavior

    public bool TryGetSeatItem(int id, out InGameBasePlayerItem item)
    {
        item = this._players.Find(x => x.SeatID == id);
        return item != null;
    }

    #region Bot Looker API Need
    public InGameAI.OtherPlayerLookingInfo GetMainPlayerLookingInfos()
    {
        //get info of the main player only
        InGameMainPlayerItem main = this.MainUserPlayer;
        if (!main.IsPlaying)
            return null;
        BaseInGameMainPlayerDataModel model = main.MainDataModel;
        InGameAI.OtherPlayerLookingInfo info = new InGameAI.OtherPlayerLookingInfo()
        {
            _playerID = model._seatId,
        };
        return info;
    }
    #endregion Bot Looker API Need

    public int GetPlayerBaseStat(PlayerStatID statID)
    {
        switch (statID)
        {
            case PlayerStatID.DAMAGE:
                return MainUserPlayer?.MainDataModel?._statConfig?._baseDamage ?? 0;
            case PlayerStatID.HP:
                return MainUserPlayer?.MainDataModel?._statConfig?._maxHP ?? 0;
            case PlayerStatID.SHIELD:
                return MainUserPlayer?.MainDataModel?._statConfig?._baseShield ?? 0;
            case PlayerStatID.HEAL:
                return MainUserPlayer?.MainDataModel?._statConfig?._baseHeal ?? 0;
            default:
                return 0;
        }
    }
    public int GetPlayerCardCurrentLevel(int cardID)
    {
        return MainUserPlayer?.GetCardLevel(cardID) ?? 1;
    }
}

public enum GameState
{
    WAITING = 0,
    PLAYING = 1,
    PAUSING = 2,
    ENDING = 3,
}