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

	private bool isWaiting = false;

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

	public void Update()
	{
		if (activeWaveId == waves.Length || activeWaveId < 0)
		{
			return;
		}

		if (waves[activeWaveId].IsWaveDefeated() && !isWaiting)
		{
			StartCoroutine(WaitBetweenWaves(waves[activeWaveId].TimeAfterWaveDefeated));
		}
	}

	public IEnumerator WaitBetweenWaves(float waitTime)
	{
		Debug.Log("waiting for next wave");
		isWaiting = true;
		yield return new WaitForSeconds(waitTime);
		Debug.Log("started next wave");
		StartNextWaveServerRpc();
		isWaiting = false;
	}
}
