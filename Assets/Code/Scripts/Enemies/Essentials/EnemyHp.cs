using UnityEngine;

public class EnemyHp : HPSystem
{
    private EnemyDeathHandler deathHandler;
    public void TakeDamageFromAttacker(int damage, GameObject attacker)
    {
        if (deathHandler == null)
        {
            deathHandler = GetComponent<EnemyDeathHandler>();
        }
        deathHandler.SetAttacker(attacker);
        TakeDamage(damage);
    }
}
