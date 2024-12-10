using Unity.VisualScripting;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
	private readonly int EnemyLayerID = 8;
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("fuck off123");

		if (!(other.gameObject.layer == EnemyLayerID))
		{
			return;
		}

		if (other.TryGetComponent<EnemyPathfinding>(out var enemyPathfinding))
		{
			enemyPathfinding.GoToNextWaypoint();
			Debug.Log("fuck off");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("123123");
	}
}
