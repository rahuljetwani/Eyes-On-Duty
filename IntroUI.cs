using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startButton;                    
    public CanvasGroup startButtonCanvasGroup;    // CanvasGroup for fading the button
    public CanvasGroup messageCanvasGroup;        


    [Header("Camera Settings")]
    public Transform cameraTransform;             
    public float fadeDuration = 1f;             
    public float rotateDuration = 2f;            
    public float messageFadeDuration = 1f;       

    private void Start()
    {
        messageCanvasGroup.alpha = 0f;
        messageCanvasGroup.interactable = false;
        messageCanvasGroup.blocksRaycasts = false;

        startButton.onClick.AddListener(() => StartCoroutine(PlayIntroSequence()));
    }

    private IEnumerator PlayIntroSequence()
    {
        // Fade out 
        yield return StartCoroutine(FadeOutButton());

        // Rotate the Camera
        yield return StartCoroutine(RotateCamera());

        // Fade in 
        yield return StartCoroutine(FadeInMessage());
    }

    private IEnumerator FadeOutButton()
    {
        float elapsed = 0f;
        float startAlpha = startButtonCanvasGroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            startButtonCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        startButtonCanvasGroup.alpha = 0f;
        startButtonCanvasGroup.interactable = false;
        startButtonCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator RotateCamera()
    {
        float elapsed = 0f;
        float startXRotation = cameraTransform.eulerAngles.x;

        // Cheaking For Negetive
        if (startXRotation > 180f) startXRotation -= 360f;

        float targetXRotation = 20f;

        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float newXRotation = Mathf.Lerp(startXRotation, targetXRotation, elapsed / rotateDuration);
            cameraTransform.eulerAngles = new Vector3(newXRotation, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
            yield return null;
        }

        cameraTransform.eulerAngles = new Vector3(targetXRotation, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
    }

    private IEnumerator FadeInMessage()
    {
        float elapsed = 0f;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        // textReady
        messageCanvasGroup.interactable = true;
        messageCanvasGroup.blocksRaycasts = true;

        while (elapsed < messageFadeDuration)
        {
            elapsed += Time.deltaTime;
            messageCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / messageFadeDuration);
            yield return null;
        }

        messageCanvasGroup.alpha = 1f;
    }

    public void LoadMainGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

}

