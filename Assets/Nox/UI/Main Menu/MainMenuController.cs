using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Nox"); // Replace with your game's main scene name
    }

    public void OpenEquipment()
    {
        Debug.Log("Equipment menu will be added later.");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings menu will be added later.");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}