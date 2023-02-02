using System.Collections.Generic;
using FP;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController2D))]
public class Player : MonoBehaviour {
    public enum PlayerStates {
        IDLE = 0,
        WALKING,
        CROUCHING,
        JUMPING,
        HOOKING,
        ATTACK,
        DAMAGED,
        DEAD
    }

    public CharacterController2D characterController2D;
    public PlayerController playerController;
    public Animator animator;
    private EventFSM<PlayerStates> _fsm;
    private Dictionary<PlayerStates, IState> _playerStates;

    public float runSpeed = 40f;
    public float horizontalMove { get; set; }
    public bool jump { get; set; }
    public bool crouch { get; set; }

    private void Start() {
        playerController = new PlayerController();

        // var attack = new State<PlayerStates>(PlayerStates.ATTACK.ToString());
        var crouching = new State<PlayerStates>(PlayerStates.CROUCHING.ToString());
        var damaged = new State<PlayerStates>(PlayerStates.DAMAGED.ToString());
        var dead = new State<PlayerStates>(PlayerStates.DEAD.ToString());
        // var hooking = new State<PlayerStates>(PlayerStates.HOOKING.ToString());
        var idle = new State<PlayerStates>(PlayerStates.IDLE.ToString());
        var jumping = new State<PlayerStates>(PlayerStates.JUMPING.ToString());
        var walking = new State<PlayerStates>(PlayerStates.WALKING.ToString());
        _fsm = new EventFSM<PlayerStates>(idle);

        _playerStates = new Dictionary<PlayerStates, IState>();
        _playerStates[PlayerStates.ATTACK] = new StateAttack(this, _fsm);
        _playerStates[PlayerStates.CROUCHING] = new StateCrouch(this, _fsm);
        _playerStates[PlayerStates.DAMAGED] = new StateDamaged(this, _fsm);
        _playerStates[PlayerStates.DEAD] = new StateDead(this, _fsm);
        _playerStates[PlayerStates.HOOKING] = new StateHooking(this, _fsm);
        _playerStates[PlayerStates.IDLE] = new StateIdle(this, _fsm);
        _playerStates[PlayerStates.JUMPING] = new StateJumping(this, _fsm);
        _playerStates[PlayerStates.WALKING] = new StateWalking(this, _fsm);

        // Attack
        // attack.SetTransition(PlayerStates.WALKING, walking);
        // attack.SetTransition(PlayerStates.IDLE, idle);
        // attack.SetTransition(PlayerStates.DAMAGED, damaged);
        //
        // attack.OnEnter += _playerStates[PlayerStates.ATTACK].OnEnter;
        // attack.OnUpdate += _playerStates[PlayerStates.ATTACK].OnUpdate;
        // attack.OnExit += _playerStates[PlayerStates.ATTACK].OnExit;
        
        // Crouch
        crouching.SetTransition(PlayerStates.WALKING, walking);
        crouching.SetTransition(PlayerStates.IDLE, idle);
        crouching.SetTransition(PlayerStates.DAMAGED, damaged);
        
        crouching.OnEnter += _playerStates[PlayerStates.CROUCHING].OnEnter;
        crouching.OnUpdate += _playerStates[PlayerStates.CROUCHING].OnUpdate;
        crouching.OnExit += _playerStates[PlayerStates.CROUCHING].OnExit;

        // Damaged
        damaged.SetTransition(PlayerStates.IDLE, idle);
        damaged.SetTransition(PlayerStates.JUMPING, jumping);
        damaged.SetTransition(PlayerStates.DAMAGED, damaged);
        damaged.SetTransition(PlayerStates.DEAD, dead);

        damaged.OnEnter += _playerStates[PlayerStates.DAMAGED].OnEnter;
        damaged.OnUpdate += _playerStates[PlayerStates.DAMAGED].OnUpdate;
        damaged.OnExit += _playerStates[PlayerStates.DAMAGED].OnExit;

        // Dead
        dead.SetTransition(PlayerStates.IDLE, idle);

        dead.OnEnter += _playerStates[PlayerStates.DEAD].OnEnter;
        dead.OnUpdate += _playerStates[PlayerStates.DEAD].OnUpdate;
        dead.OnExit += _playerStates[PlayerStates.DEAD].OnExit;

        // Hooking
        // hooking.SetTransition(PlayerStates.IDLE, idle);
        // hooking.SetTransition(PlayerStates.JUMPING, jumping);
        // hooking.SetTransition(PlayerStates.DAMAGED, damaged);
        //
        // hooking.OnEnter += _playerStates[PlayerStates.HOOKING].OnEnter;
        // hooking.OnUpdate += _playerStates[PlayerStates.HOOKING].OnUpdate;
        // hooking.OnExit += _playerStates[PlayerStates.HOOKING].OnExit;

        // Idle
        idle.SetTransition(PlayerStates.WALKING, walking);
        idle.SetTransition(PlayerStates.JUMPING, jumping);
        idle.SetTransition(PlayerStates.CROUCHING, crouching);
        // idle.SetTransition(PlayerStates.HOOKING, hooking);
        // idle.SetTransition(PlayerStates.ATTACK, attack);
        idle.SetTransition(PlayerStates.DAMAGED, damaged);

        idle.OnEnter += _playerStates[PlayerStates.IDLE].OnEnter;
        idle.OnUpdate += _playerStates[PlayerStates.IDLE].OnUpdate;
        idle.OnExit += _playerStates[PlayerStates.IDLE].OnExit;

        // Jumping
        jumping.SetTransition(PlayerStates.IDLE, idle);
        // jumping.SetTransition(PlayerStates.HOOKING, hooking);
        jumping.SetTransition(PlayerStates.DAMAGED, damaged);

        jumping.OnEnter += _playerStates[PlayerStates.JUMPING].OnEnter;
        jumping.OnUpdate += _playerStates[PlayerStates.JUMPING].OnUpdate;
        jumping.OnExit += _playerStates[PlayerStates.JUMPING].OnExit;

        // Walking
        walking.SetTransition(PlayerStates.IDLE, idle);
        walking.SetTransition(PlayerStates.JUMPING, jumping);
        walking.SetTransition(PlayerStates.CROUCHING, crouching);
        // walking.SetTransition(PlayerStates.HOOKING, hooking);
        // walking.SetTransition(PlayerStates.ATTACK, attack);
        walking.SetTransition(PlayerStates.DAMAGED, damaged);

        walking.OnEnter += _playerStates[PlayerStates.WALKING].OnEnter;
        walking.OnUpdate += _playerStates[PlayerStates.WALKING].OnUpdate;
        walking.OnExit += _playerStates[PlayerStates.WALKING].OnExit;

        ManagerUpdate.instance.Execute += playerController.Execute;
        ManagerUpdate.instance.Execute += Execute;
        ManagerUpdate.instance.ExecuteFixed += ExecuteFixed;
    }

    private void Execute() {
        _fsm.Update();
#if UNITY_EDITOR
        Logger.Debug.LogColor("Player state: " + _fsm.current.name, "red");
#endif
    }

    private void ExecuteFixed() {
        characterController2D.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
    }

    public void OnLanding() {
        _fsm.Feed(PlayerStates.IDLE);
    }

    public void OnCrouching() {
    }
}