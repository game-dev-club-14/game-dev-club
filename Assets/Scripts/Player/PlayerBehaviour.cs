using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("States")]
    private PlayerBaseState currentState;
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
    [HideInInspector] public InputAction attack;

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed;
    public Vector2 moveDirection = Vector2.zero;

    [Header("Jumping")]
    public int maxJumps;
    public int jumpsRemaining;
    public float gravity;
    public float jumpHeight;
    public float maxFallSpeed;
    public Collider2D floorCollider;
    public Collider2D roofCollider;

    [Header("Wall Movement")]
    public float slideSpeed;
    public Collider2D leftWallCollider;
    public Collider2D rightWallCollider;
    public Collider2D footCollider;
    public float wallJumpMinTime;
    public int slideSide;

    [Header("Attacking")]
    public PlayerAttack playerAttack;


    void Awake() {
        playerControls = new PlayerControls();
    }

    void OnEnable() {
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();

        attack = playerControls.Player.Attack;
        attack.performed += playerAttack.DoAttack;
        attack.Enable();
    }

    void OnDisable() {
        move.Disable();
        jump.Disable();
        attack.Disable();
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
    }

        void FixedUpdate() 
    {
        // handoff fixedupdate to current state
        currentState.FixedUpdateState(this);
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
