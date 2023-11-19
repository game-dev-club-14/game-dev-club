using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recording
{
    public ReplayObject replayObject { get; private set; }
    private Queue<ReplayData> originalQueue; // reference to entire recording
    private Queue<ReplayData> replayQueue; // used concurrently
    // [SerializeField] private Transform recordingStorage;

    public Recording(Queue<ReplayData> recordingQueue)
    {
        this.originalQueue = new Queue<ReplayData>(recordingQueue);
        this.replayQueue = new Queue<ReplayData>(recordingQueue);
    }

    public void RestartFromBeginning()
    {
        this.replayQueue = new Queue<ReplayData>(originalQueue);
    }

    public bool PlayNextFrame()
    {
        if (replayObject == null)
        {
            Debug.LogError("Tried to play next frame of recording, but replayObject was null.");
        }

        bool hasMoreFrames = false;
        if (replayQueue.Count != 0)
        {
            ReplayData data = replayQueue.Dequeue();
            replayObject.SetDataForFrame(data);
            hasMoreFrames = true;
        }
        return hasMoreFrames;
    }

    public void InstantiateReplayObject(GameObject replayObjectPrefab, GameObject parent)
    {
        if (replayQueue.Count != 0)
        {
            ReplayData startingData = replayQueue.Peek();
            replayObject = Object.Instantiate(replayObjectPrefab, startingData.position, Quaternion.identity)
                .GetComponent<ReplayObject>();
            replayObject.gameObject.transform.parent = parent.transform;
        }
    }

    public void DestroyReplayObjectIfExists()
    {
        if (replayObject != null)
        {
            Object.Destroy(replayObject.gameObject);
        }
    }
}
