using System.Collections;
using TMPro;
using Unity.AppUI.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button hostButton;
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private UnityEngine.UI.Button joinButton;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private TMP_Text portText;


    private void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);

        startButton.onClick.AddListener(StartFadeIn);
        joinButton.onClick.AddListener(JoinGame);
    }


    void Update()
    {
        
    }
    public void HostButtonOnClick()
    {
        ushort lobbyPort = (ushort)Random.Range(7777, 30000);

        // 2. Reference the Transport and set the port
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Port = lobbyPort;
            Debug.Log($"Hosting on Port: {lobbyPort}");
        }


        NetworkManager.Singleton.StartHost();

        portText.SetText($"Port:  {lobbyPort}");


    }
  
    
    public void StartFadeIn()
    {
        StartCoroutine(StartDelay());
        blackScreen.GetComponent<Animator>().SetBool("isFade", false);

    }
    public void StartGame()
    {
      
        NetworkManager.Singleton.SceneManager.LoadScene("Main Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void JoinGame()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport != null && !string.IsNullOrEmpty(portInputField.text))
        {
            // Parse the text from the InputField into a ushort
            if (ushort.TryParse(portInputField.text, out ushort targetPort))
            {
                transport.ConnectionData.Port = targetPort;
                Debug.Log($"Attempting to join on Port: {targetPort}");
            }
            else
            {
                Debug.LogError("Invalid Port entered!");
                return; // Stop if the port is invalid
            }
        }

   
        NetworkManager.Singleton.StartClient();
    }
    public void CloseHost()
    {
        if (NetworkManager.Singleton != null)
        {
            // 2. Shut down the network (works for Host, Client, or Server)
            NetworkManager.Singleton.Shutdown();

            // 3. Optional: Clear local references if needed
            Debug.Log("Network Shutdown successful.");
        }
    }
    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(6f);
        StartGame();
    }
}
