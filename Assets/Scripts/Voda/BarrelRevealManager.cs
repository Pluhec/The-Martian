using UnityEngine;

public class BarrelRevealManager : MonoBehaviour
{
    public GameObject barrel;

    void Awake()
    {
        if (barrel != null) barrel.SetActive(false);
    }
    
    public void ShowBarrel()
    {
        if (barrel != null)
            barrel.SetActive(true);
        else
            Debug.LogWarning("BarrelRevealManager: chybí reference na barrel!");
    }
    
    public void HideBarrel()
    {
        if (barrel != null)
            barrel.SetActive(false);
    }
}