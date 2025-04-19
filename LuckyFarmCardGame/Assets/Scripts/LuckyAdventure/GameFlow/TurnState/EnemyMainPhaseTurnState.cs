using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class EnemyMainPhaseTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter(); 
            Debug.Log($"Enter state EnemyMainPhaseTurnState");
            OnEnterState?.Invoke();
            
            // 4. Enemy Action Phase - Enemy Main Phase from GDD:
            // During this phase, the system will:
            // - Apply buff/debuff on enemies and trigger status effects
            // - Alive enemies execute their defined actions or abilities
            // - Enemy attacks are applied to the player, reduced by any shield or defensive effects
            
            // TODO: Implement status effect application on enemies
            ApplyEnemyStatusEffects();
            
            // TODO: Implement enemy action execution
            ExecuteEnemyActions();
            
            // After all enemy actions are complete, move to the enemy end turn state
            Exit();
        }
        
        private void ApplyEnemyStatusEffects()
        {
            // TODO: Apply all active buffs and debuffs to each enemy
            // This should handle things like poison, stun, etc.
            Debug.Log("Enemy status effects not yet implemented");
        }
        
        private void ExecuteEnemyActions()
        {
            // TODO: Implement enemy action execution based on their defined behavior patterns
            // Different enemy types will have different attack patterns and abilities
            // Attacks should be reduced by player's shield values
            Debug.Log("Enemy actions not yet implemented");
        }

        public override void Exit()
        {
            base.Exit();
            OnExitState?.Invoke();
            //change state
            InGameManager.Instance.ChangeTurnState(TurnState.Enemy_End_Turn);
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

