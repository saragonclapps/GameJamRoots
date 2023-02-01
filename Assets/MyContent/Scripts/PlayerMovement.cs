using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	private float _horizontalMove = 0f;
	private bool _jump = false;
	private bool _crouch = false;

	private void Start() {
		ManagerUpdate.instance.Execute += Execute;
		ManagerUpdate.instance.ExecuteFixed += ExecuteFixed;
	}

	private void OnDestroy() {
		ManagerUpdate.instance.Execute -= Execute;
		ManagerUpdate.instance.ExecuteFixed -= ExecuteFixed;
	}

	// Update is called once per frame
	void Execute () {

		_horizontalMove = Input.GetAxisRaw(Constants.CharacterConstants.INPUT_HORIZONTAL) * runSpeed;

		animator.SetFloat(Constants.CharacterConstants.ANIMATOR_SPEED, Mathf.Abs(_horizontalMove));

		if (Input.GetButtonDown(Constants.CharacterConstants.INPUT_JUMP))
		{
			_jump = true;
			animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_JUMPING, true);
		}

		if (Input.GetButtonDown(Constants.CharacterConstants.INPUT_CROUCH))
		{
			_crouch = true;
		} else if (Input.GetButtonUp(Constants.CharacterConstants.INPUT_CROUCH))
		{
			_crouch = false;
		}

	}

	public void OnLanding ()
	{
		animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_JUMPING, false);
	}

	public void OnCrouching (bool isCrouching)
	{
		animator.SetBool(Constants.CharacterConstants.ANIMATOR_IS_CROUCHING, isCrouching);
	}

	void ExecuteFixed ()
	{
		// Move our character
		controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
		_jump = false;
	}
}
