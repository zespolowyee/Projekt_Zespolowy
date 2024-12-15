using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
	private Wave wave;
	private int inWaveId;

	public void SetWaveData(Wave wave, int inWaveId)
	{

		this.inWaveId = inWaveId;
		this.wave = wave;
	}

	[ServerRpc (RequireOwnership = false)]
	private void DeleteFromActiveEnemiesInWaveServerRpc()
	{
		wave.MarkEnemyAsDefeatedServerRpc();
	}

	[ServerRpc (RequireOwnership = false)]
	public void DespawnEnemyServerRpc()
	{
		DeleteFromActiveEnemiesInWaveServerRpc();
		GetComponent<NetworkObject>().Despawn(true);
	}

	public void DoDespawn()
	{
		DespawnEnemyServerRpc();
	}

}
