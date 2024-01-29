using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("States")]
    public PlayerBaseState currentState;
    public RunningState RunState = new RunningState();
    public IdleState IdleState = new IdleState();
    public JumpingState JumpingState = new JumpingState();
    public FallingState FallingState = new FallingState();
    public WallSlideState WallSlideState = new WallSlideState();
    public WallJumpState WallJumpState = new WallJumpState();

    // inputs/controls
    [HideInInspector] public PlayerControls playerControls; 
    [HideInInspector] public InputAction move;
    [HideInInspector] public InputAction jump;
    [HideInInspector] public InputAction interact;

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed;
    public Vector2 momentum = Vector2.zero; // actual momentum of the character
    public Vector2 moveDirection = Vector2.zero; // movement in next frame, depending on game speed

    [Header("Jumping")]
    public int maxJumps;
    public int jumpsRemaining;
    public float gravity;
    [Range (1f, 5f)] public float fallSpeedMultiplier;
    public float jumpHeight;
    public float maxFallSpeed;
    [Range (0f, 1f)] public float airControl;
    [HideInInspector] public float horizontalAirTravel;
    public Collider2D floorCollider;
    public Collider2D roofCollider;

    [Header("Wall Movement")]
    [Range(0f, 1f)] public float slideSpeed;
    public Collider2D leftWallCollider;
    public Collider2D rightWallCollider;
    public Collider2D footCollider;
    public float wallJumpMinTime;
    public int slideSide;

    [Header("Attacking")]
    public PlayerAttack playerAttack;

    [Header("Temporary Features")]
    public bool facingRight = false;
    [SerializeField] private GameObject eyes;
    private MasterController masterController;
    private EnemyBehaviour enemyBehaviour; // Assuming EnemyBehaviour is a defined script

    // Enum to define control source
    public enum ControlSource
    {
        Player,
        Recorder,
        Enemy,
        Static
    }


    public ControlSource controlSource = ControlSource.Player;
    private float previousJumpInput = 0f;  // Track the previous jump input


    void Awake() {
        playerControls = new PlayerControls();
        // Initialization and attaching EnemyBehaviour script if available
        masterController = GetComponent<MasterController>();
        enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    void OnEnable() {
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();

        interact = playerControls.Player.Interact;
        interact.Enable();

        // attack = playerControls.Player.Attack;
        // attack.performed += playerAttack.DoAttack;
        // attack.Enable();
    }

    void OnDisable() {
        move.Disable();
        jump.Disable();
        interact.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        // start at idle state
        currentState = IdleState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        // handoff update to current state
        currentState.UpdateState(this);
        if (GetCurrentMovementInput().x != 0f)
        {
            bool currentFacingRight = facingRight;
            facingRight = GetCurrentMovementInput().x > 0;
            if (currentFacingRight != facingRight) HandleFacing(); 
        }

        // NEW CODE
        // Update method modifications

        /*
        Vector2 movementInput = Vector2.zero;
        float jumpInput = 0f;
        bool interactInput = false; // Example for interaction input

        
        switch (controlSource)
        {
            case ControlSource.Player:
                movementInput = move.ReadValue<Vector2>();
                jumpInput = jump.ReadValue<float>();
                // interactInput = interact.ReadValue<bool>(); // Assuming an interact input

                // Record the inputs
                // InputRecorder.RecordFrame(movementInput, jumpInput, interactInput, gameObject);
                break;

            case ControlSource.Recorder:
                // Retrieve input from InputRecorder and apply
                // Example: var frameData = InputRecorder.GetFrameData(timeStamp);
                // movementInput = frameData.movement;
                // jumpInput = frameData.jump;
                // interactInput = frameData.interact;
                // Apply these inputs as needed
                break;

            case ControlSource.Static:
                movementInput = Vector2.zero;
                jumpInput = 0f;
                interactInput = false; // Example for interaction input
                break;
        }
        */
        previousJumpInput = GetCurrentJumpInput();
    }

    void FixedUpdate() 
    {
        // handoff fixedupdate to current state
        currentState.FixedUpdateState(this);
    }

    // NEW CODE
    public void SwitchControlSource(ControlSource newSource)
    {
        controlSource = newSource;
        if (newSource == ControlSource.Recorder)
        {
            // Initialize or reset anything necessary for recorder control
        }
        else
        {
            // Initialize or reset anything necessary for player control
        }
    }

    // Method to get the current movement input
    public Vector2 GetCurrentMovementInput()
    {
        switch (controlSource)
        {
            case ControlSource.Player:
                return move.ReadValue<Vector2>();
            case ControlSource.Recorder:
                // Retrieve movement input from InputRecorder
                return masterController.GetMovementInput();
            case ControlSource.Enemy:
                if (enemyBehaviour != null)
                {
                    return enemyBehaviour.GetMovementInput(); // Assuming this method exists
                }
                else
                {
                    Debug.LogError("EnemyBehaviour script not found!");
                    return Vector2.zero;
                }
            case ControlSource.Static:
                return Vector2.zero;
            default:
                return Vector2.zero;
        }
    }

    // Method to get the current jump input
    public float GetCurrentJumpInput()
    {
        switch (controlSource)
        {
            case ControlSource.Player:
                return jump.ReadValue<float>();
            case ControlSource.Recorder:
                return masterController.GetJumpInput();
            case ControlSource.Enemy:
                if (enemyBehaviour != null)
                {
                    return enemyBehaviour.GetJumpInput(); // Assuming this method exists
                }
                else
                {
                    Debug.LogError("EnemyBehaviour script not found!");
                    return 0f;
                }
            case ControlSource.Static:
                return 0f;
            default: return 0f;
        }
    }

    public bool GetCurrentInteractInput()
    {
        switch (controlSource)
        {
            case ControlSource.Player:
                return interact.ReadValue<float>() > 0.5;
            case ControlSource.Recorder:
                return masterController.GetInteractInput();
            case ControlSource.Enemy:
                if (enemyBehaviour != null)
                {
                    return enemyBehaviour.GetInteractInput(); // Assuming this method exists
                }
                else
                {
                    Debug.LogError("EnemyBehaviour script not found!");
                    return false;
                }
            case ControlSource.Static:
                return false;
            default: return false;
        }
    }

    // Method to check if a new jump has been initiated
    public bool IsNewJumpInitiated()
    {
        return GetCurrentJumpInput() > 0 && previousJumpInput == 0;
    }

    // Method to record a frame (adjusted to record only for player control)
    void RecordInputFrame()
    {
        if (controlSource != ControlSource.Player) return;

        Vector2 movementInput = GetCurrentMovementInput();
        float jumpInput = GetCurrentJumpInput();
        bool interactInput = false; // Assuming an interact input

        InputRecorder.RecordFrame(movementInput, jumpInput, interactInput, gameObject);
    }

    // temporarily disabled, TODO: invert the position of all child elements
    private void HandleFacing()
    {
    /*
        Vector3 eyePosition = eyes.transform.localPosition;
        eyePosition.z = 0;
        eyePosition.y = 0.57f;

        if (facingRight)
        {
            eyePosition.x = 0.23f;
        }
        else
        {
            eyePosition.x = -0.23f;
        }
        eyes.transform.localPosition = eyePosition;
    */
    }

    // handle state switching
    public void SwitchState(PlayerBaseState state) {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);

    }

    void OnCollisionEnter2D(Collision2D collision) {
        currentState.OnCollisionEnterState(this, collision);
    }

    void OnCollisionExit2D(Collision2D collision) {
        currentState.OnCollisionExitState(this, collision);
    }

    void OnTriggerStay2D(Collider2D collider) {
        currentState.OnTriggerStayState(this, collider);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        currentState.OnTriggerEnterState(this, collider);
    }

    void OnTriggerExit2D(Collider2D collider) {
        currentState.OnTriggerExitState(this, collider);
    }
}
