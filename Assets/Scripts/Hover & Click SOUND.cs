using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource source;

    public AudioClip hoverClip;
    public AudioClip clickClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null)
        {
            source.PlayOneShot(hoverClip);
            Debug.Log("Hover");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip != null)
        {
            source.PlayOneShot(clickClip);
             Debug.Log("Click");
        }
    }
}