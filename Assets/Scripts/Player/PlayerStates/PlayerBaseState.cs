using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerBehaviour playerBehaviour);

    public abstract void UpdateState(PlayerBehaviour playerBehaviour);

    public abstract void FixedUpdateState(PlayerBehaviour playerBehaviour);

    public abstract void ExitState(PlayerBehaviour playerBehaviour);

    public abstract void OnCollisionEnterState(PlayerBehaviour playerBehaviour, Collision2D collision);

    public abstract void OnCollisionExitState(PlayerBehaviour playerBehaviour, Collision2D collision);

    public abstract void OnTriggerStayState(PlayerBehaviour playerBehaviour, Collider2D collider);

    public abstract void OnTriggerEnterState(PlayerBehaviour playerBehaviour, Collider2D collider);

    public abstract void OnTriggerExitState(PlayerBehaviour playerBehaviour, Collider2D collider);
}
