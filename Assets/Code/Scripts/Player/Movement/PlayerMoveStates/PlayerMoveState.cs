using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : State
{

	[SerializeField] public float moveSpeed;

	public bool CanExit { get; protected set; }
	public bool CanEnterToItself = false;
	protected PlayerMoveState previousState;
	protected RBController controller;
	public void Setup(RBController controller)
    {
        this.controller = controller;
        CanExit = true;
    }
    public override void Enter()
    {

    }
    public virtual void Enter(PlayerMoveState previousState)
    {
        this.previousState = previousState;
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Handle()
    {
        HandleBasicHorizontalMovement();
    }

    protected void HandleBasicHorizontalMovement()
    {

		Vector2 horizontalInput = controller.HorizontalInput;
        Vector3 moveDirection = (controller.PlayerBody.transform.right * horizontalInput.x + controller.PlayerBody.transform.forward * horizontalInput.y).normalized;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, controller.SlopeNormal);

        Vector3 targetVelocity = moveDirection * moveSpeed;
        Vector3 currentVelocity = new Vector3(controller.Rb.linearVelocity.x, 0, controller.Rb.linearVelocity.z);
        Vector3 velocityChange = targetVelocity - currentVelocity;
        if (velocityChange.y > 0)
        {
            velocityChange.y = 0;
        }

        controller.Rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
    }
}
