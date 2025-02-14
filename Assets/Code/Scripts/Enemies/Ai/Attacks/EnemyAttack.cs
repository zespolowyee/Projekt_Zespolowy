using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float minDistanceToPlayer;
    [SerializeField] private float maxDistanceToPlayer;
    [SerializeField] public float cooldown;

    [SerializeField] protected Transform attackPos;
    [SerializeField] private float attackRadius;

    [SerializeField] LayerMask whatIsTarget;

    [SerializeField] private float moveSpeedWhileAttacking = 0f;

    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private float attackDelay = 0.1f;


    private float lastPerformedTime;
    private EnemyNavigation controller;
    
    public AnimationClip AttackAnimation { get => attackAnimation; set => attackAnimation = value; }
    public float MoveSpeedWhileAttacking { get => moveSpeedWhileAttacking; set => moveSpeedWhileAttacking = value; }
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }

    public virtual void SetupAttack(EnemyNavigation controller)
    {
        this.controller = controller;
        lastPerformedTime = Time.time;
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
		Collider[] targets = Physics.OverlapSphere(attackPos.position, attackRadius, whatIsTarget);
		foreach (Collider target in targets)
		{
			if (target.gameObject.TryGetComponent<HPSystem>(out var targetHp))
			{
				targetHp.TakeDamage(damage);
			}
		}
	}

}
