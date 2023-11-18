using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayObject : ReplayObject
{
    [SerializeField] private GameObject eyes;
    public bool facingRight = false;
    public override void SetDataForFrame(ReplayData data)
    {
        // typecast the data
        PlayerReplayData playerData = (PlayerReplayData) data;
        // position
        this.transform.position = playerData.position;
        bool currentFacingRight = this.facingRight;
        this.facingRight = playerData.facingRight;
        if (currentFacingRight != this.facingRight)
        {
            HandleFacing();
        }
    }

    private void HandleFacing()
    {
        Vector3 eyePosition = eyes.transform.localPosition;
        eyePosition.z = 0;
        eyePosition.y = 0.57f;

        if (facingRight)
        {
            eyePosition.x = 0.23f;
        }
        else
        {
            eyePosition.x = -0.23f;
        }
        eyes.transform.localPosition = eyePosition;
    }
}
