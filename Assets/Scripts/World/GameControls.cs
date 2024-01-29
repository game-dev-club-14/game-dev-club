using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameControls : MonoBehaviour
{
    private InputAction setCheckpointAction;
    private InputAction resetFromCheckpointAction;
    private InputAction hackingModeAction;

    private void Awake()
    {
        // Assuming you have an InputActionAsset called 'playerControls'
        var playerControls = new PlayerControls(); // Replace with your actual InputActionAsset

        // Initialize the actions
        setCheckpointAction = playerControls.Player.SetCheckpoint;
        resetFromCheckpointAction = playerControls.Player.ResetFromCheckpoint;
        hackingModeAction = playerControls.Player.Hack;

        // Register event handlers
        setCheckpointAction.performed += _ => GameEventsManager.current.SetCheckpoint();
        resetFromCheckpointAction.performed += _ => GameEventsManager.current.ResetFromCheckpoint();
        hackingModeAction.started += _ => GameEventsManager.current.EnterHackingMode();
        hackingModeAction.canceled += _ =>
        {
            if (GameEventsManager.current.hackingMode)
            {
                GameEventsManager.current.ExitHackingMode();
            }
        };
    }

    private void OnEnable()
    {
        // Enable the actions
        setCheckpointAction.Enable();
        resetFromCheckpointAction.Enable();
        hackingModeAction.Enable();
    }

    private void OnDisable()
    {
        // Disable the actions
        setCheckpointAction.Disable();
        resetFromCheckpointAction.Disable();
        hackingModeAction.Disable();
    }


}
