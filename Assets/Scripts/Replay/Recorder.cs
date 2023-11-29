using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    [Header("Prefab to Instantiate")]
    [SerializeField] private GameObject replayObjectPrefab;
    public Queue<ReplayData> recordingQueue {  get; private set; }
    private bool isDoingReplay = false;
    private Recording recording;

    private void Awake()
    {
        recordingQueue = new Queue<ReplayData>();
    }

    private void Start()
    {
        // subscribe to events; these will automatically be called upon game manager events.
        GameEventsManager.current.onGoalReached += OnGoalReached;
        GameEventsManager.current.onRestartLevel += OnRestartLevel;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        GameEventsManager.current.onGoalReached -= OnGoalReached;
        GameEventsManager.current.onRestartLevel -= OnRestartLevel;
    }

    private void OnGoalReached()
    {
        StartReplay();
    }

    private void OnRestartLevel()
    {
        Reset();
    }

    public void RecordReplayFrame(ReplayData data)
    {
        recordingQueue.Enqueue(data);
    }

    private void Update() 
    {
        if (!isDoingReplay)
        {
            return;
        }

        bool hasMoreFrames = recording.PlayNextFrame();
        // Debug.Log("Replay in progress; object position: " + recording.replayObject.transform.position);

        // check if we're finished, so we can restart
        if (!hasMoreFrames )
        {
            RestartReplay();
        }
    }

    private void StartReplay()
    {
        isDoingReplay = true;
        // initialize the recording
        recording = new Recording(recordingQueue);
        // reset the current recording queue for next time
        recordingQueue.Clear();
        // insitantiate replay object in the scene
        recording.InstantiateReplayObject(replayObjectPrefab);
        // TODO - change the camera object to the replay object
        // Debug.Log("Starting replay...");
    }

    private void RestartReplay()
    {
        isDoingReplay = true;
        // TODO - restart our queued data from the beginning
        recording.RestartFromBeginning();
    }

    public void Reset()
    {
        isDoingReplay = false;
        // TODO: reset the recorder to a clean slate
        recordingQueue?.Clear();
        recording.DestroyReplayObjectIfExists();
        recording = null;
    }
}
