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

    private bool isGrounded;
    private float verticalCameraRotation = 0f;

    [Header("State Setup")]
    public WalkState walkState;
    public RunState runState;
    public CroutchState croutchState;

    PlayerMoveState moveState;

    private void Awake()
    {
        Rb = playerBody.GetComponent<Rigidbody>();
        
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
        inputActions.Player.Jump.performed += Jump;

        moveState = walkState;
        walkState.Setup(this);
        runState.Setup(this);
        croutchState.Setup(this);
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
        moveState.Handle();
        HandleGroundCheck();
    }

    private void Update()
    {
        HandleCamera();
        HandleInput();
        SelectState();
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


        //Wykonujemy metody na wyj�ciu i wej�ciu do nowego stanu je�li on si� zmieni�
        if (nextState!= moveState)
        {
            moveState.Exit();
            moveState = nextState;
            moveState.Enter();
        }

    }

    private void HandleInput()
    {
        HorizontalInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    //Funkcja sprawdzaj�cy czy gracz stoi na ziemi i jesli tak to jaki jest wektor normalny powierzchni na kt�rej stoi
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

        playerBody.transform.Rotate(Vector3.up * mouseDelta.x);

        verticalCameraRotation -= mouseDelta.y;
        verticalCameraRotation = Mathf.Clamp(verticalCameraRotation, -90f, 90f);

        cameraHolder.transform.localRotation = Quaternion.Euler(verticalCameraRotation, 0, 0);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            Rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
}
