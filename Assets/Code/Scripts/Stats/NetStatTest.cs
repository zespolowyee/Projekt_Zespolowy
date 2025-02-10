using UnityEngine;

public class NetStatTest : MonoBehaviour, IInteractable
{

    public NetStatModifier modifier;

    public void Interact(GameObject interactingPlayer)
    {
        NetStatController statController = interactingPlayer.GetComponent<NetStatController>();
        statController.AddModifierServerRPC(modifier);
    }


}
