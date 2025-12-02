using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Import the Input System namespace

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuCanvas; // Assign the PauseMenuCanvas in the Inspector
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f; // Ensure the game starts unpaused
        pauseMenuCanvas.SetActive(false); // Ensure the Pause Menu starts hidden
    }

    void Update()
    {
        // Check for the P key using the new Input System
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Ensure the game is unpaused before switching scenes
        SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene
    }
}