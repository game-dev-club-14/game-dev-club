using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGoal : MonoBehaviour
{
    private bool hasReachedGoal = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasReachedGoal && other.gameObject.CompareTag("Finish"))
        {
            hasReachedGoal = true; // Set the flag so this code won't run again
            Debug.Log("Goal reached!");
            GameEventsManager.current.GoalReached();

            // Teleport the player back to the starting point
            transform.position = new Vector3(5.72f, -3.001f, 0);

            // Optionally, start a coroutine to reset the flag after a delay
            StartCoroutine(ResetGoalFlag());
        }
    }

    private IEnumerator ResetGoalFlag()
    {
        yield return new WaitForSeconds(0.5f); // Wait for half a second
        hasReachedGoal = false; // Reset the flag
    }
}
