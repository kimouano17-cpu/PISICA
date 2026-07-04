using UnityEngine;

public class EnemyMusicTrigger : MonoBehaviour
{
    AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            music.Play();
        }
    }
}