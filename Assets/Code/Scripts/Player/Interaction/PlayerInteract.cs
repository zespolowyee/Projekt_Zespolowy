using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
	[SerializeField] private float lookDistance;
	[SerializeField] private float interactionDistance;
	private InputSystem_Actions inputActions;

	private void Awake()
	{
		inputActions = new InputSystem_Actions();
		inputActions.Enable();
	}

	private void OnEnable()
	{
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
			IInteractable target = hit.transform.GetComponent<IInteractable>();
			if (target != null)
			{
				target.Interact();
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
