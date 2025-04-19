using System.Collections.Generic;
using UnityEngine;
using LuckyFantasy; // Add reference to our card namespace

namespace LuckyAdventure.GameFlow
{
    public class PlayerResolvingTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter(); 
            Debug.Log($"Enter state PlayerResolvingTurnState");
            OnEnterState?.Invoke();
            
            // 3. Player Resolving Phase from GDD:
            // During this phase, the system will:
            // - Resolve all cards in the palette in the order they were played
            // - Convert any remaining MP into shields for defense
            // - Check if all enemies were killed, and if so, jump to End of Turn
            
            // TODO: Implement resolving mechanism for cards in the palette
            ResolveCardsInPalette();
            
            // TODO: Implement conversion of remaining MP to shields
            ConvertRemainingMPToShields();
            
            // Begin Enemies turn
            InGameManager.Instance.ChangeTurnState(TurnState.End_Turn);
        }
        
        private void ResolveCardsInPalette()
        {
            var cardManager = CardManager.Instance;
            if (cardManager != null)
            {
                cardManager.ResolveAllCards();
                Debug.Log("Cards in palette resolved");
                
                InGameManager.Instance.ChangeTurnState(TurnState.End_Turn);
            }
            else
            {
                Debug.LogError("CardManager not found - cannot resolve cards in palette");
            }
        }
        

        
        private void ConvertRemainingMPToShields()
        {
            var mindPointsSystem = MindPointsSystem.Instance;
            if (mindPointsSystem != null)
            {
                int remainingMP = mindPointsSystem.CurrentMP;
                if (remainingMP > 0)
                {
                    // TODO: Implement proper shield creation for the player
                    // For now, we'll just log the conversion
                    Debug.Log($"Converting {remainingMP} MP to shields for the player");
                    
                    // Reset MP after conversion
                    mindPointsSystem.ResetMP();
                }
                else
                {
                    Debug.Log("No MP remaining to convert to shields");
                }
            }
            else
            {
                Debug.LogError("MindPointsSystem not found - cannot convert MP to shields");
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
