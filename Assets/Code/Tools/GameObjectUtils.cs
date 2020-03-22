using UnityEngine;
using UnityEngine.UI;

public static class GameObjectUtils
{
    public static void SetAlpha(SpriteRenderer renderer, float alphaLevel)
    {
        Color color = renderer.color;
        renderer.color = new Color(color.r, color.g, color.b, alphaLevel);
    }
    public static void SetButtonVisibility(Button button, bool isVisible)
    {
        button.gameObject.SetActive(isVisible);
        button.enabled = isVisible;
    }
    public static void SetLabelVisibility(TMPro.TextMeshProUGUI label, bool isVisible)
    {
        label.enabled = isVisible;
    }
}
