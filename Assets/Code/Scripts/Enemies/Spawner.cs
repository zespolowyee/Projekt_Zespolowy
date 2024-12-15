using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		if (IsServer)
		{
			gameObject.SetActive(false);
		}

		foreach (Wave wave in waves)
		{
			wave.SetDeafaultServerPath(defaultPath);
		}
	}

	[ServerRpc]
	public void StartNextWaveServerRpc()
	{
		activeWaveId++;
		if (activeWaveId == waves.Length)
		{
			return;
		}
		waves[activeWaveId].StartNextWaveServerRpc();
		
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		for (int i = 1; i < defaultPath.Waypoints.Count; i++)
		{
			Gizmos.DrawLine(defaultPath.Waypoints[i - 1].position, defaultPath.Waypoints[i].position);
		}

	}

	public bool IsActiveWaveDeafeated()
	{
		if(activeWaveId == -1 || activeWaveId > waves.Length-1)
		{
			return false;
		}
		return waves[activeWaveId].IsWaveDefeated();
	}

}
