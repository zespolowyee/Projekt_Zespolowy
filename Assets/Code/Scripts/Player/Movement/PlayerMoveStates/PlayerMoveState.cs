using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : State
{
    protected RBController controller;
    public bool CanExit { get; protected set; }

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float smoothingFactor;
    public void Setup(RBController controller)
    {
        this.controller = controller;
        CanExit = true;
    }
    public override void Enter()
    {

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
        /*
        Vector2 horizontalInput = controller.HorizontalInput;

        Vector3 moveDirection = (controller.PlayerBody.transform.right * horizontalInput.x
                                + controller.PlayerBody.transform.forward * horizontalInput.y).normalized;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, controller.SlopeNormal);

        Vector3 targetVelocity = moveDirection * moveSpeed;

        Vector3 currentVelocity = new Vector3(controller.Rb.linearVelocity.x, 0, controller.Rb.linearVelocity.z);

        Vector3 smoothedVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * smoothingFactor);

        Vector3 velocityChange = smoothedVelocity - currentVelocity;

        velocityChange.y = 0;

        // Zastosuj si³ê
        controller.Rb.AddForce(velocityChange, ForceMode.VelocityChange);
        */

        
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
