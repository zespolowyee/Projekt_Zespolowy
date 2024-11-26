using UnityEngine;

[System.Serializable]
public class WalkState : PlayerMoveState
{
    public override void Enter()
    {

    }
    public override void Exit()
    {

    }

    public override void Handle()
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
