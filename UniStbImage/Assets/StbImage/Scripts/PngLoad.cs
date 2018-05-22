using System.IO;
using UnityEngine;


public class PngLoad : MonoBehaviour
{
    [SerializeField]
    Shader m_shader;

    private void Reset()
    {
        m_shader = Shader.Find("Unlit/Transparent");
    }

    // Use this for initialization
    void Start()
    {
        var material = new Material(m_shader);
        var renderer = GetComponent<Renderer>();
        renderer.sharedMaterial = material;
            
        var path = "Assets/StbImage/Resources/png.png";
        var bytes = File.ReadAllBytes(path);
        using (var image = StbImage.ImageLoader.Create(bytes))
        {
            if (image != null)
            {
                var texture = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false);
                var pixels = new byte[image.Width * image.Height * 4];
                image.CopyTo(pixels);
                texture.LoadRawTextureData(pixels);
                texture.Apply();

                material.mainTexture = texture;
            }
        }
    }
}
