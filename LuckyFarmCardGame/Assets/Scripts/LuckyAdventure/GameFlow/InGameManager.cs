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
        
        // TODO: Add card palette system to store cards played before resolving
        // Palette will be used to stack cards during Main Phase before resolution
        
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
    
    /// <summary>
    /// Check if all enemies in the current encounter have been defeated
    /// </summary>
    /// <returns>True if all enemies are defeated</returns>
    public bool AreAllEnemiesDefeated()
    {
        // TODO: Implement actual enemy checking logic
        // This is a placeholder - in a real implementation, you would check all active enemies
        // to see if any are still alive
        return false; // Always return false for now
    }
    
    /// <summary>
    /// Add shields to the player based on remaining MP
    /// </summary>
    /// <param name="shieldAmount">Amount of shields to add</param>
    public void AddPlayerShields(int shieldAmount)
    {
        // TODO: Implement actual shield application to the player
        Debug.Log($"Added {shieldAmount} shields to player");
        
        // This would interact with the player unit to apply shields
    }
    
    /// <summary>
    /// Check if the game is over (win or lose condition reached)
    /// </summary>
    /// <returns>True if the game is over</returns>
    public bool IsGameOver()
    {
        // TODO: Implement actual game over checking logic
        // This would check if player has reached victory condition or has been defeated
        return false; // Always return false for now
    }
    
    /// <summary>
    /// Check if end of wave has been reached (all enemies in wave defeated)
    /// </summary>
    /// <returns>True if wave is completed</returns>
    public bool IsWaveCompleted()
    {
        // TODO: Implement actual wave completion checking
        // This would check if all enemies in the current wave are defeated
        return false; // Always return false for now
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
        // 1. Start of Turn - Standby Phase:
        //    - Mind Gauge Determination: Roll 3 dice to establish the player's Mind Points (MP) for the turn
        //    - Apply buff/debuff on player
        //    - Players draw cards from their deck until their hand contains 5 cards
        [Type(typeof(PlayerStandbyTurnState))]
        Player_StandBy_Phase,

        //Play their turn: Planning & Playing cards into palette while they have Mind-Point left
        // 2. Action Phase - Player Main Phase:
        //    - Each card on player hand has an associated MP cost
        //    - Players may play cards as long as they have sufficient MP remaining in their Mind Gauge
        //    - Playing a card subtracts its MP cost from the Mind Gauge and that card will stack in the palette
        //    - Actions continue until the player chooses to end their turn or cannot play any more cards
        [Type(typeof(PlayerMainPhaseTurnState))]
        Main_Phase,

        //Resolving played cards on player palette
        // 3. Player Resolving Phase:
        //    - This phase resolves all cards in the palette
        //    - Any MP left will be converted into shields for defense
        //    - If all enemies were killed in this phase, jump into End of Turn and check end game
        [Type(typeof(PlayerResolvingTurnState))]
        Player_Solving_Phase,
        
        //Play enemy turn: Execute behavior, Attack Player
        // 4. Enemy Action Phase - Enemy Main Phase:
        //    - Apply buff/debuff on enemy, status effects (poison, stun, etc.) take effect if applicable
        //    - Alive enemies execute their defined actions or abilities
        //    - Enemy attacks are applied to the player, reduced by any shield or defensive effects
        [Type(typeof(EnemyMainPhaseTurnState))]
        Enemy_Main_Phase,
        
        //After player resolving phase -> Check win wave: Defeat all enemies in wave -> Check win game: Win all the waves
        // 5. End of Turn:
        //    - Ongoing effects (buffs, debuffs) have their durations reduced by 1
        //    - Checking end wave / end game
        [Type(typeof(PlayerEndTurnState))]
        End_Turn,
        
        //Enemies end their turn, if this is the last enemies -> begin Player Turn else begin next enemies turn
        [Type(typeof(EnemyEndTurnState))]
        Enemy_End_Turn,
    }
}