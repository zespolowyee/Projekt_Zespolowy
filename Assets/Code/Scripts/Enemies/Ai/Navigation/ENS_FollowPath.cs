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
		waypontCheckFrequency += Random.Range(-0.25f, 0.25f);
		desiredDistanceToWaypointSquared = desiredDistanceToWaypoint * desiredDistanceToWaypoint;
		base.Setup(controller);
	}

	public override void Enter()
	{
		lastWaypointCheckTime = Time.time;
		controller.SetTarget(controller.EnemyPath.GetNextWaypoint());
		base.Enter();
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
