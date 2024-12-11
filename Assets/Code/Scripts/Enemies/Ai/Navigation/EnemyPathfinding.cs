
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
	[SerializeField] private Path path;
	[SerializeField] private float waypontCheckFrequency = 1f;
	[SerializeField] private float playerCheckFrequency = 0.4f;
	[SerializeField] private float desiredDistanceToWaypoint = 9f;
	[SerializeField] private float checkForPlayerDistance = 9f;
	[SerializeField] private LayerMask whatIsPlayer;
	private float lastWaypointCheckTime;
	private float lastPlayerCheckTime;

	private Transform target;
	private bool isTargetWaypoint = true;
	private bool switchedToNextWaypoint = false;
	private NavMeshAgent navMeshAgent;
	void Start()
	{
		waypontCheckFrequency += Random.Range(-0.25f, 0.25f);
		playerCheckFrequency += Random.Range(-0.1f, 0.1f);
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.SetDestination(path.GetNextWaypoint().position);
		SetTarget(path.GetNextWaypoint());
		lastWaypointCheckTime = Time.time;
		lastPlayerCheckTime = Time.time;
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
		navMeshAgent.SetDestination(target.position);
	}

	public void GoToNextWaypoint()
	{
		path.IncrementLastVisitedWaypoint();
		SetTarget(path.GetNextWaypoint());
		isTargetWaypoint = true;
	}

	void Update()
	{
		if (lastWaypointCheckTime + waypontCheckFrequency < Time.time)
		{
			CheckIfReachedWaypoint();
		}
		if (lastPlayerCheckTime + playerCheckFrequency < Time.time)
		{
			SearchForPlayerInRange();
		}
	}


	public void CheckIfReachedWaypoint()
	{
		lastWaypointCheckTime = Time.time;
		if (!isTargetWaypoint)
		{
			return;
		}
		if ((transform.position - target.position).sqrMagnitude < desiredDistanceToWaypoint)
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

	public void SearchForPlayerInRange()
	{
		Collider[] playersInReach = Physics.OverlapSphere(transform.position, checkForPlayerDistance, whatIsPlayer);

		if (playersInReach.Length == 0)
		{
			if (!isTargetWaypoint)
			{
				SetTarget(path.GetNextWaypoint());
				isTargetWaypoint = true;
			}
			return;
		}

		Transform closestPlayerTransform = FindClosestPlayer(playersInReach);
		SetTarget(closestPlayerTransform);
		isTargetWaypoint = false;
	}

	public Transform FindClosestPlayer(Collider[] playersInReach)
	{
		float minDistanceSqr = float.MaxValue;
		Collider closestPlayer = playersInReach[0];
		if (playersInReach.Length == 1)
		{
			return closestPlayer.gameObject.transform;
		}
		foreach (Collider player in playersInReach)
		{
			float distanceSquared = (player.gameObject.transform.position - transform.position).sqrMagnitude;

			if (distanceSquared < minDistanceSqr)
			{
				closestPlayer = player;
				minDistanceSqr = distanceSquared;
			}
		}
		return closestPlayer.gameObject.transform;
	}
}
