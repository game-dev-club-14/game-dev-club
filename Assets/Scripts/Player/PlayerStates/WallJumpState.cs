using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : PlayerBaseState
{
    private float startJumpTime;
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        // debugging
        Debug.Log("WALL JUMP");
        playerBehaviour.moveDirection = Vector2.one.normalized * playerBehaviour.jumpHeight;
        playerBehaviour.moveDirection.x *= playerBehaviour.slideSide;
        startJumpTime = Time.time;
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour) {}

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour) {
        if (startJumpTime + playerBehaviour.wallJumpMinTime <= Time.time) {
            playerBehaviour.SwitchState(playerBehaviour.JumpingState);
        } else {
            playerBehaviour.rb.MovePosition(playerBehaviour.rb.position + playerBehaviour.moveDirection * Time.fixedDeltaTime);
        }
    }

    public override void ExitState(PlayerBehaviour playerBehaviour) {}

    public override void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision) {}

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {}
}
