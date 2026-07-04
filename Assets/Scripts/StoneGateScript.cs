using Unity.Netcode;
using UnityEngine;

public class StoneGateScript : MonoBehaviour
{
    [SerializeField] Animator animator;
    public GameObject floatingButtonUI = null;


    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;

    public bool isInTrigger;

    void Start()
    {
       
    }
    public static PauseManager Instance { get; private set; }

    void Update()
    {
       
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();

            // Check the SYNCED variable
            if (player != null && player.IsLocalPlayer && !player.IsReadySynced.Value)
            {
                isInTrigger = true;
                floatingButtonUI.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isInTrigger = false;
    }
    public void OpenGate()
    {
        animator.SetBool("IsOpen", true);
        PlaySound(openSFX);
    }
    public void CloseGate()
    {
        animator.SetBool("IsOpen", false);
    }
    public void OnConfirmReady() // Link THIS to the Button's OnClick event
    {
        // 1. Tell the player script to register as 'Ready' on the network
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.LocalClient != null)
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();

            if (localPlayer != null)
            {
                
                localPlayer.SetReadyRpc();
               
            }
        }


        GameManager.Instance.confirmationWindow.SetActive(false);
        floatingButtonUI.SetActive(false);

        
        isInTrigger = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // PlayOneShot para hindi maputol ang tunog kung magkasunod na tinawag
            audioSource.PlayOneShot(clip); 
        }
    }
}
