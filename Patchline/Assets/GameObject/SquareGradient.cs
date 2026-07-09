using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class SquareGradient : MonoBehaviour
{
    public Color topColor = Color.white;
    public Color bottomColor = Color.black;
    public int textureSize = 128;

    private Image image;

    void OnEnable()
    {
        GenerateGradient();
    }

    void OnValidate()
    {
        GenerateGradient();
    }

    void GenerateGradient()
    {
        image = GetComponent<Image>();

        Texture2D tex = new Texture2D(textureSize, textureSize);
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < textureSize; y++)
        {
            float t = y / (float)(textureSize - 1);
            Color color = Color.Lerp(bottomColor, topColor, t);

            for (int x = 0; x < textureSize; x++)
            {
                tex.SetPixel(x, y, color);
            }
        }

        tex.Apply();

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize
        );

        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.color = Color.white;
    }
}