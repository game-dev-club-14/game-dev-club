using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    // Time scale (customized)
    public float inGameTimeScale = 1.0f;

    // camera targetting
    public CinemachineVirtualCamera cinemachineCamera;
    private CinemachineFramingTransposer _framingTransposer;

    // Start is called before the first frame update
    private void Awake()
    {
        current = this;
        _framingTransposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    void Start()
    {
        // subscribe to necessary gameeventsmanager events
        GameEventsManager.current.onEnterHackingMode += onEnterHackingMode;
        GameEventsManager.current.onExitHackingMode += onExitHackingMode;
        GameEventsManager.current.onChangeCameraTarget += onChangeCameraTarget;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onEnterHackingMode()
    {
        inGameTimeScale = 0.2f;

        // move main camera
        _framingTransposer.m_CameraDistance = 11f;
    }

    private void onExitHackingMode()
    {
        inGameTimeScale = 1f;

        // move main camera
        _framingTransposer.m_CameraDistance = 10f;
    }

    private void onChangeCameraTarget(GameObject newTarget)
    {

    }
}
