using UnityEngine;

public class EnemyNavigationState : EnemyState
{
	[SerializeField] public float moveSpeed = 0;
	public override void Enter()
	{
		controller.Agent.speed = moveSpeed;
	}
}
