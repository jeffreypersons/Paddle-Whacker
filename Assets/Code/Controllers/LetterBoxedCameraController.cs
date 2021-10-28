using UnityEngine;


/*
Camera for matching the target resolution with bars added to maintain the aspect ratio

Features
* Maintains aspect ratio by adding letterbox to the sides of the view
* Scene agnostic solution, handling different resolution without having to modify scene hierarchy or gameobject scales
*/
[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class LetterBoxedCameraController : MonoBehaviour
{
    private const int MIN_NUM_PIXELS = 480;
    private const int MAX_NUM_PIXELS = 7680;
    private const int DEFAULT_TARGET_WIDTH  = 1920;
    private const int DEFAULT_TARGET_HEIGHT = 1080;
    private readonly Rect FULL_VIEWPORT_RECT = new Rect(0, 0, 1, 1);
    
    [SerializeField] [Range(MIN_NUM_PIXELS, MAX_NUM_PIXELS)] private int desiredWidth  = DEFAULT_TARGET_WIDTH;
    [SerializeField] [Range(MIN_NUM_PIXELS, MAX_NUM_PIXELS)] private int desiredHeight = DEFAULT_TARGET_HEIGHT;
    

    private RenderWindowResolution ActualResolution { get; set; }
    private RenderWindowResolution TargetResolution { get; set; }

    void Update()
    {
        if (!ActualResolution.IsSize(Screen.width, Screen.height) ||
            !TargetResolution.IsSize(desiredWidth, desiredHeight))
        {
            ActualResolution = new RenderWindowResolution(Screen.width, Screen.height);
            TargetResolution = new RenderWindowResolution(desiredWidth, desiredHeight);
            UpdateLetterbox();
        }
    }

    private void UpdateLetterbox()
    {
        float scaledHeight = ActualResolution.Aspect / TargetResolution.Aspect;
        Rect fittedRect = scaledHeight < 1.00f ? GetLetterboxRect(scaledHeight) : GetPillarboxRect(scaledHeight);
        if (fittedRect != FULL_VIEWPORT_RECT)
        {
            GetComponent<Camera>().rect = fittedRect;
            Debug.Log($"Detected mismatch between aspect ratios of " +
                      $"actual {ActualResolution.Width}x{ActualResolution.Height} and " +
                      $"desired {TargetResolution.Width}x{TargetResolution.Height} resolutions - "  +
                      $"adjusted viewport bounds to [{fittedRect.min}, {fittedRect.max}]");
        }
        else
        {
            Debug.Log($"Matched aspects between " +
                      $"actual {ActualResolution.Width}x{ActualResolution.Height} and " +
                      $"desired {TargetResolution.Width}x{TargetResolution.Height} resolutions - "  +
                      $"no resizing of viewport bounds needed");
        }
    }

    private Rect GetLetterboxRect(float scaleHeight)
    {
        return new Rect(0.00f, (1.00f - scaleHeight) * 0.50f, 1.00f, scaleHeight);
    }
    private Rect GetPillarboxRect(float scaleHeight)
    {
        float scalewidth = 1.00f / scaleHeight;
        return new Rect((1.00f - scalewidth) * 0.50f, 0.00f, scalewidth, 1.00f);
    }
}

public struct RenderWindowResolution
{
    public readonly int   Width;
    public readonly int   Height;
    public readonly float Aspect;
    
    public bool IsSize(int width, int height) => width == Width && height == Height;
    public override string ToString() => $"RenderWindowResolution{{size={Width}x{Height},aspect={Aspect}}}";
    public RenderWindowResolution(int width, int height)
    {
        Width  = width;
        Height = height;
        Aspect = width / (float)height;
    }
}
