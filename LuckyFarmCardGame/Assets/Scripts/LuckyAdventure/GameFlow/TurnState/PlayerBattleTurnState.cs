using System.Collections.Generic;
using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class PlayerBattleTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter(); Debug.Log($"Enter state PlayerBattleTurnState");
            OnEnterState?.Invoke();
            //Resolving played cards on player pallet
            
            //resolve complete: Move to player's end turn
            InGameManager.Instance.ChangeTurnState(TurnState.End_Turn);
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
