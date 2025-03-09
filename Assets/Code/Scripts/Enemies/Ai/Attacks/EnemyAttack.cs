using Unity.Netcode;
using UnityEngine;

public class EnemyAttack : NetworkBehaviour
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

    protected int finalDamage;
    protected float lastPerformedTime;
    protected EnemyNavigation controller;
    
    public AnimationClip AttackAnimation { get => attackAnimation; set => attackAnimation = value; }
    public float MoveSpeedWhileAttacking { get => moveSpeedWhileAttacking; set => moveSpeedWhileAttacking = value; }
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }

    public virtual void SetupAttack(EnemyNavigation controller)
    {
        this.controller = controller;
        lastPerformedTime = Time.time;
        finalDamage = damage;
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
        PlayerHp targetHp = controller.Target.GetComponent<PlayerHp>();
        if (targetHp != null && targetHp.GetCurrentHP() <= 0)
        {
            controller.ClearTarget();
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
        CalculateFinalDamage();
	}

    protected virtual void CalculateFinalDamage()
    {
        finalDamage = damage * (int)controller.StatController.GetNetStatValue(NetStatType.DamageMultipier);
    }
}
