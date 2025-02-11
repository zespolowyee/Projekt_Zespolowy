using UnityEngine;

public class NetStatTest : MonoBehaviour, IInteractable
{

    public NetStatModifier modifier;

    public void Interact(GameObject interactingPlayer)
    {
        INetStatController statController = interactingPlayer.GetComponent<INetStatController>();
        statController.AddModifierServerRPC(modifier);
    }


}
