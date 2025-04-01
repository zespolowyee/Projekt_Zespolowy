
using System.ComponentModel.Design.Serialization;
using System.IO;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;


public class EnemyNavigation : NetworkBehaviour
{
    private EnemyPath path;

    [SerializeField] private float playerCheckFrequency = 0.3f;
    [SerializeField] private float checkForPlayerDistance = 9f;
    [SerializeField] private LayerMask whatIsPlayer;
    private float lastPlayerCheckTime;
    public NetworkAnimator animator;
    private NetStatController statController;

    [Header("State Setup")]
    [SerializeField] private ENS_FollowPath followPathState;
    [SerializeField] private ENS_FollowNearestPlayer followNearestPlayerState;
    [SerializeField] private EnemyAttackState attackState;
    private ENS_IdleState idleState;
    public EnemyState CurrentState { get; private set; }

    private Transform target;
    public bool IsTargetPlayer { get; set; }

    private NavMeshAgent navMeshAgent;
    public NavMeshAgent Agent { get { return navMeshAgent; } }
    public EnemyPath EnemyPath
    {
        get { return path; }
        private set { path = value; }
    }
    public Transform Target { get { return target; } }

    public float DistanceToTarget { get; set; }
    public NetStatController StatController { get => statController; set => statController = value; }

    void Start()
    {
        if (!IsServer)
        {
            return;
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        statController = GetComponent<NetStatController>();

        animator = GetComponent<NetworkAnimator>();
        playerCheckFrequency += Random.Range(-0.1f, 0.1f);


        followPathState.Setup(this);
        followNearestPlayerState.Setup(this);
        attackState.Setup(this);
        idleState = new ENS_IdleState();
        idleState.Setup(this);

        lastPlayerCheckTime = Time.time;

        SwitchState(followPathState);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        navMeshAgent.SetDestination(target.position);
    }
    public void ClearTarget()
    {
        this.target = null; 
        IsTargetPlayer = false;
    }

    public void SetPath(EnemyPath enemyPath)
    {
        EnemyPath = enemyPath.Copy();
    }
    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        CurrentState.Handle();

        if (lastPlayerCheckTime + playerCheckFrequency < Time.time)
        {
            lastPlayerCheckTime = Time.time;
            Debug.Log("makaka");
            if (attackState.CheckAllConditions())
            {
                SwitchState(attackState);
            }

            SearchForPlayerInRange();
        }





    }

    public void CalculateDistanceToTarget()
    {
        DistanceToTarget = Vector3.SqrMagnitude(transform.position - target.position);
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
        CalculateDistanceToTarget();
        Transform closestPlayerTransform = FindClosestPlayer(playersInReach);
        SetTarget(closestPlayerTransform);

        SwitchState(followNearestPlayerState);
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


    public void SwitchState(EnemyState newState, bool ignoreCanExitClause = false)
    {
        if(CurrentState == null){
            CurrentState = newState;
            CurrentState.Enter();
            return;
        }

        if (!CurrentState.CanExit && !ignoreCanExitClause)
        {
            return;
        }

        if (!newState.CanEnterToItself && CurrentState == newState)
        {
            return;
        }

        //Wykonujemy metody na wyj�ciu i wej�ciu do nowego stanu je�li on si� zmieni�

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        return;
    }

}
