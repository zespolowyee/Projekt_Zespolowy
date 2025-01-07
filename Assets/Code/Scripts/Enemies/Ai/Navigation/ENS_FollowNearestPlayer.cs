using UnityEngine;
[System.Serializable]
public class ENS_FollowNearestPlayer : EnemyNavigationState
{
	public override void Enter()
	{
		controller.IsTargetPlayer = true;
		base.Enter();
	}

	public override void Exit()
	{
		controller.IsTargetPlayer = false;
		base.Exit();
	}
}
