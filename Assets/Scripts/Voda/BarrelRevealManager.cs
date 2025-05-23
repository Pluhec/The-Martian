using UnityEngine;

public class BarrelRevealManager : MonoBehaviour
{
    [Tooltip("Sem přetáhni statický Barrel GameObject (s WaterGameInteraction a Triggerem), který je v inspektoru vypnutý")]
    public GameObject barrel;

    void Awake()
    {
        // ujistíme se, že barrel je zpočátku neaktivní
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