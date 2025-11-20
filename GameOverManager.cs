using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverUI;
    public TMP_Text gameOverReasonText;

    public void TriggerGameOver(string reason)
    {
        gameOverUI.SetActive(true);
        gameOverReasonText.text = reason;
    }

    // Add this method for Quit Button:
    public void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
