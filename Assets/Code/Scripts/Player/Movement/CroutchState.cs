using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class CroutchState : PlayerMoveState
{
    [SerializeField] private float croutchOffset;
    public override void Enter()
    {
        controller.PlayerBody.transform.localScale = new Vector3(1,0.5f,1);
        controller.PlayerBody.transform.position += new Vector3(0, -croutchOffset, 0);
    }
    public override void Exit()
    {
        controller.PlayerBody.transform.localScale = new Vector3(1, 1, 1);
        controller.PlayerBody.transform.position += new Vector3(0, croutchOffset, 0);
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
