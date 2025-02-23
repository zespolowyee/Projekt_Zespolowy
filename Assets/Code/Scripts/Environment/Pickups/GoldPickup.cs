using Unity.Netcode;
using UnityEngine;

public class GoldPickup : NetworkBehaviour
{
    [SerializeField] private int amount = 1;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float animationForce;


    public void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = Random.insideUnitCircle.normalized;
        direction += new Vector3(0, 1, 0) * animationForce;
        rb.AddForce(direction, ForceMode.Impulse);
    }
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
