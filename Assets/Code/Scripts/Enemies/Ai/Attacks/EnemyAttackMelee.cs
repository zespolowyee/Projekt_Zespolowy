using UnityEngine;

public class EnemyAttackMelee : EnemyAttack
{

    [SerializeField] private float attackRadius;

    public override void PerformAttack()
    {

        base.PerformAttack();
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
