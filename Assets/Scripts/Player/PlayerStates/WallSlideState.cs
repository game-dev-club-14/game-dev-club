using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        playerBehaviour.jumpsRemaining = playerBehaviour.maxJumps;
        float gameSpeed = GameManager.current.inGameTimeScale;
        playerBehaviour.momentum.y = -playerBehaviour.slideSpeed * 
            playerBehaviour.gravity * playerBehaviour.fallSpeedMultiplier * Time.fixedDeltaTime; // adjusting slide speed as a proportion of regular falling speed
        playerBehaviour.moveDirection.y = playerBehaviour.momentum.y * gameSpeed;
        Debug.Log("WALL SLIDING");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {
        if (playerBehaviour.IsNewJumpInitiated())
        {
            playerBehaviour.SwitchState(playerBehaviour.WallJumpState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.GetCurrentMovementInput();
        float gameSpeed = GameManager.current.inGameTimeScale;

        playerBehaviour.momentum.x = Mathf.Sign(moveVec.x) * playerBehaviour.moveSpeed * 0.02f; // player "sticks" to wall to make it harder to get off by accident
        playerBehaviour.moveDirection.x = playerBehaviour.momentum.x * gameSpeed;

        playerBehaviour.rb.MovePosition(playerBehaviour.rb.position + playerBehaviour.moveDirection * Time.fixedDeltaTime);
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {
        if (collider.IsTouching(playerBehaviour.footCollider)) {
            playerBehaviour.SwitchState(playerBehaviour.IdleState);
        }
    }

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {
        if (! (collider.IsTouching(playerBehaviour.leftWallCollider) || collider.IsTouching(playerBehaviour.rightWallCollider))) {
            playerBehaviour.SwitchState(playerBehaviour.FallingState);
        }
    }
}
