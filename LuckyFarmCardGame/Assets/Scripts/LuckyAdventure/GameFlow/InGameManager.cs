using System;
using LuckyAdventure.GameUnit;
using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class InGameManager : MonoBehaviour
    {
        public static InGameManager Instance;
        void Awake() => Instance = this;

        public int currentPlayerIndex;
    
        [SerializeField] private GameState _state;
        public GameState CurrentGameState => _state;
        protected IGameState currentGameState;

        [SerializeField] private TurnState _turnState;
        public TurnState CurrentTurnState => _turnState;
        protected ITurnState currentTurnState;

        /// <summary>
        /// Implement this by managing multip-player, if the game allow
        /// In this phase, we only have one main player
        /// </summary>
        public Unit CurrentPlayerTurn => null;

        public static System.Action<GameState> OnGameStateChanged;
        //State, Player Index who taking turn
        public static System.Action<TurnState, int> OnTurnStateChanged;

        public void ChangeGameState(GameState state)
        {
            if (state != GameState.Player_Turn && _state == state)
                return;
            _state = state;
            
            if (currentGameState != null)
                currentGameState.Exit();
            currentGameState = Activator.CreateInstance(EnumUtility.GetStringType(state)) as IGameState;
            currentGameState.Init();
            currentGameState.Enter();

            OnGameStateChanged?.Invoke(state);
        }
        public void ChangeTurnState(TurnState state)
        {
            if (_turnState == state)
                return;

            _turnState = state;

            if(currentTurnState != null)
                currentTurnState.Exit();
            currentTurnState = Activator.CreateInstance(EnumUtility.GetStringType(state)) as ITurnState;
            currentTurnState.Init(CurrentPlayerTurn, this.currentPlayerIndex);
            currentTurnState.Enter();

            OnTurnStateChanged?.Invoke(_turnState, currentPlayerIndex);
        }
        public void ExistState(TurnState state)
        {
            if (_turnState != state)
                return;

            if (currentTurnState != null)
                currentTurnState.Exit();
        }

        public void FinishTurnState(TurnState state)
        {
            if (this.CurrentTurnState != state)
            {
                Debug.Log($"Want to finish unmatch state {CurrentTurnState} -- {state}");
                return;
            }
        }


        public void ResetTheGame()
        {
            //Clear previous state
            ChangeGameState(GameState.Reset_ClearingGame);
        }
        #region Test
        private void Start()
        {
            ChangeGameState(GameState.Init);
        }
        #endregion
    }
    public enum GameState
    {
        None = -1,
        //Prepare the game: Board, Unit,...
        [Type(typeof(GameInitState))]
        Init = 0,
        //Begin the game
        [Type(typeof(GameStartState))]
        Start = 1,

        [Type(typeof(GameEnemyTurnState))]
        //Begin turn of Enemy
        Enemy_Turn = 2,
        //Begin turn of main player and other player, check the Turn Index
        [Type(typeof(GamePlayerTurnState))]
        Player_Turn = 3,

        [Type(typeof(GameEndState))]
        EndGame = 4,

        [Type(typeof(GameResetState))]
        Reset_ClearingGame = 5,
    }
    public enum TurnState
    {
        //Init game
        None = 0,
        
        //Player: Roll and Refresh Mind-Point gauge, execute buff/debuff casted on player, let's player draw cards
        [Type(typeof(PlayerStandbyTurnState))]
        Player_StandBy_Phase,

        //Play their turn: Planning & Playing cards into pallete while they have Mind-Point left
        [Type(typeof(PlayerMainPhaseTurnState))]
        Main_Phase,

        //Resolving played cards on player pallet
        [Type(typeof(PlayerBattleTurnState))]
        Player_Battle_Phase,
        
        
        //Play enemy turn: Execute behavio, Attack Player
        [Type(typeof(EnemyMainPhaseTurnState))]
        Enemy_Main_Phase,
        
        //Player end his turn -> Check win wave: Defeat all enemies in wave -> Check win game: Win all the waves
        [Type(typeof(PlayerEndTurnState))]
        End_Turn,
        //Enemies end their turn, if this is the last enemies -> begin Player Turn else begin next enemies turn
        [Type(typeof(EnemyEndTurnState))]
        Enemy_End_Turn,
    }
}