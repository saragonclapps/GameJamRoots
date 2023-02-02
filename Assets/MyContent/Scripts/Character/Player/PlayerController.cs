using System;
using UnityEngine;

public class PlayerController {
    public bool keyDownHook;
    public bool keyUpHook;
    public bool keyDownAttack;
    public bool keyUpAttack;
    public bool keyDownCrouch;
    public bool keyUpCrouch;
    public bool keyDownJump;
    public bool keyUpJump;
    public float horizontalMove;
    public float verticalMove;


    public void Execute() {
        keyDownHook = Input.GetKeyDown(KeyCode.Mouse1);
        keyUpHook = Input.GetKeyUp(KeyCode.Mouse1);
        
        keyDownAttack = Input.GetKeyDown(KeyCode.Mouse0);
        keyUpAttack = Input.GetKeyUp(KeyCode.Mouse0);
        
        keyDownCrouch = Input.GetButtonDown(Constants.CharacterConstants.INPUT_CROUCH);
        keyUpCrouch = Input.GetButtonUp(Constants.CharacterConstants.INPUT_CROUCH);
        
        keyDownJump = Input.GetButtonDown(Constants.CharacterConstants.INPUT_JUMP);
        keyUpJump = Input.GetButtonUp(Constants.CharacterConstants.INPUT_JUMP);
        
        horizontalMove = Input.GetAxisRaw(Constants.CharacterConstants.INPUT_HORIZONTAL);
        
        verticalMove = Input.GetAxisRaw(Constants.CharacterConstants.INPUT_VERTICAL);
    }
}