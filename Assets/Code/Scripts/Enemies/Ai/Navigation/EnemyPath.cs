using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPath : MonoBehaviour
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

	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;

		for (int i = 1; i< Waypoints.Count; i++)
		{
			Gizmos.DrawLine(Waypoints[i - 1].position, Waypoints[i].position);
		}

	}
}
