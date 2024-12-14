using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPath
{
	public List<Transform> Waypoints;
	private int lastVisitedWaypointId = -1;

	public int LastVisitedWaypointId
	{
		get => lastVisitedWaypointId;
		private set => lastVisitedWaypointId = value;
	}

	public void IncrementLastVisitedWaypoint()
	{
		lastVisitedWaypointId++;
	}


	public Transform GetNextWaypoint()
	{
		if(lastVisitedWaypointId + 1 >= Waypoints.Count)
		{
			lastVisitedWaypointId = -1;
		}

		return Waypoints[lastVisitedWaypointId+1];
	}

	public EnemyPath Copy()
	{
		return new EnemyPath
		{
			Waypoints = this.Waypoints,
			LastVisitedWaypointId = this.LastVisitedWaypointId
		};
	}
}
