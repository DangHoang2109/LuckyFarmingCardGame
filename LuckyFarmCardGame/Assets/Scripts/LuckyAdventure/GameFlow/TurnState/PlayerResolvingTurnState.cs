using System.Collections.Generic;
using UnityEngine;

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
            // TODO: Implement resolving all cards from the palette
            // This should execute the effects of each card in the order they were played
            // Cards might deal damage, apply buffs/debuffs, heal, etc.
            Debug.Log("Resolving cards in palette not yet implemented");
        }
        
        private void ConvertRemainingMPToShields()
        {
            // TODO: Implement conversion of any leftover Mind Points to shields
            // This is a key mechanic mentioned in the GDD that rewards efficient MP usage
            Debug.Log("Converting remaining MP to shields not yet implemented");
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
