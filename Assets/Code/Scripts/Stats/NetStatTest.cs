using UnityEngine;

public class NetStatTest : MonoBehaviour
{

    public NetStatModifier modifier;
    private void OnTriggerEnter(Collider other)
    {
        NetStatController controller = other.GetComponentInParent<NetStatController>();
        controller.AddModifierServerRPC(modifier);
    }

}
