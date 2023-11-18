using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager current;

    private void Awake() {
        current = this;
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

}
