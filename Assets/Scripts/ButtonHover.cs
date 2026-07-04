using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Vector3 originalScale;
    private Material mat;

    public Animator anim; // 🔥 ADD THIS

    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    public float scaleMultiplier = 1.2f;
    public float speed = 10f;

    private Vector3 targetScale;
    private bool isClicked = false;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalScale = transform.localScale;
        targetScale = originalScale;

        mat = text.fontMaterial;
    }

    void Update()
    {
        if(!isClicked)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
        targetScale = originalScale * scaleMultiplier;

        mat.SetFloat("_UnderlayDilate", 0.6f);
        mat.SetFloat("_UnderlaySoftness", 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
        targetScale = originalScale;

        mat.SetFloat("_UnderlayDilate", 0.3f);
        mat.SetFloat("_UnderlaySoftness", 0.3f);
    }

    public void OnClickPlay()
    {
        if (anim != null)
            anim.SetTrigger("Click");
    }
}