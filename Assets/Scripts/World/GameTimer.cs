using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameTimer : MonoBehaviour
{
    public static GameTimer current;

    public TextMeshProUGUI timerText;
    public float gameSpeed = 1.0f;
    public float elapsedTime = 0f;

    private void Awake()
    {
        current = this;
    }
    void LateUpdate()
    {
        // Update elapsed time based on game speed
        elapsedTime += Time.deltaTime * GameManager.current.inGameTimeScale * gameSpeed;

        // Convert elapsed time to minutes and seconds format
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int centiseconds = (int)((elapsedTime * 100) % 100);

        // Update the UI Text
        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, centiseconds);
    }

    // Method to change the game speed
    public void SetGameSpeed(float newSpeed) // deprecated; might remove later
    {
        gameSpeed = newSpeed;
    }
}
