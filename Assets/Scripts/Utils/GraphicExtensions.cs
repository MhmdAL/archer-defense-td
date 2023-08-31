using UnityEngine;
using UnityEngine.UI;

public static class GraphicExtensions
{
    public static void SetAlpha(this Image image, float alpha)
    {
        var imageColor = image.color;
        imageColor.a = alpha;
        image.color = imageColor;
    }

    public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        var spriteColor = spriteRenderer.color;
        spriteColor.a = alpha;
        spriteRenderer.color = spriteColor;
    }
}