using UnityEngine;


public static class GameObjectUtils
{
    public static void SetAlpha(SpriteRenderer renderer, float alphaLevel)
    {
        Color color = renderer.color;
        renderer.color = new Color(color.r, color.g, color.b, alphaLevel);
    }
}
