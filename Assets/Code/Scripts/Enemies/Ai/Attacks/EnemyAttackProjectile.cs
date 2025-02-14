using UnityEngine;

public class EnemyAttackProjectile : EnemyAttack
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    public override void PerformAttack()
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPos.transform.position, Quaternion.identity);
        projectile.transform.forward = attackPos.transform.forward;

    }
}
