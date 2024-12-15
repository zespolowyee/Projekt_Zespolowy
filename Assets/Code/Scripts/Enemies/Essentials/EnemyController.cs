using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
	private Wave wave;
	private int inWaveId;

	public void SetWaveData(Wave wave, int inWaveId)
	{
		Debug.Log("chhoj");
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
		GetComponent<NetworkObject>().Despawn(true);
	}

	public void DoDespanin()
	{
		DespawnEnemyServerRpc();
	}

	public override void OnDestroy()
	{
		DeleteFromActiveEnemiesInWaveServerRpc();
		base.OnDestroy();
	}
}
