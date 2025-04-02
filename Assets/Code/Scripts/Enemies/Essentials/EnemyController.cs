using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
	private Wave wave;
    private EnemyHp enemyHp;
    [SerializeField] private bool countInWave = true;
    public void OnEnable()
	{
        enemyHp = GetComponent<EnemyHp>();
        if (!countInWave)
        {
            return;
        }

        if (enemyHp != null)
        {
            enemyHp.OnEnemyDeath.AddListener(DespawnEnemyServerRpc);
        }
        else
        {
            Debug.LogError($"enemyHp component not found on enemy instance {gameObject.GetInstanceID()}");
        }
    }
    public void SetWaveData(Wave wave, int inWaveId)
	{
		this.wave = wave;
	}

	private void DeleteFromActiveEnemiesInWave()
	{
		wave.MarkEnemyAsDefeated();
	}

	[ServerRpc(RequireOwnership = true)]
	public void DespawnEnemyServerRpc()
	{
        if (!IsServer)
        {
            return;
        }
        DeleteFromActiveEnemiesInWave();
		GetComponent<NetworkObject>().Despawn(true);
	}

	public void DoDespawn()
	{
		DespawnEnemyServerRpc();
	}

}
