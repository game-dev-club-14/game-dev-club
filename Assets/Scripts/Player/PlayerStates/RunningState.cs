using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        // debugging
        // playerMovement.rend.material.color = Color.blue;
        Debug.Log("Running");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {
        if (playerBehaviour.jump.triggered) {
            playerBehaviour.SwitchState(playerBehaviour.JumpingState);
        }
        if (playerBehaviour.move.ReadValue<Vector2>() == Vector2.zero) {
            playerBehaviour.SwitchState(playerBehaviour.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.move.ReadValue<Vector2>();
        if (moveVec != Vector2.zero) {
            playerBehaviour.moveDirection.x = Mathf.Sign(moveVec.x) * playerBehaviour.moveSpeed;

            playerBehaviour.rb.MovePosition(playerBehaviour.rb.position + playerBehaviour.moveDirection * Time.fixedDeltaTime);
        }
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {
    }

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {
    if (!collider.IsTouching(playerBehaviour.floorCollider)) {
            playerBehaviour.SwitchState(playerBehaviour.FallingState);
        }
    }
}
