using UnityEngine;
using System.Collections.Generic;

public static class InputRecorder
{
    // Structure to hold frame data
    public struct FrameData
    {
        public Vector2 movement;
        public float jump;
        public bool interact;
        public GameObject character;
        public float timeStamp;
    }

    // List to store recorded frames
    private static List<FrameData> recordedFrames = new List<FrameData>();

    // Indicates if recording is active
    private static bool isRecording = false;

    // Timer to track relative time
    private static float recordingStartTime;

    // Method to start recording
    public static void StartRecording()
    {
        isRecording = true;
        recordingStartTime = Time.fixedTime;
        recordedFrames.Clear();
    }

    // Method to stop recording
    public static void StopRecording()
    {
        isRecording = false;
    }

    // Method to record a frame
    public static void RecordFrame(Vector2 movement, float jump, bool interact, GameObject character)
    {
        if (!isRecording) return;

        FrameData frameData = new FrameData
        {
            movement = movement,
            jump = jump,
            interact = interact,
            character = character,
            timeStamp = Time.fixedTime - recordingStartTime
        };

        recordedFrames.Add(frameData);
    }

    // Additional methods like GetRecordedData, etc., can be added as needed
}
