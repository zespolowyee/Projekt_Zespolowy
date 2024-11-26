using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class AirborneState : PlayerMoveState
{
    [SerializeField] private float groundCheckDelay = 0.1f;
    private float remainingDelay;
    public override void Enter()
    {
        CanExit = false;
        remainingDelay = groundCheckDelay;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Handle()
    {
        AirborneMovement();
    }
    public void AirborneMovement()
    {
        remainingDelay -= Time.deltaTime;
        if (remainingDelay < 0)
        {
            CanExit = true;
        }
        Vector2 horizontalInput = controller.HorizontalInput;
        Vector3 moveDirection = (controller.PlayerBody.transform.right * horizontalInput.x + controller.PlayerBody.transform.forward * horizontalInput.y).normalized;

        Vector3 targetVelocity = moveDirection * moveSpeed;
        //Vector3 currentVelocity = new Vector3(controller.Rb.linearVelocity.x, 0, controller.Rb.linearVelocity.z);
        //Vector3 velocityChange = targetVelocity - currentVelocity;

        controller.Rb.AddForce(targetVelocity, ForceMode.Force);

    }
}
