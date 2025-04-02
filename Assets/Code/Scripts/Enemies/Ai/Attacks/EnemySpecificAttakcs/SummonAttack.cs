using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SummonAttack : EnemyAttack
{

    [Header("Summon attack settings")]
    [SerializeField] private int enemyAmount;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius;

    public override void PerformAttack()
    {
        base.PerformAttack();
        if (IsServer)
        {
            GameObject enemy;
            for (int i = 0; i < enemyAmount; i++)
            {
                Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
                randomPos.y = 0;
                enemy = Instantiate(enemyPrefab, controller.transform.position + randomPos, Quaternion.identity);
                EnemyNavigation enemyAI = enemy.GetComponent<EnemyNavigation>();
                enemyAI.SetPath(controller.EnemyPath);
                enemy.GetComponent<NetworkObject>().Spawn(true);
            }
        }
    }


}
