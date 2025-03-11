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
    private int currentSelectedRadialPart = -1;
    private Vector3 lastSpot = Vector3.zero;

    void Start()
    {

    }

    void Update()
    {
        // Record cursor position of the mouse
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        lastSpot += new Vector3(mouseX, 0, mouseY);
        if (lastSpot.x > 4) lastSpot.x = 4;
        if (lastSpot.x < -4) lastSpot.x = -4;
        if (lastSpot.z > 4) lastSpot.z = 4;
        if (lastSpot.z < -4) lastSpot.z = -4;

        spawnRadialPart();
        getSelectedRadialPart();
    }

    public void getSelectedRadialPart()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 centerToMouse = mousePosition - radialPartCanvas.position;
        Vector3 centerToMouseProjected = Vector3.ProjectOnPlane(centerToMouse, Vector3.forward);

        // Replace angle calculation
        float angle = Mathf.Atan2(centerToMouseProjected.x, centerToMouseProjected.y) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360;
        }

        currentSelectedRadialPart = (int) angle * numberOfRadialPart / 360;

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = Color.red;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white;
                spawnedParts[i].transform.localScale = 1f * Vector3.one;
            }
        }
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
            // Invert spawning angle
            float angle = - i * 360 / numberOfRadialPart - angleBetweenPart / 2 + 180;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.eulerAngles = radialPartEulerAngle;

            spawnedRadialPart.GetComponent<Image>().fillAmount = (1 / (float)numberOfRadialPart) - angleBetweenPart / 360;

            spawnedParts.Add(spawnedRadialPart);
        }
    }
}