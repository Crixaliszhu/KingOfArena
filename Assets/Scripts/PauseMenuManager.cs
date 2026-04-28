using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    public static bool IsPaused { get; private set; } = false;

    void Start()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        IsPaused = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
