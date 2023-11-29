using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayData : ReplayData
{
    public bool facingRight { get; private set; }
    public PlayerReplayData(Vector3 position, bool facingRight)
    {
        this.position = position;
        this.facingRight = facingRight;
    }
}
