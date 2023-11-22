using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

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
    public InGameTurnNotification Notificator => _notificator;
    #endregion Property in Inspector

    #region Data
    protected int _turnIndex;
    protected int _enemyWaveIndex = 0; //round=wave enemy hiện tại, 1 wave có thể kéo dài trong nhiều round nếu user không diệt được hết enemy

    protected GameState _gameState;
    public bool IsPlaying => this._gameState == GameState.PLAYING;

    public List<int> idsMainPlayersAlive; //main player and his coop/pet
    public List<int> idsEnemysAlive; //Enemy currently active

    /// <summary>
    /// Contain map name, theme, wave ,enemy stat...
    /// </summary>
    protected InGameMapConfig _mapConfig;
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


    private void Start()
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

        //Start the game
        StartGame();
    }

    protected void InitGame()
    {
        ///Get the map Config
        int mapID = 0;
        this._mapConfig = InGameEnemyConfigs.Instance._mapConfigs.GetMapConfig(mapID);

        int amountPlayerJoin = 1;//Random.Range(2, this._players.Count); //pulling this info fromn JoinGameData outside
        Debug.Log($"INGAME MANGE: Init {amountPlayerJoin} players");
        //Init player seat
        InitPlayers(amountPlayerJoin);
        //inti enemy wave 0;
        NewWaveAndSpawnEnemy();
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

    protected void NewWaveAndSpawnEnemy()
    {
        //get wave config
        InGameEnemyWaveConfig waveConfig = _mapConfig.GetWaveConfig(this._enemyWaveIndex);
        if(waveConfig != null)
        {
            InitCreep(waveConfig);
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

        //if (this._players != null && this._players.Count > 0)
        //{
        //    int creepIndexMax = waveConfig.AmountEnemy + 1;
        //    for (int i = 1; i < this._players.Count; i++)
        //    {
        //        _players[i].gameObject.SetActive(i < creepIndexMax);
        //        if (i >= creepIndexMax)
        //            continue;

        //        int configIndex = i - 1;
        //        InGameEnemyStatConfig enemyStat = waveConfig.GetEnemyStat(configIndex);
        //        if(enemyStat != null)
        //        {
        //            BaseInGameEnemyDataModel enemyModel = new BaseInGameEnemyDataModel();
        //            enemyModel.SetSeatID(id: i, isMain: false);
        //            enemyModel.SetStatConfig(enemyStat);
        //            enemyModel.StartGame();

        //            _players[i].SetAPlayerModel(enemyModel);
        //            this._playersModels.Add(enemyModel);
        //            idsEnemysAlive.Add(enemyModel._seatId);
        //        }

        //    }
        //}
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
                seat.SetStun(turnStuntruly);
                amount--;
            }

            if (amount <= 0)
                break;
        }
    }
    public void StartGame()
    {
        this._gameState = GameState.PLAYING;

        OnBeginRound();
    }

    #region Round Action
    public void OnBeginRound()
    {
        if (!IsHaveEnemy)
        {
            //sinh thêm creep vì datamodel = 1 -> chỉ còn main player
            //clear model creep đã chết
            this._playersModels.RemoveAll(x => x.IsDead());

            _enemyWaveIndex++;
            NewWaveAndSpawnEnemy();
        }

        //bgein turn, luôn là từ player
        //Roll a turn index 
        this._turnIndex = 0;//Random.Range(0, amountPlayerJoin);
        OnBeginTurn();

    }
    public void OnEndRound()
    {
        //loop all the alive player and reset it shield, we not allow shield remain 
        foreach (var item in this._players)
        {
            if (item.IsActive())
                item.ResetShield();
        }
        //begin new round
        OnBeginRound();
    }
    #endregion Round Action

    #region Turn Action
    public void OnBeginTurn()
    {
        this.GameController?.BeginTurn(CurrentTurnPlayer.IsMainPlayer);
        Debug.Log($"GAME MANGE: Player seat {this._turnIndex} begin turn");
        CurrentTurnPlayer.BeginTurn();
    }
    public void ShowNotificationCardAction(string text)
    {
        this.Notificator?.ShowText(text, this.CurrentTurnPlayer.IsMainPlayer);
    }

    public void OnDrawCard()
    {
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
        //trừ coin từ user đang dùng
        if(this.CurrentTurnPlayerModel?.SubtractGameCoinIfCan(amountUsing) ?? false)
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
        if (TryGetSeatItem(idWhoAttacked, out InGameBasePlayerItem attacked))
        {
            attacked.Attacked(damage, this.OnCallbackPlayerDead);
            //create animation
        }
        else
        {
            //hết enemy
        }

    }
    /// <summary>
    /// Tấn công toàn bộ đối thủ
    /// </summary>
    /// <param name="isEnemySide">Tấn công toàn bộ enemy hay toàn bộ main player character (future when multiplayer or pet)</param>
    /// <param name="damage"></param>
    public void OnPlayerAttackingAllUnit(bool isEnemySide, int damage) //int idWhoAttacking: only the current turn user will be able to attack
    {
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
        if (TryGetSeatItem(idHealReceiver, out InGameBasePlayerItem healReceive))
        {
            healReceive.AddHP(heal);
        }
    }
    public void OnPlayerDefense(int idDefenseder, int def)
    {
        if (TryGetSeatItem(idDefenseder, out InGameBasePlayerItem defenseder))
        {
            defenseder.AddShield(def);
        }
    }
    public void OnAPlayerDie(int id)
    {            
        //disable the obj
        if (TryGetSeatItem(id, out InGameBasePlayerItem dead))
        {
            //dead.Dead(OnCallbackPlayerDead);
        }
    }
    public void OnCallbackPlayerDead(InGameBasePlayerItem dead)
    {
        this.idsMainPlayersAlive.Remove(dead.SeatID);
        this.idsEnemysAlive.Remove(dead.SeatID);
        dead.ClearWhenDead();

        //remove from the list -> move to remove when no enemy left
        //RemoveCharacter(dead);
    }
    private void RemoveCharacter(InGameBasePlayerItem dead)
    {
        this._playersModels.RemoveAll(x => x._seatId == dead.SeatID);
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
                while (!this._players[_turnIndex].IsActive())
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
    /// <summary>
    /// Bật UI chọn card, simulate hành động click chọn của bot
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="cardID"></param>
    public void OnBotClickChoseToggleBagUIItem(int playerID, int cardID)
    {
        if(TryGetSeatItem(playerID, out InGameBasePlayerItem player))
        {
            Debug.Log("COMMENTED THIS, CONSIDER MOVE THIS LOGIC TO MAIN PLAYER ONLY");
            //if(player.BagVisual.TryFindUIItem(cardID, out InGameBagCardTypeUIItem uiCardItem))
            //{
            //    uiCardItem.ClickToggleFromMManager();
            //}
        }
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
    public void OnEndGame()
    {
        Debug.Log($"INGAME MANGE: End Game");
        this._gameState = GameState.ENDING;

    }
    #endregion End game behavior

    #region Card activator behavior
    public void OnTellControllerContinueTurn()
    {
        this.GameController?.ContinueTurn();
        CurrentTurnPlayer?.ContinueTurn();

    }
    public void OnTellControllerToRollDice()
    {
        this.GameController?.RollADiceAndCheckPalletCondition();
    }
    public void OnTellControllerToDrawCards(int cardToDraw)
    {
        for (int i = 0; i < cardToDraw; i++)
        {
            this.OnDrawCard();
        }
    }
    public void OnTellControllerToRevealTopCard(int cardToReveal)
    {
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
}

public enum GameState
{
    WAITING = 0,
    PLAYING = 1,
    PAUSING = 2,
    ENDING = 3,
}