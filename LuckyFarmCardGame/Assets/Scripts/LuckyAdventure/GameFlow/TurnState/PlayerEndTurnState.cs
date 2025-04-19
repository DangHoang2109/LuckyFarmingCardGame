using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class PlayerEndTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log($"Enter state PlayerEndTurnState");
            OnEnterState?.Invoke();
            
            // 5. End of Turn from GDD:
            // During this phase, the system will:
            // - Reduce durations of ongoing effects (buffs, debuffs) by 1
            // - Check if the wave is complete (all enemies defeated)
            // - Check if the game is complete (all waves defeated)
            
            // TODO: Implement reduction of durations for status effects
            ReduceStatusEffectDurations();
            
            // Check if wave is complete using InGameManager
            bool isWaveComplete = InGameManager.Instance.IsWaveCompleted();
            if (isWaveComplete)
            {
                // TODO: Implement wave transition mechanics
                // This should handle things like bonus stages, merchant encounters, etc.
                StartNextWave();
            }
            else
            {
                // If wave is not complete, start the next turn
                // If there are still enemies, go to enemy turn
                InGameManager.Instance.ChangeGameState(GameState.Enemy_Turn);
            }
        }
        
        private void ReduceStatusEffectDurations()
        {
            // TODO: Implement reduction of status effect durations
            // Both player and enemy status effects should have their durations reduced by 1
            Debug.Log("Status effect duration reduction not yet implemented");
        }
        

        
        private void StartNextWave()
        {
            // TODO: Implement logic to start the next wave
            // This should handle the 10-stage structure mentioned in the GDD:
            // 4 battle waves - Bonus Stage - 4 battle waves - Boss wave
            Debug.Log("Next wave transition not yet implemented");
            
            // After setting up the next wave, begin with player turn again
            InGameManager.Instance.ChangeGameState(GameState.Player_Turn);
        }
        public override void Exit()
        {
            base.Exit(); Debug.Log($"exit state PlayerEndTurnState");
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
