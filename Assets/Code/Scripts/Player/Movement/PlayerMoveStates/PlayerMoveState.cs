using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : State
{
    protected RBController controller;
    public bool CanExit { get; protected set; }

    [SerializeField] protected float moveSpeed;
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

    }
}
