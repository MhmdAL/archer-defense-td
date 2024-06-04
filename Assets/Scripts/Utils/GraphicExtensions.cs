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

    public static void DrawCircle(this LineRenderer renderer, Vector3 center, float radius) => DrawArc(renderer, 0, 360, center, radius);

    public static void DrawArc(this LineRenderer renderer, float startAngle, float endAngle, Vector3 center, float radius)
    {
        int steps = 500;
        float angleRange = endAngle - startAngle;

        renderer.positionCount = steps + 1;

        for (int currentStep = 0; currentStep <= steps; currentStep++)
        {
            float circProgress = (float)currentStep / steps;

            // Calculate the current angle within the specified range
            float currentRadian = (startAngle + circProgress * angleRange) * Mathf.Deg2Rad;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            var curPos = new Vector3(x, y, 0);

            renderer.SetPosition(currentStep, curPos + center);
        }
    }
}