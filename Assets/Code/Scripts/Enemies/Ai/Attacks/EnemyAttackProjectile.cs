using UnityEngine;

public class EnemyAttackProjectile : EnemyAttack
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed;

    public override void PerformAttack()
    {
        base.PerformAttack();
        GameObject projectile = Instantiate(projectilePrefab, attackPos.transform.position, Quaternion.identity);
        projectile.transform.forward = attackPos.transform.forward;

    }
}
