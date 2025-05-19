using UnityEngine;

public class BloodScreenCheck : MonoBehaviour
{
    public Canvas bloodOverlayImage;
    
    void Start()
    {
        if (PlayerPrefs.GetInt("BloodOverlayDisabled", 0) == 1)
        {
            if (bloodOverlayImage != null)
                bloodOverlayImage.enabled = false;
        }
    }
    
}
