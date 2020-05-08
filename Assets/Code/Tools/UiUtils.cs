using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


// note that these methods use old style for loops so that the found value CAN be modified after return
public static class UiUtils
{
    public static void AddAutoUnsubscribeOnClickListenerToButton(Button button, System.Action onButtonClicked)
    {
        // note null initialization is required to force nonlocal scope of the handler, see https://stackoverflow.com/a/1362244
        UnityAction handler = null;
        handler = () =>
        {
            button.onClick.RemoveListener(handler);
            onButtonClicked.Invoke();
        };
        button.onClick.AddListener(handler);
    }

    public static void SetSpriteVisibility(SpriteRenderer spriteRenderer, bool isVisible)
    {
        spriteRenderer.enabled = isVisible;
    }
    public static void SetLabelVisibility(TMPro.TextMeshProUGUI label, bool isVisible)
    {
        label.enabled = isVisible;
    }
    // hide button without the overhead or implication of `SetActive`
    // assumes active button with or without a textmeshpro child-label
    public static void SetButtonVisibility(Button button, bool isVisible)
    {
        button.enabled       = isVisible;
        button.image.enabled = isVisible;

        TMPro.TextMeshProUGUI buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (buttonText)
        {
            buttonText.enabled = isVisible;
        }
    }
    // fully enable/disable the button
    // warning: unlike _just_ changing button visibility, this will affect ALL attached scripts/listeners
    public static void SetButtonActiveAndEnabled(Button button, bool isActiveAndEnabled)
    {
        button.gameObject.SetActive(isActiveAndEnabled);
        button.enabled       = isActiveAndEnabled;
        button.image.enabled = isActiveAndEnabled;
    }
}
