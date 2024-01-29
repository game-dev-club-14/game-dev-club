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
        Vector2 movementInput = playerBehaviour.GetCurrentMovementInput();

        if (playerBehaviour.IsNewJumpInitiated())
        {
            playerBehaviour.SwitchState(playerBehaviour.JumpingState);
        }
        if (movementInput == Vector2.zero)
        {
            // playerBehaviour.momentum.x = 0; (make adjustments in case of wanting to gradually decrease speed)
            playerBehaviour.SwitchState(playerBehaviour.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        Vector2 moveVec = playerBehaviour.GetCurrentMovementInput();
        if (moveVec != Vector2.zero)
        {
            float gameSpeed = GameManager.current.inGameTimeScale;
            playerBehaviour.momentum.x = Mathf.Sign(moveVec.x) * playerBehaviour.moveSpeed;
            playerBehaviour.moveDirection.x = playerBehaviour.momentum.x * gameSpeed;
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
