using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
	private Wave wave;

	public void SetWaveData(Wave wave, int inWaveId)
	{
		this.wave = wave;
	}

	private void DeleteFromActiveEnemiesInWave()
	{
		wave.MarkEnemyAsDefeated();
	}

	[ServerRpc(RequireOwnership = false)]
	public void DespawnEnemyServerRpc()
	{

		DeleteFromActiveEnemiesInWave();
		GetComponent<NetworkObject>().Despawn(true);


	}

	public void DoDespawn()
	{
		DespawnEnemyServerRpc();
	}

}
