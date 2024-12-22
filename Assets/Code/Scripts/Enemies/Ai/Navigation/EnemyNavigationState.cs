using UnityEngine;

public class EnemyNavigationState : EnemyState
{
	[SerializeField] public float moveSpeed;
	public override void Enter()
	{
		controller.Agent.speed = moveSpeed;
	}
}
