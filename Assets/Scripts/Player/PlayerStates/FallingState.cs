using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        playerBehaviour.moveDirection.y = 0;
        playerBehaviour.jumpsRemaining--;
        // debugging
        // playerBehaviour.rend.material.color = Color.yellow;
        Debug.Log("FALLING");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {
        if (playerBehaviour.jump.triggered && (playerBehaviour.jumpsRemaining > 0)) {
            playerBehaviour.SwitchState(playerBehaviour.JumpingState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.move.ReadValue<Vector2>();

        playerBehaviour.moveDirection.y = Math.Max(-playerBehaviour.maxFallSpeed, playerBehaviour.moveDirection.y - playerBehaviour.gravity);
        playerBehaviour.moveDirection.x = Math.Sign(moveVec.x) * playerBehaviour.moveSpeed;

        playerBehaviour.rb.MovePosition(playerBehaviour.rb.position + playerBehaviour.moveDirection * Time.fixedDeltaTime);
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {
        if (collider.IsTouching(playerBehaviour.leftWallCollider)) {
            playerBehaviour.slideSide = 1;
            playerBehaviour.SwitchState(playerBehaviour.WallSlideState);
        } else if (collider.IsTouching(playerBehaviour.rightWallCollider)) {
            playerBehaviour.slideSide = -1;
            playerBehaviour.SwitchState(playerBehaviour.WallSlideState);
        } else if (collider.IsTouching(playerBehaviour.floorCollider)) {
            playerBehaviour.SwitchState(playerBehaviour.IdleState);
        }
    }

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {}
}
