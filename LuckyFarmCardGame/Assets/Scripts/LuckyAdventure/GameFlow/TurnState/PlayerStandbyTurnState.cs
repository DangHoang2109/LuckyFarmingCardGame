namespace LuckyAdventure.GameFlow
{
    public class PlayerStandbyTurnState : ITurnState
    {
        public override void Enter()
        {
            base.Enter();
            OnEnterState?.Invoke();
        
            //Player: Roll and Refresh Mind-Point gauge, execute buff/debuff casted on player, let's player draw cards
        
            //Complete standby: Begin player's main phase
            InGameManager.Instance.ChangeTurnState(TurnState.Main_Phase);
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
