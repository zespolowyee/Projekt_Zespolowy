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
            Vector3 shootDirection;
            switch (aimStyle)
            {
                case AimStyle.DirectlyAtPlayer:
                    shootDirection = (controller.Target.transform.position - attackPos.position).normalized;

                    break;
                default:
                    shootDirection = attackPos.transform.forward;
                    break;
            }
            ShootAtTargetClientRpc(shootDirection);
        }

    }

    void ShootAtTarget(Vector3 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPos.position, Quaternion.identity);
        projectile.transform.forward = direction;
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.WhatIsTarget = whatIsTarget;
        projectileScript.Damage = finalDamage;
        projectileScript.Speed = projectileSpeed;
        projectileScript.Lifetime = projectileLifetime;


    }

    [Rpc(SendTo.ClientsAndHost)]
    void ShootAtTargetClientRpc(Vector3 shootDirection)
    {
        
        ShootAtTarget(shootDirection);
    }
}


public enum AimStyle
{
    DirectlyAtPlayer,
    InFrontOfAttackPos
}
