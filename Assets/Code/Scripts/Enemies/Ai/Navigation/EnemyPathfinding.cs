
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private Path path;
    [SerializeField] private float waypontCheckFrequency = 1f;
    private float lastCheckTime;

    private Transform target;
    private bool isTargetWaypoint = true;
    private bool switchedToNextWaypoint = false;
    private NavMeshAgent navMeshAgent;
    void Start()
    {
        waypontCheckFrequency += Random.Range(-0.25f, 0.25f);
        navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.SetDestination(path.GetNextWaypoint().position);
        SetTarget(path.GetNextWaypoint());
        lastCheckTime = Time.time;
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
        if(lastCheckTime + waypontCheckFrequency < Time.time)
        {
            CheckIfReachedWaypoint();
        }
    }


    public void CheckIfReachedWaypoint()
    {
        lastCheckTime = Time.time;
		if (!isTargetWaypoint)
		{
			return;
		}
		if ((transform.position - target.position).sqrMagnitude < path.waypointRadiusSquared)
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
