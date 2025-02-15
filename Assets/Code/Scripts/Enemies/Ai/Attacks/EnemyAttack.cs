using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Basic settings")]
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;
    [SerializeField] protected LayerMask whatIsTarget;
    [SerializeField] protected float minDistanceToPlayer;     
    [SerializeField] protected float maxDistanceToPlayer;     


    [Tooltip("Position of attack, if left null, defaults to gameObject.transform")]
    [SerializeField] protected Transform attackPos;


    [Header("Attack animation settings")]
    [SerializeField] protected AnimationClip attackAnimation;
    [SerializeField] protected float moveSpeedWhileAttacking = 0f;
    [Tooltip("Time ofter starting animation and activating attack hitbox")]
    [SerializeField] protected float attackDelay = 0f;


    protected float lastPerformedTime;
    protected EnemyNavigation controller;
    
    public AnimationClip AttackAnimation { get => attackAnimation; set => attackAnimation = value; }
    public float MoveSpeedWhileAttacking { get => moveSpeedWhileAttacking; set => moveSpeedWhileAttacking = value; }
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }

    public virtual void SetupAttack(EnemyNavigation controller)
    {
        this.controller = controller;
        lastPerformedTime = Time.time;
        if(attackPos == null)
        {
            attackPos = gameObject.transform;
        }
    }
    public virtual bool CheckAttackConditon()
    {
        if (Time.time < lastPerformedTime + cooldown)
        {
            return false;
        }

        if (!controller.IsTargetPlayer)
        {
            return false;
        }

        if (controller.DistanceToTarget >= maxDistanceToPlayer)
        {
            return false;
        }

        if (controller.DistanceToTarget <= minDistanceToPlayer)
        {
            return false;
        }

        return true;

    }

    public virtual void PerformAttack()
    {
		lastPerformedTime = Time.time;
	}

}
