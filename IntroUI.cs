using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startButton;                    // Start button
    public CanvasGroup startButtonCanvasGroup;    // CanvasGroup for fading the button
    public CanvasGroup messageCanvasGroup;        // CanvasGroup for fading in the message text

    [Header("Camera Settings")]
    public Transform cameraTransform;             // Reference to the camera
    public float fadeDuration = 1f;               // Duration of fade-out
    public float rotateDuration = 2f;             // Duration of camera rotation
    public float messageFadeDuration = 1f;        // Duration of the message fade-in

    private void Start()
    {
        // Hide the message text initially
        messageCanvasGroup.alpha = 0f;
        messageCanvasGroup.interactable = false;
        messageCanvasGroup.blocksRaycasts = false;

        // Hook up the button click
        startButton.onClick.AddListener(() => StartCoroutine(PlayIntroSequence()));
    }

    private IEnumerator PlayIntroSequence()
    {
        // Fade out the button
        yield return StartCoroutine(FadeOutButton());

        // Rotate the camera
        yield return StartCoroutine(RotateCamera());

        // Fade in the message
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

        // Adjust if rotation is negative
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

        // Make the text ready to appear
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
