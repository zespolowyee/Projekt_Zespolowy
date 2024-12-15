using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Wave : NetworkBehaviour
{
    [SerializeField] private WaveEntry[] waveEntries;
	private EnemyPath defaultServerPath;


	private List<GameObject> activeEnemies = new List<GameObject>();
	public WaveEntry[] WaveEntries { get => waveEntries; set => waveEntries = value; }

	[ServerRpc]
	public void StartNextWaveServerRpc()
	{

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

				activeEnemies.Add(spawnedEnemy);
			}
		}
	}

	public void SetDeafaultServerPath(EnemyPath path)
	{
		defaultServerPath = path;
	}
}
