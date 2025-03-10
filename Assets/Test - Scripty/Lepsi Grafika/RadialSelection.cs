using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadialSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    
    private List<GameObject> spawnedParts = new List<GameObject>();
    
    void Start()
    {
        
    }

    void Update()
    {
        spawnRadialPart();
    }

    public void spawnRadialPart()
    {
        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }
        
        spawnedParts.Clear();
        
        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = i * 360 / numberOfRadialPart - angleBetweenPart / 2; 
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);
            
            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.eulerAngles = radialPartEulerAngle;
            
            spawnedRadialPart.GetComponent<Image>().fillAmount = (1 / (float)numberOfRadialPart) - angleBetweenPart / 360;
            
            spawnedParts.Add(spawnedRadialPart);
        }
    }
}
