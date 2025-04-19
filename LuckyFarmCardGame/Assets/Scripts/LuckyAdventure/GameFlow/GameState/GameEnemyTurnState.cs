namespace LuckyAdventure.GameFlow
{
    public class GameEnemyTurnState : IGameState
    {
        public override void Enter()
        {
            base.Enter();
            OnEnterState?.Invoke();
            
            //Todo: Implement multiple enemies on wave, begin the first enemy turn
            InGameManager.Instance.ChangeTurnState(TurnState.Enemy_Main_Phase);
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
