using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class PlayerEndTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter();
            OnEnterState?.Invoke();
            //Player end his turn -> Check win wave: Defeat all enemies in wave -> Check win game: Win all the waves

            //If still have enemies left: Begin enemies turn
            InGameManager.Instance.ChangeGameState(GameState.Enemy_Turn);
            //else go to next wave
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
