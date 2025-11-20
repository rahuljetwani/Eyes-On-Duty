using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyReportingSystem : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject reportButtonUI;
    public GameObject anomalyReportUI;
    public GameObject cameraSelectorUI;

    [Header("References")]
    public AnomalyManager anomalyManager; 
    public GameOverManager gameOverManager; 

    [Header("Report UI")]
    public Image reportImageUI; 
    public float reportImageDuration = 0.5f;
    private Coroutine reportUICoroutine;

    [Header("Crosses UI")]
    public Image[] crossesUI;
    private int wrongAttempts = 0;
    

    [Header("Sound Effects")]
    public AudioSource audioSource; 
    public AudioClip crossSoundEffect; 

    [Header("Report Sounds")]
    public AudioClip correctReportSound;
    public AudioClip wrongReportSound;


    private AnomalyType selectedAnomalyType;


    void Start()
    {
        ResetUI();
        ResetCrosses();
    }

    public void OnReportButtonClicked()
    {
        reportButtonUI.SetActive(false);
        anomalyReportUI.SetActive(true);
    }

    public void OnAnomalyTypeSelected_Intruder() => SelectAnomaly(AnomalyType.Intruder);
    public void OnAnomalyTypeSelected_MissingObject() => SelectAnomaly(AnomalyType.MissingObject);
    public void OnAnomalyTypeSelected_MovedObject() => SelectAnomaly(AnomalyType.MovedObject);
    public void OnAnomalyTypeSelected_NewObject() => SelectAnomaly(AnomalyType.NewObject);
    public void OnAnomalyTypeSelected_Camera() => SelectAnomaly(AnomalyType.Camera);
    public void OnAnomalyTypeSelected_Door() => SelectAnomaly(AnomalyType.Door);

    private void SelectAnomaly(AnomalyType anomalyType)
    {
        selectedAnomalyType = anomalyType;
        anomalyReportUI.SetActive(false);
        cameraSelectorUI.SetActive(true);
    }
    //Camera
    public void OnCameraSelected_Camera0() => ReportAnomaly(0);
    public void OnCameraSelected_Camera1() => ReportAnomaly(1);
    public void OnCameraSelected_Camera2() => ReportAnomaly(2);
    public void OnCameraSelected_Camera3() => ReportAnomaly(3);
    public void OnCameraSelected_Camera4() => ReportAnomaly(4);

    private void ReportAnomaly(int cameraIndex)
    {
        Debug.Log($"[Report Attempted] Player selected Camera Index: {cameraIndex} | Anomaly: {selectedAnomalyType}");

        bool isCorrect = anomalyManager.ReportAnomaly(cameraIndex, selectedAnomalyType);

        if (isCorrect)
        {
            Debug.Log("[Report Success] Correct anomaly reported!");

            if (anomalyReportUI.activeSelf && correctReportSound != null)
                audioSource.PlayOneShot(correctReportSound);
        }
        else
        {
            Debug.Log("[Report Failure] Wrong anomaly reported!");
            HandleWrongReport();
        }

        if (reportUICoroutine != null)
            StopCoroutine(reportUICoroutine);
        reportUICoroutine = StartCoroutine(ShowReportImage());

        ResetUI();
    }

    private void HandleWrongReport()
    {
        if (wrongAttempts < crossesUI.Length)
        {
            crossesUI[wrongAttempts].enabled = true;
            wrongAttempts++;

            if (audioSource != null && crossSoundEffect != null)
                audioSource.PlayOneShot(crossSoundEffect);
        }

        if (anomalyReportUI.activeSelf && wrongReportSound != null)
            audioSource.PlayOneShot(wrongReportSound);

        if (wrongAttempts >= crossesUI.Length)
        {
            Debug.Log("[Game Over] Player reached max wrong attempts!");
            gameOverManager.TriggerGameOver("Too many false reports submitted");
        }
    }

    public void OnCancelReport()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        reportButtonUI.SetActive(true);
        anomalyReportUI.SetActive(false);
        cameraSelectorUI.SetActive(false);
    }

    private void ResetCrosses()
    {
        wrongAttempts = 0;
        foreach (Image cross in crossesUI)
            cross.enabled = false;
    }

    IEnumerator ShowReportImage()
    {
        reportImageUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(reportImageDuration);
        reportImageUI.gameObject.SetActive(false);
    }
}

