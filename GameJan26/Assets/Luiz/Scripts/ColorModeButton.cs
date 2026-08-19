using UnityEngine;

public class ColorModeButton : MonoBehaviour
{
    public ColorMode mode;

    public void SelectColorMode()
    {
        ColorAccessibilityManager.Instance.SetColorMode(mode);
    }
}