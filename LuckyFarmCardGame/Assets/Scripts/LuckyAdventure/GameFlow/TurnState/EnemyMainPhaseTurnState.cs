using UnityEngine;

namespace LuckyAdventure.GameFlow
{
    public class EnemyMainPhaseTurnState : ITurnState
    {
        void EnemyCompletePlayTurn()
        {
            Exit();
        }
        void EnemyPlayHisMainTurn()
        {
        }
        public override void Enter()
        {
            base.Enter(); Debug.Log($"Enter state EnemyMainPhaseTurnState");
            OnEnterState?.Invoke();
            EnemyPlayHisMainTurn();
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

