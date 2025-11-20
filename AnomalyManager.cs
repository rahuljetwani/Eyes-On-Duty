using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnomalyType
{
    Camera,
    Intruder,
    NewObject,
    MovedObject,
    Door,
    MissingObject
}

public class AnomalyManager : MonoBehaviour
{
    [System.Serializable]
    public class MovedOrDoorAnomaly
    {
        public GameObject originalObject;
        public GameObject anomalyClone;
    }

    [System.Serializable]
    public class AnomalySetup
    {
        public string cameraName;

        public GameObject[] intruders;
        public GameObject[] newObjects;
        public MovedOrDoorAnomaly[] movedObjects;
        public MovedOrDoorAnomaly[] doorObjects;
        public GameObject[] missingObjects;

        public bool hasCameraAnomaly = false;
        public bool hasIntruderAnomaly = false;
        public bool hasNewObjectAnomaly = false;
        public bool hasMovedObjectAnomaly = false;
        public bool hasDoorAnomaly = false;
        public bool hasMissingObjectAnomaly = false;

        public bool HasAnyAnomaly()
        {
            return hasCameraAnomaly || hasIntruderAnomaly || hasNewObjectAnomaly ||
                   hasMovedObjectAnomaly || hasDoorAnomaly || hasMissingObjectAnomaly;
        }

        public void ClearAllAnomalies()
        {
            hasCameraAnomaly = false;
            hasIntruderAnomaly = false;
            hasNewObjectAnomaly = false;
            hasMovedObjectAnomaly = false;
            hasDoorAnomaly = false;
            hasMissingObjectAnomaly = false;
        }
    }

    public AnomalySetup[] cameraAnomalies;
    public Camera[] cameraFeeds;

    private float nextAnomalyTime;

    void Start()
    {
        nextAnomalyTime = Time.time + GetRandomAnomalyInterval();

        foreach (var setup in cameraAnomalies)
        {
            ToggleObjects(setup.intruders, false);
            ToggleObjects(setup.newObjects, false);

            foreach (var item in setup.movedObjects)
            {
                if (item.anomalyClone != null) item.anomalyClone.SetActive(false);
                if (item.originalObject != null) item.originalObject.SetActive(true);
            }

            foreach (var item in setup.doorObjects)
            {
                if (item.anomalyClone != null) item.anomalyClone.SetActive(false);
                if (item.originalObject != null) item.originalObject.SetActive(true);
            }
        }
    }

    float GetRandomAnomalyInterval()
    {
        return Random.Range(20f, 30f);
    }

    void Update()
    {
        if (Time.time >= nextAnomalyTime)
        {
            TriggerRandomAnomaly();
            nextAnomalyTime = Time.time + GetRandomAnomalyInterval();
        }
    }

    void TriggerRandomAnomaly()
    {
        List<int> availableCameraIndexes = new List<int>();
        for (int i = 0; i < cameraAnomalies.Length; i++)
        {
            if (!cameraAnomalies[i].HasAnyAnomaly())
                availableCameraIndexes.Add(i);
        }

        if (availableCameraIndexes.Count == 0)
        {
            Debug.Log("[AnomalyManager] No cameras available for anomaly. Skipping spawn.");

            // Trigger game over due to too many anomalies
            GameOverManager gameOverManager = FindFirstObjectByType<GameOverManager>();
            if (gameOverManager != null)
            {
                gameOverManager.TriggerGameOver("Too many anomalies happened");
            }
            return;
        }

        int selectedIndex = availableCameraIndexes[Random.Range(0, availableCameraIndexes.Count)];
        AnomalySetup setup = cameraAnomalies[selectedIndex];
        AnomalyType selectedType = (AnomalyType)Random.Range(0, 6);

        switch (selectedType)
        {
            case AnomalyType.Camera:
                cameraFeeds[selectedIndex].gameObject.SetActive(false);
                setup.hasCameraAnomaly = true;
                camManager.Instance.RemoveCameraFromCycle(selectedIndex);
                break;

            case AnomalyType.Intruder:
                ToggleObjects(setup.intruders, true);
                setup.hasIntruderAnomaly = true;
                break;

            case AnomalyType.NewObject:
                ToggleObjects(setup.newObjects, true);
                setup.hasNewObjectAnomaly = true;
                break;

            case AnomalyType.MovedObject:
                foreach (var item in setup.movedObjects)
                {
                    if (item.originalObject != null) item.originalObject.SetActive(false);
                    if (item.anomalyClone != null) item.anomalyClone.SetActive(true);
                }
                setup.hasMovedObjectAnomaly = true;
                break;

            case AnomalyType.Door:
                foreach (var item in setup.doorObjects)
                {
                    if (item.originalObject != null) item.originalObject.SetActive(false);
                    if (item.anomalyClone != null) item.anomalyClone.SetActive(true);
                }
                setup.hasDoorAnomaly = true;
                break;

            case AnomalyType.MissingObject:
                ToggleObjects(setup.missingObjects, false);
                setup.hasMissingObjectAnomaly = true;
                break;
        }

        Debug.Log($"[Anomaly Spawned] Camera: {setup.cameraName} | Type: {selectedType}");
    }


