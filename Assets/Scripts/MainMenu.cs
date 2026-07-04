using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuContainer;
    public GameObject playContainer;
    public GameObject settingsContainer;

    public void PlayGame()
    {
        Invoke("OpenPlayMenu", 0.25f);
    }
    private void OpenPlayMenu()
    {
        mainMenuContainer.SetActive(false);
        playContainer.SetActive(true);
    }

    public void OpenSettings()
    {
        mainMenuContainer.SetActive(false);
        settingsContainer.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // pang test sa editor
    }

    public void BackToMenu()
    {
        mainMenuContainer.SetActive(true);
        playContainer.SetActive(false);
        settingsContainer.SetActive(false);
    }
}