using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource musicSource;

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}