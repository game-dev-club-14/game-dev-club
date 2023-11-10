using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        playerBehaviour.moveDirection.y = playerBehaviour.jumpHeight;
        // debugging
        // playerBehaviour.rend.material.color = Color.green;
        Debug.Log("JUMPING");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {
        if (playerBehaviour.moveDirection.y <= 0 || playerBehaviour.jump.ReadValue<float>() == 0) {
            playerBehaviour.SwitchState(playerBehaviour.FallingState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.move.ReadValue<Vector2>();

        playerBehaviour.moveDirection.y -= playerBehaviour.gravity;
        playerBehaviour.moveDirection.x = Math.Sign(moveVec.x) * playerBehaviour.moveSpeed;

        playerBehaviour.rb.MovePosition(playerBehaviour.rb.position + playerBehaviour.moveDirection * Time.fixedDeltaTime);
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {
        if (collider.IsTouching(playerBehaviour.roofCollider)) {
            playerBehaviour.SwitchState(playerBehaviour.FallingState);
        }
    }

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {}
}
