using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelect"); // loads level select scene
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionsMenu"); // loads options/settings scene
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit"); // works inside Editor
        Application.Quit(); // works in built game
    }
}
