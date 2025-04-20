using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class MoveState : PlayerState
{
    [Header("Elements")]
    [SerializeField] private MobileJoystick joystick;

    private CharacterController characterController;
    private float fallVelocity;
    private float gravity = -9.81f;

    private void Start()
    {
        characterController = base.GetComponentInParent<CharacterController>();
    }

    private void ManageMovement()
    {
        // Get movement vector from joystick
        Vector3 vector = joystick.GetMoveVector();
        vector.z = vector.y;
        vector.y = 0f;

        Vector3 moveVector = Vector3.zero;
        float axis = Input.GetAxis("Horizontal");
        float axis2 = Input.GetAxis("Vertical");

        Transform transform = Camera.main.transform;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Normalize forward and right to remove any y-component
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate move vector
        moveVector = (forward * axis2 + right * axis).normalized;

        // Calculate motion
        Vector3 motion = moveVector.normalized * blackboard.speed * Time.fixedDeltaTime;

        // Apply gravity when not grounded
        if (!characterController.isGrounded)
        {
            fallVelocity += gravity * Time.fixedDeltaTime;
        }
        else
        {
            fallVelocity = -2f;  // Reset fall velocity when grounded
        }

        motion.y = fallVelocity * Time.fixedDeltaTime;

        // Move the character
        characterController.Move(motion);

        // Manage animations based on movement
        ManageAnimations(moveVector);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void ManageAnimations(Vector3 moveVector)
    {
        // Set move speed animation parameter
        blackboard.animator.SetFloat("moveSpeed", 1f);

        Vector3 normalized = new Vector3(moveVector.x, 0f, moveVector.z).normalized;

        // Rotate character based on move direction
        blackboard.animator.transform.rotation = Quaternion.LookRotation(normalized);
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        ManageMovement();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        bool isMoving = joystick.GetMoveVector().magnitude > 0.1f;
        bool isIdle = Input.GetAxisRaw("Horizontal") == 0f && Input.GetAxisRaw("Vertical") == 0f;

        // Transition to idle state if no input or joystick movement
        if (!isMoving && isIdle)
        {
            stateMachine.ChangeState(blackboard.idlePlayer);
            return;
        }

        // Handle running state with LeftShift key
        if (Input.GetKey(KeyCode.LeftShift))
        {
            blackboard.speed = 5f;
            blackboard.animator.Play("Run");
            return;
        }

        // Handle walking state
        blackboard.speed = 1.7f;
        blackboard.animator.Play("Walk");
    }
}
