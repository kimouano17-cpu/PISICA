using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{
    
  

    [SerializeField] private GameObject pauseContainer;
    [SerializeField] private GameObject pauseConfirmation;

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseContainer.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseContainer.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowPauseConfirmation()
    {
        pauseConfirmation.SetActive(true);
        pauseContainer.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Map");
    }
    public void HidePauseConfirmation()
    {
        pauseConfirmation.SetActive(false);
        pauseContainer.SetActive(true);
    }
}