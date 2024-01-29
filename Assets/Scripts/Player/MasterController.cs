using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PlayerBehaviour;

public class MasterController : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour player;
    private PlayerStateData savedState;

    private float timestamp; // rapidly mutating, kept here to keep everything working
    private bool isRecordingInput = false;
    private PlayerInputData currentInputData;
    private int _beforeFrameIndex, _afterFrameIndex, _CPbeforeFrameIndex, _CPafterFrameIndex, _startingDataIndex = 0;
    private bool isReplaying = false;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    // recorded player inputs
    private Vector2 _currentMovementInput;
    private float _currentJumpInput;
    private bool _currentInteractInput;
    public struct PlayerStateData // may need tweaking depending on nature of player
    {
        public Vector2 position;
        public bool facingRight;
        public Vector2 moveDirection;
        public float moveSpeed;
        public PlayerBaseState currentState;
        // could add animation state, if needed
    }
    private struct PlayerInputFrame
    {
        public float timeStamp;
        public Vector2 movementInput;
        public float jumpInput;
        public bool interactInput;

        public PlayerInputFrame(float timeStamp, Vector2 movementInput, float jumpInput, bool interactInput)
        {
            this.timeStamp = timeStamp;
            this.movementInput = movementInput;
            this.jumpInput = jumpInput;
            this.interactInput = interactInput;
        }
    }

    private struct PlayerInputData
    {
        public float startingTime;
        public float endingTime;
        public float checkpointCounter; // checkpoint associated with input data
        public List<PlayerInputFrame> inputFrames;

        public PlayerInputData(float startingTime)
        {
            this.startingTime = startingTime;
            this.checkpointCounter = GameEventsManager.current.checkpointCounter;
            this.endingTime = 0f;
            this.inputFrames = new List<PlayerInputFrame>();
        }

        public void AddFrame(PlayerInputFrame frame)
        {
            inputFrames.Add(frame);
        }

        public void SetEndTime(float endTime)
        {
            this.endingTime = endTime;
        }
    }

    private List<PlayerInputData> _levelInputData = new List<PlayerInputData>();
    private PlayerInputData _selectedFrames;

    private struct Checkpoint
    {
        public float timeStamp;
        public int beforeFrameIndex;
        public int afterFrameIndex;
        public PlayerStateData playerStateData;

        public Checkpoint(float timeStamp, int beforeFrameIndex, int afterFrameIndex, PlayerStateData playerStateData)
        {
            this.timeStamp = timeStamp;
            this.beforeFrameIndex = beforeFrameIndex;
            this.afterFrameIndex = afterFrameIndex;
            this.playerStateData = playerStateData;
        }
    }

    private void Awake()
    {
        player = GetComponent<PlayerBehaviour>();
        // SavePlayerState(); // Initial save at the beginning of the game
    }

    private void Start()
    {
        // Subscribing to checkpoint and restart events in GameEventsManager
        GameEventsManager.current.onResetFromCheckpoint += ResetFromLastCheckpoint;
        GameEventsManager.current.onSetCheckpoint += SetCheckpoint;
        // Other event subscriptions can be added here

        // add initial checkpoint right at the start; saves a LOT of headaches later on!
        SetCheckpoint();
    }

    private void Update()
    {
        timestamp = GameTimer.current.elapsedTime; // change me if timing method ever changes!
        ManageInputRecordingState();

        if (isRecordingInput)
        {
            RecordPlayerInput();
        }
        if (isReplaying)
        {
            BroadcastPlayerInput();
        }
    }

    // Recording input data

    private void ManageInputRecordingState()
    {
        // only allows recording if the ControlSource is set to player.
        if (player.controlSource == ControlSource.Player && !isRecordingInput)
        {
            StartRecording();
        }
        else if (player.controlSource != ControlSource.Player && isRecordingInput)
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        // reset these indices
        _beforeFrameIndex = 0;
        _afterFrameIndex = 0;

        currentInputData = new PlayerInputData(timestamp);
        isRecordingInput = true;
        Debug.Log("started recording at time " + timestamp);
    }

    private void RecordPlayerInput()
    {
        // Assuming you have methods to get current movement, jump, and interact input
        PlayerInputFrame frameData = new PlayerInputFrame(timestamp, player.GetCurrentMovementInput(),
            player.GetCurrentJumpInput(), player.GetCurrentInteractInput());
        currentInputData.AddFrame(frameData);
    }

    private void StopRecording()
    {
        if (isRecordingInput)
        {
            currentInputData.SetEndTime(timestamp);
            _levelInputData.Add(currentInputData);
            isRecordingInput = false;
            Debug.Log("stopped recording at time " + timestamp);
        }
    }

    // input-based replay

    private void StartReplay(int checkpointIndex)
    {
        if (player.controlSource == ControlSource.Recorder)
        {
            // need to consider two cases:
            // one: if we played as this guy in the last checkpoint (start at zero with latest index), or 
            // two: if there is movement leftover from a previous checkpoint.
            // due to game rules, we cannot have both cases true at once (THANK GOD!)
            _beforeFrameIndex = checkpoints[checkpointIndex].beforeFrameIndex;
            _afterFrameIndex = checkpoints[checkpointIndex].afterFrameIndex;
            // condition to avoid checking further into data:
            // checking for two conditions where it is impossible to get a recording
            Debug.Log("length of _levelInputData: " + _levelInputData.Count);
            if (_levelInputData.Count == 0) 
            {
                EndReplay();
                return;
            }
            if (timestamp > _levelInputData[_levelInputData.Count - 1].endingTime)
            {
                Debug.Log("entered test line!");
                EndReplay();
            }
            else
            {
                isReplaying = true;
                // match the checkpoint to the replay data that is closest before it
                int len = _levelInputData.Count;
                for (int i = len - 1; i >= 0; i--)
                {
                    if (_levelInputData[i].checkpointCounter <= checkpointIndex)
                    {
                        // find out if the selected frames are the closest before the checkpoint index
                        if (i == len - 1)
                        {
                            _selectedFrames = _levelInputData[i];
                            break;
                        }
                        else if (_levelInputData[i + 1].checkpointCounter > checkpointIndex)
                        {
                            _selectedFrames = _levelInputData[i];
                            break;
                        }
                    }

                }
            }
        }
    }

    public void BroadcastPlayerInput()
    {
        GetClosestInputFrames();
        PlayerInputFrame before = _selectedFrames.inputFrames[_beforeFrameIndex];
        PlayerInputFrame after = _selectedFrames.inputFrames[_afterFrameIndex];

        // lerp inputs
        float duration = after.timeStamp - before.timeStamp;
        if (duration > 0)
        {
            float lerpFactor = (timestamp - before.timeStamp) / duration;
            _currentMovementInput = Vector2.Lerp(before.movementInput, after.movementInput, lerpFactor);
            _currentJumpInput = Mathf.Lerp(before.jumpInput, after.jumpInput, lerpFactor);
            _currentInteractInput = timestamp - before.timeStamp < duration / 2 ? before.interactInput : after.interactInput;
            Debug.Log("broadcasting x movement input of " + _currentMovementInput.x + " at time " + timestamp);
        }
        else
        {
            _currentMovementInput = before.movementInput;
            _currentJumpInput = before.jumpInput;
            _currentInteractInput = before.interactInput;
        }
        if (_beforeFrameIndex == _selectedFrames.inputFrames.Count - 1)
        {
            EndReplay();
        }
    }

    public void EndReplay()
    {
        player.controlSource = ControlSource.Static;
        isReplaying = false;
        Debug.Log("Ended replay" + " of " + gameObject.name + " at timestamp " + timestamp);
    }

    private void GetClosestInputFrames ()
    {
        int len = _selectedFrames.inputFrames.Count;
        // take the two frames that immediately sandwich the timestamp
        while ((_beforeFrameIndex < len - 1) && (_selectedFrames.inputFrames[_beforeFrameIndex + 1].timeStamp <= timestamp))
        {
            _beforeFrameIndex++;
        }
        _afterFrameIndex = _beforeFrameIndex;
        if ((_beforeFrameIndex < len - 1) && (_selectedFrames.inputFrames[_beforeFrameIndex + 1].timeStamp <= timestamp))
        {
            _afterFrameIndex++;
        }
    }

    public Vector2 GetMovementInput()
    {
        return _currentMovementInput;
    }
    public float GetJumpInput()
    {
        return _currentJumpInput;
    }
    public bool GetInteractInput()
    {
        return _currentInteractInput;
    }

    public void SetCheckpoint()
    {
        Checkpoint checkpoint = new Checkpoint(timestamp, _beforeFrameIndex, _afterFrameIndex, SavePlayerState());
        checkpoints.Add(checkpoint);
    }
    public PlayerStateData SavePlayerState()
    {
        return new PlayerStateData
        {
            position = player.transform.position,
            facingRight = player.facingRight,
            moveDirection = player.moveDirection,
            moveSpeed = player.moveSpeed,
            currentState = player.currentState
        };
    }

    public void ResetFromLastCheckpoint()
    {
        int checkpointIndex = checkpoints.Count - 1;
        player.transform.position = checkpoints[checkpointIndex].playerStateData.position;
        player.facingRight = checkpoints[checkpointIndex].playerStateData.facingRight;
        player.moveDirection = checkpoints[checkpointIndex].playerStateData.moveDirection;
        player.moveSpeed = checkpoints[checkpointIndex].playerStateData.moveSpeed;
        player.SwitchState(checkpoints[checkpointIndex].playerStateData.currentState);
        // temporary; needs to be fixed probably
        player.controlSource = ControlSource.Recorder;
        StopRecording();

        StartReplay(checkpointIndex);
        // Other reset logic can be added here
    }

    // Other methods and logic can be added as needed
}