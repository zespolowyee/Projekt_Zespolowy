using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;


public class EnemyNavigation : NetworkBehaviour
{
	[SerializeField] private EnemyPath path;
	private int lastVisitedWaypointId = -1;

	[SerializeField] private float playerCheckFrequency = 0.4f;
	[SerializeField] private float checkForPlayerDistance = 9f;
	[SerializeField] private LayerMask whatIsPlayer;
	private float lastPlayerCheckTime;


	[Header("State Setup")]
	[SerializeField] private ENS_FollowPath followPathState;
	[SerializeField] private ENS_FollowNearestPlayer followNearestPlayer;

	public EnemyNavigationState CurrentState { get; private set; }

	private Transform target;

	private NavMeshAgent navMeshAgent;
	public NavMeshAgent Agent { get { return navMeshAgent; }}
	public EnemyPath EnemyPath
	{
		get { return path; }
		private set { path = value; }
	}
	public Transform Target { get { return target; } }
	
	void Start()
	{

		playerCheckFrequency += Random.Range(-0.1f, 0.1f);
		navMeshAgent = GetComponent<NavMeshAgent>();

		followPathState.Setup(this);
		followNearestPlayer.Setup(this);

		lastPlayerCheckTime = Time.time;

		SwitchState(followPathState);
	}

	public void IncrementLastVisitedWaypoint()
	{
		lastVisitedWaypointId++;
	}
	public Transform GetNextWaypoint()
	{
		if (lastVisitedWaypointId + 1 >= path.Waypoints.Count)
		{
			lastVisitedWaypointId = -1;
		}

		return path.Waypoints[lastVisitedWaypointId + 1];
	}


	public void SetTarget(Transform target)
	{
		this.target = target;
		navMeshAgent.SetDestination(target.position);
	}

	public void SetPath(EnemyPath enemyPath)
	{
		EnemyPath = enemyPath; 
	}
	void Update()
	{
		CurrentState.Handle();
		if (lastPlayerCheckTime + playerCheckFrequency < Time.time)
		{
			SearchForPlayerInRange();
		}
	}


	public void SearchForPlayerInRange()
	{
		Collider[] playersInReach = Physics.OverlapSphere(transform.position, checkForPlayerDistance, whatIsPlayer);

		if (playersInReach.Length == 0)
		{
			if (CurrentState != followPathState)
			{
				SwitchState(followPathState);
			}
			return;
		}

		Transform closestPlayerTransform = FindClosestPlayer(playersInReach);
		SetTarget(closestPlayerTransform);
		SwitchState(followNearestPlayer);
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

	public void SwitchState(EnemyNavigationState newState)
	{
		CurrentState?.Exit();
		CurrentState = newState;
		CurrentState.Enter();
	}
}
