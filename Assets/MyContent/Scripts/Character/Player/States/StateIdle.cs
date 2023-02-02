using FP;
using UnityEngine;

public class StateIdle: IState {
    private Player _player;
    private EventFSM<Player.PlayerStates> _fsm;
    public StateIdle(Player player, EventFSM<Player.PlayerStates> fsm) {
        _player = player;
        _fsm = fsm;
    }
    
    public void OnEnter() {
        _player.animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_JUMPING, false);
        _player.animator.SetFloat(Constants.CharacterConstants.ANIMATOR_SPEED, 0);
        // Do something when entering this state
    }
    
    public void OnUpdate() {
        _player.horizontalMove = _player.playerController.horizontalMove * _player.runSpeed;

        if (_player.playerController.horizontalMove != 0) {
            _fsm.Feed(Player.PlayerStates.WALKING);
            return;
        }
        if (_player.playerController.keyDownJump) {
            _fsm.Feed(Player.PlayerStates.JUMPING);
            return;
        }
        if (_player.playerController.keyDownAttack) {
            _fsm.Feed(Player.PlayerStates.ATTACK);
            return;
        }
        if (_player.playerController.keyDownHook) {
            _fsm.Feed(Player.PlayerStates.HOOKING);
            return;
        }
        if (_player.playerController.keyDownCrouch) {
            _fsm.Feed(Player.PlayerStates.CROUCHING);
        }
    }
    
    public void OnExit() {
        // Do something when exiting this state
    }
}