using UnityEngine;

public class CircleGlow : MonoBehaviour
{
    public Light glowLight;

    void Start()
    {
        glowLight.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            glowLight.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            glowLight.enabled = false;
        }
    }
}