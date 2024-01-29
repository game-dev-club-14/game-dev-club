using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : PlayerBaseState
{
    private float startJumpTime; // used internally, not known function so don't touch!
    private float _jumpStartTime;
    private float _jumpStartVelocity;
    private float _jumpHeight;
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        // debugging
        Debug.Log("WALL JUMP");
        float gameSpeed = GameManager.current.inGameTimeScale;
        float frameForwardTime = gameSpeed * Time.fixedDeltaTime;

        // calculate customized x-velocity
        playerBehaviour.momentum.x = playerBehaviour.moveSpeed * Vector2.one.normalized.x * playerBehaviour.slideSide; // jump horizontal speed is lower than before!

        _jumpStartTime = GameTimer.current.elapsedTime;
        // jump height is different than normal
        _jumpStartVelocity = Mathf.Sqrt(2 * playerBehaviour.jumpHeight * playerBehaviour.gravity) * Vector2.one.normalized.y;

        // set initial momentum and move direction
        // need to implement jump height using old-school physics controls!
        playerBehaviour.momentum.y = ((-(1 / 2 * playerBehaviour.gravity * frameForwardTime) + _jumpStartVelocity));
        playerBehaviour.moveDirection.y = playerBehaviour.momentum.y * gameSpeed;
        playerBehaviour.moveDirection.x = playerBehaviour.momentum.x * gameSpeed;

        // startJumpTime = Time.time;
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) 
    {
        float jumpInput = playerBehaviour.GetCurrentJumpInput();

        if (playerBehaviour.moveDirection.y <= 0 || jumpInput == 0)
        {
            playerBehaviour.SwitchState(playerBehaviour.FallingState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.GetCurrentMovementInput();
        float gameSpeed = GameManager.current.inGameTimeScale;

        // vertical movement
        float currentTime = GameTimer.current.elapsedTime;
        float timeJumping = (currentTime - _jumpStartTime);
        // most efficient way to calculate height difference
        float frameForwardTime = (Time.fixedDeltaTime * gameSpeed);
        playerBehaviour.momentum.y = (((-timeJumping) * (playerBehaviour.gravity))
            - (1 / 2 * playerBehaviour.gravity * frameForwardTime) + _jumpStartVelocity); // missing * Time.fixedDeltaTime multiplier, applies later
        playerBehaviour.moveDirection.y = playerBehaviour.momentum.y * gameSpeed;
        // playerBehaviour.moveDirection.y = (playerBehaviour.gravity / (gameSpeed * Mathf.Sqrt(gameSpeed)));// * Time.fixedDeltaTime;

        if (moveVec != Vector2.zero)
        {
            // playerBehaviour.moveDirection.x = ((playerBehaviour.horizontalAirTravel * (1 - playerBehaviour.airControl)) + 
            //   (Mathf.Sign(moveVec.x) * playerBehaviour.airControl)) * playerBehaviour.moveSpeed * gameSpeed;
            if (moveVec.x < 0)
            {
                playerBehaviour.momentum.x = Mathf.Max((playerBehaviour.moveSpeed * (-1.25f)),
                    (playerBehaviour.momentum.x + (Mathf.Sign(moveVec.x) * playerBehaviour.airControl *
                    playerBehaviour.moveSpeed * gameSpeed * Time.fixedDeltaTime))); // gradually adds x-momentum to player while capping at 1.25x movement speed
            }
            else if (moveVec.x > 0)
            {
                playerBehaviour.momentum.x = Mathf.Min((playerBehaviour.moveSpeed * 1.25f),
                    (playerBehaviour.momentum.x + (Mathf.Sign(moveVec.x) * playerBehaviour.airControl *
                    playerBehaviour.moveSpeed * gameSpeed * Time.fixedDeltaTime)));
            }
        }

        playerBehaviour.moveDirection.x = playerBehaviour.momentum.x * gameSpeed;
        playerBehaviour.rb.MovePosition(playerBehaviour.rb.position + playerBehaviour.moveDirection * Time.fixedDeltaTime);
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {}
}
