using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
	//For now only for testing purposes, we would need to add real enemy spawn logic in here

	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private EnemyPath defaultPath;


	[ServerRpc(RequireOwnership = false)]
	public void SpawnEnemyServerRpc()
	{
		GameObject spawnedEnemy = Instantiate(enemyPrefab);
		spawnedEnemy.GetComponent<EnemyNavigation>().SetPath(defaultPath);
		spawnedEnemy.GetComponent<NetworkObject>().Spawn(true);
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