    void ToggleObjects(GameObject[] objects, bool active)
    {
        foreach (var obj in objects)
        {
            if (obj != null) obj.SetActive(active);
        }
    }

    public bool ReportAnomaly(int cameraIndex, AnomalyType type)
    {
        if (cameraIndex < 0 || cameraIndex >= cameraAnomalies.Length)
            return false; // Invalid camera index, treat as false

        AnomalySetup setup = cameraAnomalies[cameraIndex];
        bool anomalyFixed = false; // Track if we fixed an anomaly

        switch (type)
        {
            case AnomalyType.Camera:
                if (setup.hasCameraAnomaly)
                {
                    cameraFeeds[cameraIndex].gameObject.SetActive(true);
                    EnsureSingleAudioListener(cameraFeeds[cameraIndex]);
                    setup.hasCameraAnomaly = false;
                    camManager.Instance.AddCameraToCycle(cameraIndex);
                    anomalyFixed = true;
                }
                break;

            case AnomalyType.Intruder:
                if (setup.hasIntruderAnomaly)
                {
                    ToggleObjects(setup.intruders, false);
                    setup.hasIntruderAnomaly = false;
                    anomalyFixed = true;
                }
                break;

            case AnomalyType.NewObject:
                if (setup.hasNewObjectAnomaly)
                {
                    ToggleObjects(setup.newObjects, false);
                    setup.hasNewObjectAnomaly = false;
                    anomalyFixed = true;
                }
                break;

            case AnomalyType.MovedObject:
                if (setup.hasMovedObjectAnomaly)
                {
                    foreach (var item in setup.movedObjects)
                    {
                        if (item.anomalyClone != null) item.anomalyClone.SetActive(false);
                        if (item.originalObject != null) item.originalObject.SetActive(true);
                    }
                    setup.hasMovedObjectAnomaly = false;
                    anomalyFixed = true;
                }
                break;

            case AnomalyType.Door:
                if (setup.hasDoorAnomaly)
                {
                    foreach (var item in setup.doorObjects)
                    {
                        if (item.anomalyClone != null) item.anomalyClone.SetActive(false);
                        if (item.originalObject != null) item.originalObject.SetActive(true);
                    }
                    setup.hasDoorAnomaly = false;
                    anomalyFixed = true;
                }
                break;

            case AnomalyType.MissingObject:
                if (setup.hasMissingObjectAnomaly)
                {
                    ToggleObjects(setup.missingObjects, true);
                    setup.hasMissingObjectAnomaly = false;
                    anomalyFixed = true;
                }
                break;
        }

        if (anomalyFixed)
        {
            Debug.Log($"[Anomaly Cleared] Camera: {setup.cameraName} | Type: {type}");
        }
        else
        {
            Debug.Log($"[No Anomaly Found] Camera: {setup.cameraName} | Type: {type}");
        }

        return anomalyFixed;
    }

    void EnsureSingleAudioListener(Camera cam)
    {
        AudioListener[] audioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        // Remove all other AudioListeners
        foreach (AudioListener listener in audioListeners)
        {
            if (listener.gameObject != cam.gameObject)
            {
                Destroy(listener);
            }
        }

        // Add AudioListener to the current camera if it doesn't have one
        if (cam.GetComponent<AudioListener>() == null)
        {
            cam.gameObject.AddComponent<AudioListener>();
        }
    }

}
