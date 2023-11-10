using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        playerBehaviour.jumpsRemaining = playerBehaviour.maxJumps;
        playerBehaviour.moveDirection.y = -playerBehaviour.slideSpeed;
        Debug.Log("WALL SLIDING");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {
        if (playerBehaviour.jump.triggered) {
            playerBehaviour.SwitchState(playerBehaviour.WallJumpState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.move.ReadValue<Vector2>();

        playerBehaviour.moveDirection.x = Math.Sign(moveVec.x) * playerBehaviour.moveSpeed;

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
