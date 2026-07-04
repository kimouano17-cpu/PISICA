using UnityEngine;
using System.IO;

public class ItemIconGenerator : MonoBehaviour
{
    public Camera iconCamera;
    public int resolution = 256;

    [ContextMenu("Generate Icon")]
    void GenerateIcon()
    {
        if(iconCamera == null)
        {
            Debug.LogError("Assign Icon Camera");
            return;
        }

        RenderTexture rt = new RenderTexture(resolution,resolution,24);
        iconCamera.targetTexture = rt;

        Texture2D tex = new Texture2D(resolution,resolution,TextureFormat.ARGB32,false);

        iconCamera.Render();

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0,0,resolution,resolution),0,0);
        tex.Apply();

        iconCamera.targetTexture = null;
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Assets/PotionIcon.png", bytes);

        Debug.Log("Icon Generated");
    }
}