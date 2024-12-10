using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class CroutchState : PlayerMoveState
{
    [SerializeField] private float croutchOffset;
    [SerializeField] private float cellingCheckRadius;
    public override void Enter(PlayerMoveState previousState)
    {
        controller.PlayerBody.transform.localScale = new Vector3(1,0.5f,1);
        controller.PlayerBody.transform.position += new Vector3(0, -croutchOffset, 0);
        base.Enter(previousState);
    }
    public override void Exit()
    {
        controller.PlayerBody.transform.localScale = new Vector3(1, 1, 1);
        controller.PlayerBody.transform.position += new Vector3(0, croutchOffset, 0);
    }

    public override void Handle()
    {
        Collider[] collidersAbove = Physics.OverlapSphere(controller.HeadCheck.transform.position, cellingCheckRadius, controller.GroundLayer);
        if (collidersAbove.Length != 0)
        {
            CanExit = false;
        }
        else
        {
            CanExit = true;
        }
        base.Handle();
    }
}
