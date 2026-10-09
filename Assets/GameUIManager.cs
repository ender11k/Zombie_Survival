using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    private int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthText == null) return;
        healthText.text = $"Health: {Mathf.Max(0, currentHealth)} / 3";
    }

    public void AddScore(int points)
    {
        score += points;
        if (scoreText != null) scoreText.text = "Kills: " + score;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game logic
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause time BEFORE loading scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}