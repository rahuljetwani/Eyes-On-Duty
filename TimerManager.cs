using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("Timer UI")]
    public Text timerText;

    [Header("Game Time Settings")]
    public float totalRealSeconds = 420f;         // 7 minutes
    private float elapsedRealSeconds = 0f;

    private int startHour = 23;
    private int endHour = 6;
    private int lastHourPlayed = -1;

    private bool isTimerRunning = true;

    [Header("Game WON UI")]
    public GameObject gameOverUI;

    [Header("Sound Settings")]
    public AudioSource hourlyAudioSource;      
    public AudioClip hourlyReminderSound;      

    void Update()
    {
        if (!isTimerRunning) return;

        elapsedRealSeconds += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedRealSeconds / totalRealSeconds);
        float inGameHoursPassed = progress * ((24 - startHour) + endHour);
        int currentHour = (int)(startHour + inGameHoursPassed) % 24;
        int currentMinute = (int)((inGameHoursPassed - Mathf.Floor(inGameHoursPassed)) * 60);

        string hourString = currentHour.ToString("D2");
        string minuteString = currentMinute.ToString("D2");
        timerText.text = $"{hourString}:{minuteString}";

        // sound
        if (currentMinute == 0 && currentHour != lastHourPlayed)
        {
            lastHourPlayed = currentHour;

            if (hourlyAudioSource != null && hourlyReminderSound != null)
                hourlyAudioSource.PlayOneShot(hourlyReminderSound);
        }
        
        if (elapsedRealSeconds >= totalRealSeconds)
        {
            isTimerRunning = false;
            timerText.text = "06:00";
            TriggerGameOver();
        }
    }
    

    void TriggerGameOver()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

