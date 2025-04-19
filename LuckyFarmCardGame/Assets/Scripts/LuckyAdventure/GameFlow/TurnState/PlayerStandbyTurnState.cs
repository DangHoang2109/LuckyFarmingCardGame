using LuckyFantasy; // Add reference to our card namespace

namespace LuckyAdventure.GameFlow
{
    public class PlayerStandbyTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter();
            OnEnterState?.Invoke();
        
            // 1. Start of Turn - Standby Phase steps from GDD:
            
            // Step 1: Mind Gauge Determination - Roll 3 dice to establish the player's Mind Points (MP)
            // Use our MindPointsSystem for proper dice rolling mechanics
            MindPointsSystem.Instance.RollForMindPoints();
            
            // Step 2: Apply buff/debuff on player
            // TODO: Implement buff/debuff system for the player
            ApplyPlayerStatusEffects();
            
            // Step 3: Players draw cards from their deck until their hand contains 5 cards
            // TODO: Implement card drawing system
            DrawPlayerCards();
            
            //Complete standby: Begin player's main phase
            InGameManager.Instance.ChangeTurnState(TurnState.Main_Phase);
        }
        
        private void ApplyPlayerStatusEffects()
        {
            // TODO: Apply all active buffs and debuffs to the player at the beginning of their turn
            // This should handle things like poison, regeneration, strength buffs, etc.
            UnityEngine.Debug.Log("Player status effects not yet implemented");
        }
        
        private void DrawPlayerCards()
        {
            // Use CardManager to handle card drawing logic
            var cardManager = CardManager.Instance;
            if (cardManager != null)
            {
                cardManager.DrawCardsToHandLimit();
                UnityEngine.Debug.Log("Cards drawn to hand limit");
            }
            else
            {
                // Fallback if CardManager is not available
                UnityEngine.Debug.Log("Card drawing system not yet implemented - CardManager not found");
            }
        }

        public override void Exit()
        {
            base.Exit();
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
