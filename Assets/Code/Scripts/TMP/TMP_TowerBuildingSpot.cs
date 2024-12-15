using Unity.Netcode;
using UnityEngine;

public class TMP_TowerBuildingSpot : NetworkBehaviour, IInteractable
{
	[SerializeField] private GameObject towerPrefab;
	public void Interact()
	{
		BuildTowerServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	public void BuildTowerServerRpc()
	{
		GameObject spawnedTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
		spawnedTower.GetComponent<NetworkObject>().Spawn(true);
	}
}
