using UnityEngine;
using UnityEngine.InputSystem;

public class OrbAttack : PlayerAttack
{
    public override void DoAttack(InputAction.CallbackContext obj) {
        Debug.Log("ATTTAAAAACK");
    }
}
