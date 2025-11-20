using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class camManager : MonoBehaviour
{
    public static camManager Instance;

    public Camera[] cameras;
    private List<int> activeCameraIndexes = new List<int>();
    private int currentIndex = 0;

    private Vector2 swipeStart;
    private bool isSwiping = false;
    private float swipeThreshold = 50f;

    public Image switchImageUI; // Assign in Inspector
    private float lastSwitchTime = -999f;
    private float switchCooldown = 1.75f;
    private float uiDisplayDuration = 0.15f;
    private Coroutine uiCoroutine;

    [Header("Screen Switch Sound")]
    public AudioClip screenSwitchSound;
    public AudioSource screenAudioSource; 


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Add all cameras to cycle at start
        for (int i = 0; i < cameras.Length; i++)
        {
            activeCameraIndexes.Add(i);
        }
        ActivateCamera(activeCameraIndexes[currentIndex]);
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleSwipeInput();
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchCamera(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SwitchCamera(1);
        }
    }

    void HandleSwipeInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    swipeStart = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        Vector2 swipeEnd = touch.position;
                        float deltaX = swipeEnd.x - swipeStart.x;

                        if (Mathf.Abs(deltaX) > swipeThreshold) 
                        {
                            if (deltaX < 0)
                                SwitchCamera(1);
                            else
                                SwitchCamera(-1);
                        }

                        isSwiping = false;
                    }
                    break;
            }
        }
    }

    void SwitchCamera(int direction)
    {
        if (Time.time - lastSwitchTime < switchCooldown) return;

        if (activeCameraIndexes.Count == 0) return;

        lastSwitchTime = Time.time;

        cameras[activeCameraIndexes[currentIndex]].gameObject.SetActive(false);

        currentIndex += direction;

        if (currentIndex >= activeCameraIndexes.Count)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = activeCameraIndexes.Count - 1;

        ActivateCamera(activeCameraIndexes[currentIndex]);

        // Show the image
        if (uiCoroutine != null) StopCoroutine(uiCoroutine);
        uiCoroutine = StartCoroutine(ShowSwitchImage());
    }

    void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
        }
    }

    // Called by AnomalyManager when a camera anomaly is triggered
    public void RemoveCameraFromCycle(int index)
    {
        if (activeCameraIndexes.Contains(index))
        {
            activeCameraIndexes.Remove(index);
            if (currentIndex >= activeCameraIndexes.Count)
                currentIndex = 0;
            if (activeCameraIndexes.Count > 0)
                ActivateCamera(activeCameraIndexes[currentIndex]);
        }
    }

    // Called by AnomalyManager when a camera anomaly is cleared
    public void AddCameraToCycle(int index)
    {
        if (!activeCameraIndexes.Contains(index))
        {
            activeCameraIndexes.Add(index);
            activeCameraIndexes.Sort();
        }
    }

    // --- New helper methods for anomaly manager ---
    public void OnAnomalySpawned(int cameraIndex, AnomalyType anomalyType)
    {
        if (anomalyType == AnomalyType.Camera)
        {
            RemoveCameraFromCycle(cameraIndex);
        }
    }

    public void OnAnomalyCleared(int cameraIndex, AnomalyType anomalyType)
    {
        if (anomalyType == AnomalyType.Camera)
        {
            AddCameraToCycle(cameraIndex);
        }
    }

    IEnumerator ShowSwitchImage()
    {
        // Start playing the sound (from the beginning)
        if (screenAudioSource != null && screenSwitchSound != null)
        {
            screenAudioSource.clip = screenSwitchSound;
            screenAudioSource.Play();
        }

        switchImageUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(uiDisplayDuration);

        switchImageUI.gameObject.SetActive(false);

        // Pause the sound if still playing
        if (screenAudioSource != null && screenAudioSource.isPlaying)
        {
            screenAudioSource.Pause();
        }
    }

}
