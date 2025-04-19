using UnityEngine;
using LuckyFantasy; // Add reference to our card namespace

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
        
        // Implemented method to play card from hand to palette
        public void PlayCardToPalette(int cardIndex)
        {
            var cardManager = CardManager.Instance;
            if (cardManager == null)
            {
                Debug.LogError("CardManager not found");
                return;
            }

            // Check if player has enough MP and play the card
            bool cardPlayed = cardManager.PlayCard(cardIndex);

            // if (cardPlayed)
            // {
            //     Debug.Log($"Card at index {cardIndex} played to palette");
            //     
            //     // Check if player has no more playable cards or MP
            //     if (cardManager.GetPlayableCardsCount() == 0)
            //     {
            //         // Automatically end turn if no playable cards remain
            //         Debug.Log("No more playable cards - automatically ending turn");
            //         OnEndTurnButtonPressed();
            //     }
            // }
        }
        
        // Helper method to check if a specific card can be played
        public bool CanPlayCard(int cardIndex)
        {
            var cardManager = CardManager.Instance;
            if (cardManager == null) return false;
            
            return cardManager.CanPlayCardAt(cardIndex);
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
