using FP;
using UnityEngine;

public class StateCrouch: IState {
    private Player _player;
    private EventFSM<Player.PlayerStates> _fsm;
    public StateCrouch(Player player, EventFSM<Player.PlayerStates> fsm) {
        _player = player;
        _fsm = fsm;
    }
    
    public void OnEnter() {
        _player.animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_JUMPING, false);
        _player.animator.SetFloat(Constants.CharacterConstants.ANIMATOR_SPEED, 0);
        _player.animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_CROUCHING, true);
        _player.crouch = true;
    }
    
    public void OnUpdate() {
        _player.horizontalMove = _player.playerController.horizontalMove * _player.runSpeed;
        
        if (_player.playerController.horizontalMove != 0 && _player.playerController.keyUpCrouch) {
            _fsm.Feed(Player.PlayerStates.WALKING);
            return;
        }
        else if (_player.playerController.keyUpCrouch){
            _fsm.Feed(Player.PlayerStates.IDLE);
        }
    }
    
    public void OnExit() {
        _player.crouch = false;
        _player.animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_CROUCHING, false);
    }
}