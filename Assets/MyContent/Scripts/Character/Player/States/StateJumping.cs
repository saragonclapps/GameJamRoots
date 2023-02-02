using FP;

public class StateJumping: IState {
    private Player _player;
    private EventFSM<Player.PlayerStates> _fsm;
    public StateJumping(Player player, EventFSM<Player.PlayerStates> fsm) {
        _player = player;
        _fsm = fsm;
    }
    
    public void OnEnter() {
        _player.animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_JUMPING, true);
        _player.jump = true;
    }
    
    public void OnUpdate() {
        _player.horizontalMove = _player.playerController.horizontalMove * _player.runSpeed;
    }
    
    public void OnExit() {
        _player.animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_JUMPING, false);
        _player.jump = false;
    }
}