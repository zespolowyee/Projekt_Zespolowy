using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Wave : NetworkBehaviour
{
    [SerializeField] private WaveEntry[] waveEntries;
	[SerializeField] private float timeAfterWaveDefeated = 5f;
	private EnemyPath defaultServerPath;
	private int activeEnemies = 0;
	public WaveEntry[] WaveEntries { get => waveEntries; set => waveEntries = value; }
	public float TimeAfterWaveDefeated { get => timeAfterWaveDefeated; set => timeAfterWaveDefeated = value; }

	[ServerRpc(RequireOwnership = false)]
	public void StartNextWaveServerRpc()
	{
		int inWaveId = 0;

		foreach (WaveEntry entry in waveEntries)
		{

			for (int i = 0; i < entry.EnemyAmount; i++)
			{
				GameObject spawnedEnemy = Instantiate(
					entry.EnemyPrefab,
					gameObject.transform,
					false);

				if (entry.UseCustomPath)
				{
					spawnedEnemy.GetComponent<EnemyNavigation>().SetPath(entry.EnemyPath);
				}
				else
				{
					spawnedEnemy.GetComponent<EnemyNavigation>().SetPath(defaultServerPath);
				}
				spawnedEnemy.GetComponent<NetworkObject>().Spawn(true);
				spawnedEnemy.GetComponent<EnemyController>().SetWaveData(this, inWaveId);
				inWaveId++;
				activeEnemies++;
			}
		}
	}

	public bool IsWaveDefeated()
	{
		Debug.Log(activeEnemies);
		if (activeEnemies <1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	[ServerRpc]
	public void MarkEnemyAsDefeatedServerRpc()
	{
		activeEnemies--;
	}



	public void SetDeafaultServerPath(EnemyPath path)
	{
		defaultServerPath = path;
	}
}
