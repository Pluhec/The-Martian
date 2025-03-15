using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class RadialMenuEvent : UnityEvent<int> { }

public class RadialSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public TextMeshProUGUI actionText; // **Odkaz na ActionText v RadialInside**
    public float angleBetweenPart = 10;

    public Color32 selectedColor = new Color32(0x33, 0x33, 0x33, 0x0D);
    public Color32 defaultColor = Color.white;
    public float selectedScale = 1.1f;
    public float defaultScale = 1f;
    public float transitionSpeed = 10f;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;
    private int lastSelectedRadialPart = -1; // **Sledování poslední vybrané sekce**
    private List<string> actions = new List<string>();

    public RadialMenuEvent onPartSelected = new RadialMenuEvent();

    public void SetupMenu(List<string> actionList)
    {
        actions = actionList;

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
            image.raycastTarget = true;
            spawnedParts.Add(spawnedRadialPart);
        }
    }

    void Update()
    {
        getSelectedRadialPart();
        AnimateSelection();
        UpdateActionText(); // **Aktualizace textu při změně výběru**

        if (Input.GetMouseButtonDown(0)) 
        {
            if (currentSelectedRadialPart >= 0 && currentSelectedRadialPart < actions.Count)
            {
                Debug.Log($"Kliknuto na: {actions[currentSelectedRadialPart]}");
                onPartSelected.Invoke(currentSelectedRadialPart);
                gameObject.SetActive(false);
            }
        }
    }

    private void getSelectedRadialPart()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 centerToMouse = mousePosition - radialPartCanvas.position;
        Vector3 centerToMouseProjected = Vector3.ProjectOnPlane(centerToMouse, Vector3.forward);

        float angle = Mathf.Atan2(centerToMouseProjected.x, centerToMouseProjected.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360);
    }

    private void UpdateActionText()
    {
        if (currentSelectedRadialPart != lastSelectedRadialPart && actionText != null)
        {
            if (currentSelectedRadialPart >= 0 && currentSelectedRadialPart < actions.Count)
            {
                actionText.text = actions[currentSelectedRadialPart]; // **Přepíše text do ActionText**
            }
            else
            {
                actionText.text = ""; // **Pokud není žádná akce, text se smaže**
            }
            lastSelectedRadialPart = currentSelectedRadialPart; // **Uložíme poslední vybranou sekci**
        }
    }

    private void AnimateSelection()
    {
        for (int i = 0; i < spawnedParts.Count; i++)
        {
            Image img = spawnedParts[i].GetComponent<Image>();
            Transform tf = spawnedParts[i].transform;

            if (i == currentSelectedRadialPart)
            {
                img.color = Color.Lerp(img.color, selectedColor, Time.deltaTime * transitionSpeed);
                tf.localScale = Vector3.Lerp(tf.localScale, selectedScale * Vector3.one, Time.deltaTime * transitionSpeed);
            }
            else
            {
                img.color = Color.Lerp(img.color, defaultColor, Time.deltaTime * transitionSpeed);
                tf.localScale = Vector3.Lerp(tf.localScale, defaultScale * Vector3.one, Time.deltaTime * transitionSpeed);
            }
        }
    }
}
