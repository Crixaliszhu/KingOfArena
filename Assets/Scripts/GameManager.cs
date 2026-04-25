using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Fighter playerFighter;
    public Fighter enemyFighter;
    public Slider playerHPBar;
    public Slider enemyHPBar;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    private bool gameEnded = false;

    void Start()
    {
        playerFighter.OnHPChanged += OnPlayerHPChanged;
        enemyFighter.OnHPChanged += OnEnemyHPChanged;
        playerFighter.OnDeath += () => EndGame("You Lose!");
        enemyFighter.OnDeath += () => EndGame("You Win!");

        if (playerHPBar) playerHPBar.value = 1f;
        if (enemyHPBar) enemyHPBar.value = 1f;
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void OnPlayerHPChanged(int current, int max)
    {
        if (playerHPBar) playerHPBar.value = (float)current / max;
    }

    void OnEnemyHPChanged(int current, int max)
    {
        if (enemyHPBar) enemyHPBar.value = (float)current / max;
    }

    void EndGame(string message)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (gameOverText) gameOverText.text = message;

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
