using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerAttack : MonoBehaviour
{
    public abstract void DoAttack(InputAction.CallbackContext obj);
}
