using UnityEngine;
using System.Linq;
using Unity.Netcode;
public class EnemyDeathHandler : NetworkBehaviour
{
	[SerializeField] private int expReward = 50;
    [SerializeField] private int goldReward = 50;
    [SerializeField] private GameObject goldPickupPrefab;
    private GameObject lastAttacker;
    private EnemyHp enemyHp;
    public void OnEnable()
    {
        enemyHp = GetComponent<EnemyHp>();
        if (enemyHp != null){
            enemyHp.OnEnemyDeath.AddListener(Die);
        }
        else
        {
            Debug.LogError($"enemyHp component not found on enemy instance {gameObject.GetInstanceID()}");
        }
    }

    private void OnDisable()
    {
        if (enemyHp != null)
        {
            enemyHp.OnEnemyDeath.RemoveListener(Die);
        }
        else
        {
            Debug.LogError($"enemyHp component not found on enemy instance {gameObject.GetInstanceID()}");
        }
    }

    public void SetAttacker(GameObject attacker)
    {
        lastAttacker = attacker;
    }

    public void Die()
    {
        if (IsServer)
        {
            DropGold();
        }


        if (lastAttacker != null)
        {
            PlayerStatsDemo xpsystem = lastAttacker.GetComponent<PlayerStatsDemo>();
            if (xpsystem != null)
            {
                xpsystem.AddEXP(expReward);
                Debug.Log($"{lastAttacker.name} gained {expReward} XP for killing {gameObject.name}.");
            }
            else
            {
                Debug.LogWarning("XPSystem not found on attacker! Must be turret");
                DistributeXPAmongAll(expReward);
            }
        }
        else
        {
            Debug.LogWarning("LastAttacker not found! Must be turret");
            DistributeXPAmongAll(expReward);
        }
    }
    private void DistributeXPAmongAll(int totalExp)
    {
        PlayerStatsDemo[] allXPSystems = FindObjectsByType<PlayerStatsDemo>(FindObjectsSortMode.None);
        if (allXPSystems.Length > 0)
        {
            int expPerPlayer = totalExp / allXPSystems.Length;
            foreach (var xp in allXPSystems)
            {
                xp.AddEXP(expPerPlayer);
                Debug.Log($"{xp.gameObject.name} received {expPerPlayer} XP due to shared reward.");
            }
        }
        else
        {
            Debug.LogWarning("No PlayerStats instances found to distribute XP.");
        }
    }

    private void DropGold()
    {
        var pickup = Instantiate(goldPickupPrefab, transform.position, Quaternion.identity);

        var networkObject = pickup.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();

            pickup.GetComponent<GoldPickup>().SetAmount(goldReward);
        }
        else
        {
            Debug.LogError("GoldPickup prefab must have a NetworkObject component!");
        }
    }
}
