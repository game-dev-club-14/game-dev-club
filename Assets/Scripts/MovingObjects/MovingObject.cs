using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [Header("Speed")]
    public float speed = 3f;

    [Header("Waypoints")]
    public GameObject waypointsParent;

    private LinkedList<Transform> waypointsLinkedList;

    private LinkedListNode<Transform> targetNode;

    private SpriteRenderer sr;

    private Recorder recorder;

    private void Awake()
    {
        InitializeWaypoints();
        if (waypointsParent != null)
            targetNode = waypointsLinkedList.First;
        sr = GetComponent<SpriteRenderer>();
        recorder = GetComponent<Recorder>();
    }

    private void Start()
    {
        // subscribe to events
        GameEventsManager.current.onGoalReached += OnGoalReached;
        GameEventsManager.current.onRestartLevel += OnRestartLevel;
        GameEventsManager.current.onPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        GameEventsManager.current.onGoalReached -= OnGoalReached;
        GameEventsManager.current.onRestartLevel -= OnRestartLevel;
        GameEventsManager.current.onPlayerRespawn -= OnPlayerRespawn;
    }

    private void OnGoalReached()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
    }

    private void OnRestartLevel()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
    }

    private void OnPlayerRespawn()
    {
        recorder.Reset();
    }

    private void Update()
    {
        if (waypointsParent != null)
        {
            // calculate the move distance for this frame
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetNode.Value.position, speed * Time.deltaTime);
            float distanceFromWaypoint = (newPosition - targetNode.Value.position).magnitude;
            if (distanceFromWaypoint <= 0.1f)
            {
                StartCoroutine(IdleRoutine(2.0f));
                targetNode = FindNextNodeCircular();
            }
            // move
            this.transform.position = newPosition;
        }
    }

    private void LateUpdate()
    {
        ReplayData data = new MovingObjectReplayData(this.transform.position, this.transform.localScale);
        recorder.RecordReplayFrame(data);
    }

    private LinkedListNode<Transform> FindNextNodeCircular()
    {
        return targetNode.Next ?? targetNode.List.First;
    }

    private IEnumerator IdleRoutine(float idleDuration)
    {
        float originalSpeed = speed;
        speed = 0; // Stop the object by setting speed to 0

        yield return new WaitForSeconds(idleDuration); // Wait for the specified idle duration

        speed = originalSpeed; // Reset the speed to its original value
    }

    private void InitializeWaypoints()
    {
        if (waypointsParent != null)
        {
            Transform[] possibleWaypoints = waypointsParent.gameObject.GetComponentsInChildren<Transform>();
            // Transform[] possibleWaypoints = GetComponentsInChildren<Transform>();
            List<Transform> waypoints = new List<Transform>();

            foreach (Transform possibleWaypoint in possibleWaypoints)
            {
                // only add child objects tagged with "Waypoint"
                if (possibleWaypoint != transform && possibleWaypoint.gameObject.tag.Equals("Waypoint"))
                {
                    waypoints.Add(possibleWaypoint);
                }
            }
            waypointsLinkedList = new LinkedList<Transform>(waypoints);
        }
    }
}
