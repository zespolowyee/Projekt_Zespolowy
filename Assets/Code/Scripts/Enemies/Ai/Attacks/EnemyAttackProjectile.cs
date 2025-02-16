using Unity.Netcode;
using UnityEngine;

public class EnemyAttackProjectile : EnemyAttack
{
    [Header("Projectile attack settings")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileLifetime;
    [SerializeField] protected AimStyle aimStyle;

    public override void PerformAttack()
    {
        base.PerformAttack();
        if (IsServer)
        {
            ShootAtTargetClientRpc();
        }

    }

    void ShootAtTarget()
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPos.position, Quaternion.identity);

        switch (aimStyle)
        {
            case AimStyle.DirectlyAtPlayer:
                Vector3 shootDirection = (controller.Target.transform.position - attackPos.position).normalized;
                projectile.transform.forward = shootDirection;

                break;
            default:
                projectile.transform.forward = attackPos.transform.forward;
                break;
        }



        Projectile projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.WhatIsTarget = whatIsTarget;
        projectileScript.Damage = damage;
        projectileScript.Speed = projectileSpeed;
        projectileScript.Lifetime = projectileLifetime;


    }

    [Rpc(SendTo.ClientsAndHost)]
    void ShootAtTargetClientRpc()
    {
        ShootAtTarget();
    }
}


public enum AimStyle
{
    DirectlyAtPlayer,
    InFrontOfAttackPos
}
