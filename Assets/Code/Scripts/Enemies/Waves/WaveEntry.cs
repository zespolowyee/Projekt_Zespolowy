using Unity.Netcode;
using UnityEngine;

public class WaveEntry: NetworkBehaviour
{
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private int enemyAmount;
	[SerializeField] private EnemyPath enemyPath;
	[SerializeField] private bool useCustomPath = false;

	public EnemyPath EnemyPath { get => enemyPath; set => enemyPath = value; }
	public int EnemyAmount { get => enemyAmount; set => enemyAmount = value; }
	public GameObject EnemyPrefab { get => enemyPrefab; set => enemyPrefab = value; }
	public bool UseCustomPath { get => useCustomPath; set => useCustomPath = value; }

	private void Start()
	{
		if (enemyPath.Waypoints.Count < 1)
		{
			useCustomPath = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;

		if(enemyPath.Waypoints.Count < 1)
		{
			return;
		}
		for (int i = 1; i < enemyPath.Waypoints.Count; i++)
		{
			Gizmos.DrawLine(enemyPath.Waypoints[i - 1].position, enemyPath.Waypoints[i].position);
		}

	}
}
