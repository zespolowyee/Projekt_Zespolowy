using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : State
{
    protected RBController controller;

    [SerializeField] protected float moveSpeed;
    public void Setup(RBController controller)
    {
        this.controller = controller;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Handle()
    {

    }
}
