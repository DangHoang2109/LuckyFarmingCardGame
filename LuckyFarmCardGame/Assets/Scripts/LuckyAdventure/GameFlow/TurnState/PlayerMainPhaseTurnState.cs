using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class PlayerMainPhaseTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter();
            OnEnterState?.Invoke();
            
            // 2. Action Phase - Player Main Phase from GDD:
            // During this phase, the player will:
            // - Play cards from their hand as long as they have sufficient MP
            // - Each card played will stack in the palette for later resolution
            // - Playing continues until player chooses to end their turn or cannot play more cards
            
            // TODO: Implement UI controls for:  
            // 1. Displaying available MP in the Mind Gauge
            // 2. Showing cards in hand with their MP costs
            // 3. Allowing player to select and play cards to the palette
            // 4. Showing the palette of cards waiting to be resolved
            // 5. Having an "End Turn" button to manually end the turn
            
            // This state will persist until the player presses End Turn or has no playable cards left
            // The state transition will be triggered by UI events, not immediately after Enter()
            
            Debug.Log("Player Main Phase: Player should now play cards from hand using Mind Points");
        }
        
        // TODO: Implement method to play card from hand to palette
        private void PlayCardToPalette(/* Card card */)
        {
            // 1. Check if player has enough MP to play the card
            // 2. Subtract card's MP cost from Mind Gauge
            // 3. Add card to the palette queue for later resolution
            // 4. Remove card from player's hand
            Debug.Log("PlayCardToPalette not yet implemented");
        }
        
        // This method will be called by the UI when the player clicks End Turn button
        public void OnEndTurnButtonPressed()
        {
            // Transition to the Resolving Phase
            InGameManager.Instance.ChangeTurnState(TurnState.Player_Solving_Phase);
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
