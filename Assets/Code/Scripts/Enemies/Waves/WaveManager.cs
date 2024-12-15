
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<Spawner> Spawners;
	private bool isWaiting = false;

	[SerializeField] private float TimeBetweenChecks = 1f;
	private float timeSinceLastCheck = 0;

	[SerializeField] private float timeBetweenWaves;

	public override void OnNetworkSpawn()
	{
		if (!IsServer)
		{
			gameObject.SetActive(false);
		}
		base.OnNetworkSpawn();
	}

	public void StartNextWaveOnAllSpawners()
	{
		foreach (Spawner spawner in Spawners)
		{
			spawner.StartNextWaveServerRpc();
		}
	}

	public bool AreAllSpawnersAreReady()
	{
		foreach (Spawner spawner in Spawners)
		{
			if (!spawner.IsActiveWaveDeafeated())
			{
				return false;
			}
		}
		return true;
	}

	private void Update()
	{
		timeSinceLastCheck += Time.deltaTime;
		if (timeSinceLastCheck > TimeBetweenChecks)
		{
			if (AreAllSpawnersAreReady() && !isWaiting){
				Debug.Log("Next wave starts in " + timeBetweenWaves + "s");
				StartCoroutine(WaitBetweenWaves());
			}
			timeSinceLastCheck = 0;
		}
	}

	public IEnumerator WaitBetweenWaves()
	{

		isWaiting = true;
		yield return new WaitForSeconds(timeBetweenWaves);
		Debug.Log("started next wave!");
		StartNextWaveOnAllSpawners();
		isWaiting = false;
	}
}
