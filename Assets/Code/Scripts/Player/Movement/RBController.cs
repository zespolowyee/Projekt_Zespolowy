using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RBController : NetworkBehaviour
{
    [SerializeField] private GameObject playerBody;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform headCheck;

    [Header("Basic Parameters")]
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Ground Check Parameters")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private InputSystem_Actions inputActions;
    public Rigidbody Rb { get; private set; }
    public Vector2 HorizontalInput { get; private set; }
    public Vector3 SlopeNormal { get; private set; }
    public float GroundCheckDistance { get => groundCheckDistance; private set {} }
    public GameObject PlayerBody { get => playerBody; set => playerBody = value; }
    public LayerMask GroundLayer { get => groundLayer; set => groundLayer = value; }
    public Transform HeadCheck { get => headCheck; set => headCheck = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }

    private bool isGrounded;
    private float verticalCameraRotation = 0f;

    [Header("State Setup")]
    public WalkState walkState;
    public AirborneState airbourneState;
    public RunState runState;
    public CroutchState croutchState;

    public PlayerMoveState moveState;

    private void Awake()
    {
        Rb = playerBody.GetComponent<Rigidbody>();
		Rb.MovePosition(NetworkManager.Singleton.transform.position);
		inputActions = new InputSystem_Actions();
        inputActions.Enable();
        inputActions.Player.Jump.performed += Jump;

        moveState = walkState;
        walkState.Setup(this);
        runState.Setup(this);
        croutchState.Setup(this);
        airbourneState.Setup(this);
    }

    private void Start()
    {
        if (!IsOwner)
        {
            cameraHolder.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            enabled = false;
            return;
        }

    }

    private void FixedUpdate()
    {
        SelectState();
        moveState.Handle();
        HandleGroundCheck();
    }

    private void Update()
    {
        HandleCamera();
        HandleInput();
    }

    private void SelectState()
    {
        if (!moveState.CanExit)
        {
            return;
        }

        PlayerMoveState nextState = moveState;
        if (isGrounded)
        {
            if (inputActions.Player.Sprint.ReadValue<float>() == 1f)
            {
                nextState = runState;
            }else if (inputActions.Player.Crouch.ReadValue<float>() == 1f)
            {
                nextState = croutchState;
            }
            else
            {
                nextState = walkState;
            }
        }

        SwitchToState(nextState, false);

    }

    private void HandleInput()
    {
        HorizontalInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    //Funkcja sprawdzaj¹cy czy gracz stoi na ziemi i jesli tak to jaki jest wektor normalny powierzchni na której stoi
    private void HandleGroundCheck()
    {        
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundCheckDistance - 0.1f, groundLayer))
            {
                SlopeNormal = hit.normal;
            }
        }
        else
        {
            SlopeNormal = Vector3.up;
        }

    }


    private void HandleCamera()
    {
        Vector2 mouseDelta = inputActions.Player.Look.ReadValue<Vector2>() * mouseSensitivity;

		

		Quaternion horizontalRotation = Quaternion.Euler(0, mouseDelta.x, 0);
		Rb.MoveRotation(Rb.rotation * horizontalRotation);

		verticalCameraRotation -= mouseDelta.y;
        verticalCameraRotation = Mathf.Clamp(verticalCameraRotation, -90f, 90f);

        cameraHolder.transform.localRotation = Quaternion.Euler(verticalCameraRotation, 0, 0);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
			SwitchToState(airbourneState, false);
            Rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    public void SwitchToState(PlayerMoveState nextState, bool ignoreCanExitClause)
    {
        //Jeœli pole CanExit w stanie != true, wtedy nie zmieniamy stanu. Chyba ¿e wymusimy to argumentem ignoreCanExitClause
        if (!moveState.CanExit && !ignoreCanExitClause)
        {
            return;
        }

        if (!nextState.CanEnterToItself && moveState == nextState)
        {
            return;
        }

		//Wykonujemy metody na wyjœciu i wejœciu do nowego stanu jeœli on siê zmieni³
		PlayerMoveState previousState = moveState;
		moveState.Exit();
		moveState = nextState;
		nextState.Enter(previousState);
	}
}
