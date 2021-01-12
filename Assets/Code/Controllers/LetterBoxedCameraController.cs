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
    private const int DEFAULT_TARGET_WIDTH = 1920;
    private const int DEFAULT_TARGET_HEIGHT = 1080;

    [SerializeField] [Range(MIN_NUM_PIXELS, MAX_NUM_PIXELS)] private int targetWidth  = DEFAULT_TARGET_WIDTH;
    [SerializeField] [Range(MIN_NUM_PIXELS, MAX_NUM_PIXELS)] private int targetHeight = DEFAULT_TARGET_HEIGHT;

    private int currentScreenWidth;
    private int currentScreenHeight;

    void Update()
    {
        if (Screen.width != currentScreenWidth || Screen.height != currentScreenHeight)
        {
            currentScreenWidth  = Screen.width;
            currentScreenHeight = Screen.height;
            UpdateLetterbox();
        }
    }

    private void UpdateLetterbox()
    {
        float targetAspect = targetWidth / (float)targetHeight;
        float windowAspect = currentScreenWidth / (float)currentScreenHeight;
        float scaleHeight = windowAspect / targetAspect;

        Camera attachedCamera = GetComponent<Camera>();
        attachedCamera.rect = scaleHeight < 1.0f ? GetLetterboxRect(scaleHeight) : GetPillarboxRect(scaleHeight);
        Debug.Log($"Viewport adjusted to bounds [{attachedCamera.rect.min}, ({attachedCamera.rect.max})])");
    }
    private Rect GetLetterboxRect(float scaleHeight)
    {
        return new Rect(0, (1f - scaleHeight) / 2f, 1f, scaleHeight);
    }
    private Rect GetPillarboxRect(float scaleHeight)
    {
        float scalewidth = 1.0f / scaleHeight;
        return new Rect((1f - scalewidth) / 2f, 0, scalewidth, 1f);
    }
}
