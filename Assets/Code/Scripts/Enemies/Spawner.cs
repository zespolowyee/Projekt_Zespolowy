using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
	//For now only for testing purposes, we would need to add real enemy spawn logic in here

	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private EnemyPath defaultPath;

	[SerializeField] private Wave[] waves;
	private int activeWaveId = -1;


	public void Start()
	{
		foreach (Wave wave in waves)
		{
			wave.SetDeafaultServerPath(defaultPath);
		}
	}

	[ServerRpc]
	public void StartNextWaveServerRpc()
	{
		activeWaveId++;
		waves[activeWaveId].StartNextWaveServerRpc();
		/*
		foreach(WaveEntry entry in waves[activeWaveId].WaveEntries)
		{
			for (int i =0; i< entry.EnemyAmount; i++)
			{
				GameObject spawnedEnemy = Instantiate(
					entry.EnemyPrefab, 
					waves[activeWaveId].gameObject.transform,
					false);

				if (entry.UseCustomPath)
				{
					spawnedEnemy.GetComponent<EnemyNavigation>().SetPath(entry.EnemyPath);
				}
				else
				{
					spawnedEnemy.GetComponent<EnemyNavigation>().SetPath(defaultPath);
				}
				spawnedEnemy.GetComponent<NetworkObject>().Spawn(true);

			}

		}
		*/


	}



	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;

		for (int i = 1; i < defaultPath.Waypoints.Count; i++)
		{
			Gizmos.DrawLine(defaultPath.Waypoints[i - 1].position, defaultPath.Waypoints[i].position);
		}

	}
}
