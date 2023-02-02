using FP;

public class StateAttack: IState {
    private Player _player;
    private EventFSM<Player.PlayerStates> _fsm;
    public StateAttack(Player player, EventFSM<Player.PlayerStates> fsm) {
        _player = player;
        _fsm = fsm;
    }
    
    public void OnEnter() {
        // Do something when entering this state
    }
    
    public void OnUpdate() {
        // Do something every frame while in this state
    }
    
    public void OnExit() {
        // Do something when exiting this state
    }
}