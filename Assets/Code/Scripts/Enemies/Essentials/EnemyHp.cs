using UnityEngine;

public class EnemyHp : HPSystem
{
	public int expReward = 50;
    private GameObject lastAttacker;
    public void TakeDamageFromAttacker(int damage, GameObject attacker)
    {
        lastAttacker = attacker;
        TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();

        if (lastAttacker != null)
        {
            PlayerStats playerStats = lastAttacker.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddEXP(expReward);
                Debug.Log($"{lastAttacker.name} gained {expReward} XP for killing {gameObject.name}.");
            }
        }
    }
}
