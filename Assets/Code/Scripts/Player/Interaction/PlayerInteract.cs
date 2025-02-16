using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private float lookDistance;
    [SerializeField] private float interactionDistance;
    [SerializeField] private PlayerPublicPreferences playerPublicPreferences;
    [SerializeField] private GameObject playerRef;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        inputActions.Player.Interact.performed += CastInteractionBeam;
    }

    private void OnDisable()
    {

        inputActions.Player.Interact.performed += CastInteractionBeam;
    }

    private void Update()
    {
        CastLookBeam();
    }


    private void CastInteractionBeam(InputAction.CallbackContext context)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            IInteractable targetInteractable = hit.transform.GetComponent<IInteractable>();
            if (targetInteractable != null)
            {
                targetInteractable.Interact(playerRef);
                return;
            }

            IInteractableTwoWay targetInteractableTwoWay = hit.transform.GetComponent<IInteractableTwoWay>();
            if (targetInteractableTwoWay != null)
            {
                targetInteractableTwoWay.Interact(playerPublicPreferences);
            }
        }
    }

    private void CastLookBeam()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, lookDistance))
        {
            ILookable target = hit.transform.GetComponent<ILookable>();
            if (target != null)
            {
                target.DoWhenLookedAt();
            }
        }
    }
}
