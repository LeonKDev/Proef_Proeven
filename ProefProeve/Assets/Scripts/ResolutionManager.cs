using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private int targetWidth = 1920;
    [SerializeField] private int targetHeight = 1080;
    [SerializeField] private bool maintainAspectRatio = true;

    private void Start()
    {
        SetResolution();
    }

    private void SetResolution()
    {
        // Set the target resolution
        Screen.SetResolution(targetWidth, targetHeight, Screen.fullScreen);

        if (maintainAspectRatio)
        {
            float targetAspect = (float)targetWidth / targetHeight;
            
            // Get the current screen's width and height
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float screenAspect = screenWidth / screenHeight;

            // Calculate the scale needed to fit while maintaining aspect ratio
            float scaleHeight = screenWidth / targetAspect;

            if (scaleHeight > screenHeight)
            {
                // If scaled height is greater than screen height, scale by height instead
                float scaleWidth = screenHeight * targetAspect;
                Screen.SetResolution((int)scaleWidth, (int)screenHeight, Screen.fullScreen);
            }
            else
            {
                // Otherwise scale by width
                Screen.SetResolution((int)screenWidth, (int)scaleHeight, Screen.fullScreen);
            }
        }
    }

    // Call this method if you need to change resolution at runtime
    public void UpdateResolution()
    {
        SetResolution();
    }
}