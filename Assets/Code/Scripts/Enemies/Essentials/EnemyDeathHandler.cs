using UnityEngine;
using System.Linq;
using Unity.Netcode;
public class EnemyDeathHandler : NetworkBehaviour
{
	public int expReward = 50;
    private GameObject lastAttacker;
    private EnemyHp enemyHp;
    public void Start()
    {
        enemyHp = GetComponent<EnemyHp>();
    }
    public void Update()
    {
        if (enemyHp.isDead)
        {
            Die();
        }
    }
    public void SetAttacker(GameObject attacker)
    {
        lastAttacker = attacker;
    }

    public void Die()
    {
        if (lastAttacker != null)
        {
            XPSystem xpsystem = lastAttacker.GetComponent<XPSystem>();
            if (xpsystem != null)
            {
                xpsystem.AddEXP(expReward);
                Debug.Log($"{lastAttacker.name} gained {expReward} XP for killing {gameObject.name}.");
            }
            else
            {
                Debug.LogError("XPSystem not found on attacker! Must be turret");
                DistributeXPAmongAll(expReward);
            }
        }
        if (gameObject != null && IsServer) 
        {
            Debug.Log($"Destroying {gameObject.name} after death.");
            Destroy(gameObject);
        }
    }
    private void DistributeXPAmongAll(int totalExp)
    {
        XPSystem[] allXPSystems = FindObjectsByType<XPSystem>(FindObjectsSortMode.None);
        if (allXPSystems.Length > 0)
        {
            int expPerPlayer = totalExp / allXPSystems.Length; // Podział doświadczenia
            foreach (var xp in allXPSystems)
            {
                xp.AddEXP(expPerPlayer);
                Debug.Log($"{xp.gameObject.name} received {expPerPlayer} XP due to shared reward.");
            }
        }
        else
        {
            Debug.LogWarning("No XPSystem instances found to distribute XP.");
        }
    }
}
