using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class EnemyEndTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log($"Enter state EnemyEndTurnState");
            OnEnterState?.Invoke();
        
            // Enemy End Turn Phase:
            // During this phase, the system will:
            // - Determine if there are more enemies that need to take their turn
            // - If yes, activate the next enemy
            // - If no, begin the player's turn
            
            // TODO: Implement proper enemy turn management for multiple enemies
            // Currently the system assumes all enemies act at once in EnemyMainPhaseTurnState,
            // but in the future we might want individual enemy turns
            bool hasMoreEnemiesForTurn = CheckMoreEnemiesForTurn();
            
            if (hasMoreEnemiesForTurn)
            {
                // If there are more enemies that need to take a turn, activate the next one
                // by returning to the enemy main phase with a different active enemy
                InGameManager.Instance.ChangeTurnState(TurnState.Enemy_Main_Phase);
            }
            else
            {
                // If all enemies have completed their turns, start a new player turn
                InGameManager.Instance.ChangeGameState(GameState.Player_Turn);
            }
        }
        
        private bool CheckMoreEnemiesForTurn()
        {
            // TODO: Implement check for more enemies that need to take their turn
            // This would involve tracking which enemies have already acted this round
            Debug.Log("Enemy turn management not yet implemented");
            
            // For now, return false so we always transition to the player turn
            return false;
        }
        public override void Exit()
        {
            base.Exit(); Debug.Log($"exit state EnemyEndTurnState");
            OnExitState?.Invoke();
        }
        public static System.Action OnEnterState, OnExitState;
        public static void RegisterEnterStateCallback(System.Action cb)
        {
            UnRegisterEnterStateCallback(cb);
            OnEnterState += cb;
        }
        public static void RegisterExitStateCallback(System.Action cb)
        {
            UnRegisterExistStateCallback(cb);
            OnExitState += cb;
        }
        public static void UnRegisterEnterStateCallback(System.Action cb)
        {
            OnEnterState -= cb;
        }
        public static void UnRegisterExistStateCallback(System.Action cb)
        {
            OnExitState -= cb;
        }
    }
}

