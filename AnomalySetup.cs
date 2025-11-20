using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovedOrDoorAnomaly
{
    public GameObject targetObject;
    public Transform anomalyPosition;
}

[System.Serializable]
public class AnomalySetup
{
    public GameObject[] intruders;
    public GameObject[] newObjects;
    public MovedOrDoorAnomaly[] movedObjects;
    public MovedOrDoorAnomaly[] doorObjects;
    public GameObject[] missingObjects;

    // Flags for each anomaly type
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
}

