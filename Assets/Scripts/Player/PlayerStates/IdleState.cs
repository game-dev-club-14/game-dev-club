using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        playerBehaviour.moveDirection = Vector2.zero;
        playerBehaviour.momentum = Vector2.zero;
        playerBehaviour.jumpsRemaining = playerBehaviour.maxJumps;
        // debugging
        // playerBehaviour.rend.material.color = Color.white;
        Debug.Log("IDLE");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 movementInput = playerBehaviour.GetCurrentMovementInput();
        // switch to attack
        // Switch to run
        if (movementInput != Vector2.zero)
        {
            playerBehaviour.SwitchState(playerBehaviour.RunState);
        }
        // Switch to jump
        if (playerBehaviour.IsNewJumpInitiated())
        {
            playerBehaviour.SwitchState(playerBehaviour.JumpingState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {
        if (!collider.IsTouching(playerBehaviour.floorCollider)) {
            playerBehaviour.SwitchState(playerBehaviour.FallingState);
        }
    }
}
