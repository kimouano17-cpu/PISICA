using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public Material ghostMat;
    public float lifeTime = 0.5f;

    float timer;

    Renderer[] rends;
    Color[] originalColors;

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        originalColors = new Color[rends.Length];

        for (int i = 0; i < rends.Length; i++)
        {
            // apply ghost material
            rends[i].material = ghostMat;

            // get correct color (URP safe)
            if (rends[i].material.HasProperty("_BaseColor"))
            {
                originalColors[i] = rends[i].material.GetColor("_BaseColor");
            }
            else
            {
                originalColors[i] = rends[i].material.color;
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);

        for (int i = 0; i < rends.Length; i++)
        {
            Material m = rends[i].material;

            if (m.HasProperty("_BaseColor"))
            {
                Color c = originalColors[i];
                c.a = alpha;
                m.SetColor("_BaseColor", c);
            }
            else
            {
                Color c = originalColors[i];
                c.a = alpha;
                m.color = c;
            }
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}