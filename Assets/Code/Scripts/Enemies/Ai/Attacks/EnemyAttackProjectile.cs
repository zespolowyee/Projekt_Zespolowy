using UnityEngine;

public class EnemyAttackProjectile : EnemyAttack
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    public override void PerformAttack()
    {
       
        base.PerformAttack();
    }
}
