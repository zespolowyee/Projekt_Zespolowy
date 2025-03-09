using UnityEngine;
[System.Serializable]
public class ENS_FollowNearestPlayer : EnemyNavigationState
{
	public override void Enter()
	{
		controller.animator.Animator.SetBool("IsWalking", true);
		controller.IsTargetPlayer = true;
		base.Enter();
	}

	public override void Exit()
	{
		controller.animator.Animator.SetBool("IsWalking", false);
		controller.IsTargetPlayer = false;
		base.Exit();
	}
}
