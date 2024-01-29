using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetMovementInput()
    {
        return Vector2.zero;
    }

    public float GetJumpInput()
    {
        return 0f;
    }

    public bool GetInteractInput()
    {
        return false;
    }
}
