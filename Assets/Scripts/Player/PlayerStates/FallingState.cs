using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : PlayerBaseState
{
    public override void EnterState(PlayerBehaviour playerBehaviour) {
        // playerBehaviour.moveDirection.y = 0;
        // instead, apply falling speed multiplier * gravity to the object!

        playerBehaviour.jumpsRemaining--;
        // debugging
        // playerBehaviour.rend.material.color = Color.yellow;
        Debug.Log("FALLING");
    }

    public override void UpdateState(PlayerBehaviour playerBehaviour)
    {
        if (playerBehaviour.IsNewJumpInitiated() && playerBehaviour.jumpsRemaining > 0)
        {
            playerBehaviour.SwitchState(playerBehaviour.JumpingState);
        }
    }

    public override void FixedUpdateState(PlayerBehaviour playerBehaviour)
    {
        Vector2 moveVec = playerBehaviour.GetCurrentMovementInput();
        float gameSpeed = GameManager.current.inGameTimeScale;

        playerBehaviour.momentum.y = Math.Max(-playerBehaviour.maxFallSpeed, 
            playerBehaviour.momentum.y - (playerBehaviour.gravity * Time.fixedDeltaTime 
            * playerBehaviour.fallSpeedMultiplier * gameSpeed));
        playerBehaviour.moveDirection.y = playerBehaviour.momentum.y * gameSpeed;
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

    public override void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider) {
        if (collider.IsTouching(playerBehaviour.leftWallCollider)) {
            playerBehaviour.slideSide = 1;
            playerBehaviour.SwitchState(playerBehaviour.WallSlideState);
        } else if (collider.IsTouching(playerBehaviour.rightWallCollider)) {
            playerBehaviour.slideSide = -1;
            playerBehaviour.SwitchState(playerBehaviour.WallSlideState);
        } else if (collider.IsTouching(playerBehaviour.floorCollider)) {
            if (playerBehaviour.momentum.x != 0)
            {
                playerBehaviour.SwitchState(playerBehaviour.RunState);
            }
            else
            {
                playerBehaviour.SwitchState(playerBehaviour.IdleState);
            }
        }
    }

    public override void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider) {}

    public override void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider) {}
}
