using UnityEngine;


// Camera for matching the target resolution with bars added to maintain the aspect ratio
// 
// Features
// * maintains aspect ratio by adding letterbox to the sides of the view
// * scene agnostic solution, handling different resolution without having to modify scene hierarchy or gameobject scales
[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class LetterBoxedCameraController : MonoBehaviour
{
    private const int MIN_NUM_PIXELS = 480;
    private const int MAX_NUM_PIXELS = 7680;
    private const int DEFAULT_TARGET_WIDTH  = 1920;
    private const int DEFAULT_TARGET_HEIGHT = 1080;
    private readonly Rect FULL_VIEWPORT_RECT = new Rect(0, 0, 1, 1);

    [SerializeField] [Range(MIN_NUM_PIXELS, MAX_NUM_PIXELS)] private int targetWidth  = DEFAULT_TARGET_WIDTH;
    [SerializeField] [Range(MIN_NUM_PIXELS, MAX_NUM_PIXELS)] private int targetHeight = DEFAULT_TARGET_HEIGHT;

    private int currentWindowWidth;
    private int currentWindowHeight;

    void Update()
    {
        if (Screen.width != currentWindowWidth || Screen.height != currentWindowHeight)
        {
            currentWindowWidth  = Screen.width;
            currentWindowHeight = Screen.height;
            UpdateLetterbox();
        }
    }

    private void UpdateLetterbox()
    {
        float targetAspect = targetWidth / (float)targetHeight;
        float windowAspect = currentWindowWidth / (float)currentWindowHeight;
        float scaleHeight = windowAspect / targetAspect;

        Camera attachedCamera = GetComponent<Camera>();
        attachedCamera.rect = scaleHeight < 1.00f ? GetLetterboxRect(scaleHeight) : GetPillarboxRect(scaleHeight);
        if (attachedCamera.rect != FULL_VIEWPORT_RECT)
        {
            Debug.Log($"Current window resolution {currentWindowWidth}x{currentWindowHeight} " +
                      $"differs in aspect of target {targetWidth}x{targetHeight} - " +
                      $"adjusted viewport bounds to [{attachedCamera.rect.min}, ({attachedCamera.rect.max})])");
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
