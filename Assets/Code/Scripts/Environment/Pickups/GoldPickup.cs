using Unity.Netcode;
using UnityEngine;

public class GoldPickup : NetworkBehaviour
{
    [SerializeField] private int amount = 1;
    [SerializeField] private LayerMask playerLayer;

    public void SetAmount(int amount)
    {
        this.amount = amount;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) == 0)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<PlayerStatsDemo>(out var playerStats))
        {
            playerStats.AddGold(amount);
            Destroy(gameObject);

        }
    }
}
