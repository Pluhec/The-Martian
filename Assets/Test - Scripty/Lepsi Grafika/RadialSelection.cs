using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class RadialMenuEvent : UnityEvent<int> { }

public class RadialSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;

    public Color32 selectedColor = new Color32(0x33, 0x33, 0x33, 0x0D);
    public Color32 defaultColor = Color.white;
    public float selectedScale = 1.1f;
    public float defaultScale = 1f;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;
    private List<string> actions = new List<string>();

    public RadialMenuEvent onPartSelected = new RadialMenuEvent();

    public void SetupMenu(List<string> actionList)
    {
        actions = actionList;

        // Odstranění starých prvků
        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }
        spawnedParts.Clear();

        numberOfRadialPart = actions.Count;

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart / 2 + 180;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.eulerAngles = radialPartEulerAngle;

            Image image = spawnedRadialPart.GetComponent<Image>();
            image.fillAmount = (1f / numberOfRadialPart) - (angleBetweenPart / 360);
            image.raycastTarget = true; // Zajistíme, že klikání funguje
            spawnedParts.Add(spawnedRadialPart);
        }
    }

    void Update()
    {
        getSelectedRadialPart();

        if (Input.GetMouseButtonDown(0)) // Kliknutí levým tlačítkem myši
        {
            if (currentSelectedRadialPart >= 0 && currentSelectedRadialPart < actions.Count)
            {
                Debug.Log($"Kliknuto na: {actions[currentSelectedRadialPart]}");
                onPartSelected.Invoke(currentSelectedRadialPart); // Zavolání akce
                gameObject.SetActive(false); // Zavření menu
            }
        }
    }

    public void getSelectedRadialPart()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 centerToMouse = mousePosition - radialPartCanvas.position;
        Vector3 centerToMouseProjected = Vector3.ProjectOnPlane(centerToMouse, Vector3.forward);

        float angle = Mathf.Atan2(centerToMouseProjected.x, centerToMouseProjected.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360);

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = selectedColor;
                spawnedParts[i].transform.localScale = selectedScale * Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = defaultColor;
                spawnedParts[i].transform.localScale = defaultScale * Vector3.one;
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