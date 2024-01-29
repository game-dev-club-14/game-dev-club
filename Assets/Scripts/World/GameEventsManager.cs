using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager current;

    public List<float> checkpointTimes;
    public int checkpointCounter = 1; // there is always a checkpoint time at the very beginning
    public bool hackingMode = false;

    private void Awake() {
        current = this;
        checkpointTimes.Add(0f);
    }

    public event Action onGoalReached;

    public void GoalReached()
    {
        onGoalReached?.Invoke();
    }

    public event Action onRestartLevel;

    public void RestartLevel()
    {
        onRestartLevel?.Invoke();
        checkpointCounter = 1;
    }

    public event Action<GameObject> onChangeCameraTarget;

    public void ChangeCameraTarget(GameObject newTarget)
    {
        onChangeCameraTarget?.Invoke(newTarget);
    }

    public event Action onPlayerRespawn;

    public void PlayerRespawn()
    {
        onPlayerRespawn?.Invoke();
    }

    public event Action onSetCheckpoint;

    public void SetCheckpoint()
    {
        checkpointCounter++;
        checkpointTimes.Add(GameTimer.current.elapsedTime);
        onSetCheckpoint?.Invoke();
    }

    public event Action onResetFromCheckpoint;

    public void ResetFromCheckpoint()
    {
        onResetFromCheckpoint?.Invoke();
        GameTimer.current.elapsedTime = checkpointTimes[checkpointTimes.Count - 1]; // last element of checkpointTimes
        Debug.Log("resetting from checkpoint!");
    }

    public event Action onEnterHackingMode;

    public void EnterHackingMode()
    {
        if (!hackingMode)
        {
            onEnterHackingMode?.Invoke();
            hackingMode = true;
        }
    }

    public event Action onExitHackingMode;

    public void ExitHackingMode()
    {
        if (hackingMode)
        {
            onExitHackingMode?.Invoke();
            hackingMode = false; // why does environment suggest '&=' operator?
        }
    }
}
