using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    void Start()
    {
        
    }
    public bool isPaused()
    {
        return pausePanel != null && pausePanel.activeSelf;
    }
    public static PauseManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused())
            {

                PauseGame();
            }
            else if(isPaused())
            {
                UnpauseGame();
            }
          
        }
        
    }
    public void BackToMainMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            // 2. Shut down the network (works for Host, Client, or Server)
            NetworkManager.Singleton.Shutdown();

            // 3. Optional: Clear local references if needed
            Debug.Log("Network Shutdown successful.");
        }

        SceneManager.LoadScene("Map");
    }
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
    public void UnpauseGame()
    {
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

    }

}
