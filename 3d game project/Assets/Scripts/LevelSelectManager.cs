using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // loads main menu scene
    }

    public void LevelOne()
    {
        SceneManager.LoadScene("Level1"); // loads level one scene
    }

    public void LevelTwo()
    {
        SceneManager.LoadScene("Level2"); // loads level two scene
    }

    public void LevelThree()
    {
        SceneManager.LoadScene("Level3"); // loads level three scene
    }
}
