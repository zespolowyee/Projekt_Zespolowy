using System.IO;
using UnityEngine;


[System.Serializable]
public class ENS_FollowPath : EnemyNavigationState
{
	private float lastWaypointCheckTime;
	[SerializeField] private float waypontCheckFrequency = 1f;
	[SerializeField] private float desiredDistanceToWaypoint = 3f;
	private float desiredDistanceToWaypointSquared;
	private bool switchedToNextWaypoint = false;

	public override void Setup(EnemyNavigation controller)
	{
		waypontCheckFrequency += Random.Range(-0.1f, 0.1f);
		desiredDistanceToWaypointSquared = desiredDistanceToWaypoint * desiredDistanceToWaypoint;
		base.Setup(controller);
	}

	public override void Enter()
	{
		controller.animator.Animator.SetBool("IsWalking", true);
		lastWaypointCheckTime = Time.time;
		controller.SetTarget(controller.EnemyPath.GetNextWaypoint());
		base.Enter();
	}

	public override void Exit()
	{
		controller.animator.Animator.SetBool("IsWalking", false);
		base.Exit();
	}

	public override void Handle()
	{
		if (lastWaypointCheckTime + waypontCheckFrequency < Time.time)
		{
			CheckIfReachedWaypoint();
		}
	}
	public void GoToNextWaypoint()
	{
		controller.EnemyPath.IncrementLastVisitedWaypoint();
		controller.SetTarget(controller.EnemyPath.GetNextWaypoint());
	}

	public void CheckIfReachedWaypoint()
	{
		lastWaypointCheckTime = Time.time;
		if ((controller.transform.position - controller.Target.position).sqrMagnitude < desiredDistanceToWaypointSquared)
		{
			if (!switchedToNextWaypoint)
			{
				GoToNextWaypoint();
				switchedToNextWaypoint = true;
			}

		}
		else
		{
			switchedToNextWaypoint = false;
		}
	}
}
