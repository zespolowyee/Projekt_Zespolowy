using UnityEngine;

public class EnemyNavigationState : State
{
	public bool CanEnterToItself = false;
	[SerializeField] public float moveSpeed;

	public bool CanExit { get; protected set; }

	protected EnemyNavigationState previousState;
	protected EnemyNavigation controller;
	public virtual void Setup(EnemyNavigation controller)
	{
		this.controller = controller;
		CanExit = true;
	}
	public override void Enter()
	{
		controller.Agent.speed = moveSpeed;
	}
	public virtual void Enter(EnemyNavigationState previousState)
	{
		this.previousState = previousState;
	}
	public override void Exit()
	{
		base.Exit();
	}

	public override void Handle()
	{
	}

}
