using System.Collections.Generic;
using UnityEngine;
using SimplePieMenu;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private PieMenu pieMenu;
    [SerializeField] private GameObject menuItemPrefab; // Přidáme Prefab pro menu položku
    private List<GameObject> menuItems = new List<GameObject>(); 
    private List<string> actions = new List<string>();

    private void Start()
    {
        if (pieMenu != null)
        {
            pieMenu.OnPieMenuFullyInitialized += SetupMenu;
        }
    }

    public void SetActions(List<string> newActions)
    {
        actions = newActions;
        SetupMenu(); // Při změně akcí přegenerujeme menu
    }

    private void SetupMenu()
    {
        if (pieMenu == null || actions.Count == 0 || menuItemPrefab == null)
        {
            Debug.LogWarning("❌ Pie Menu nebo Prefab položky není přiřazeno.");
            return;
        }

        // ❌ Ručně odstraníme existující položky
        ClearExistingMenuItems();

        // ✅ Dynamicky nastavíme velikost Pie Menu na základě počtu položek
        float menuSize = Mathf.Clamp(actions.Count * 30f, 150f, 500f);
        pieMenu.transform.localScale = new Vector3(menuSize, menuSize, 1);

        // ✅ Přidáváme nové položky podle počtu akcí
        for (int i = 0; i < actions.Count; i++)
        {
            GameObject newMenuItem = Instantiate(menuItemPrefab, pieMenu.transform); // Opraveno!

            if (newMenuItem != null)
            {
                PieMenuItem menuItem = newMenuItem.GetComponent<PieMenuItem>();
                if (menuItem != null)
                {
                    menuItem.SetHeader(actions[i]);
                    menuItem.SetDetails($"Akce: {actions[i]}");

                    // ✅ Automaticky upravíme vzdálenost od středu Pie Menu
                    float angle = (360f / actions.Count) * i; // Rovnoměrné rozložení položek
                    newMenuItem.transform.rotation = Quaternion.Euler(0, 0, angle);
                    newMenuItem.transform.localPosition = new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad) * menuSize * 0.4f,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * menuSize * 0.4f,
                        0
                    );

                    // ✅ Přidáme klikací akci
                    DynamicClickHandler clickHandler = newMenuItem.AddComponent<DynamicClickHandler>();
                    int index = i;
                    clickHandler.SetAction(() => PerformAction(index));

                    menuItems.Add(newMenuItem);
                    newMenuItem.SetActive(true);
                }
            }
        }

        Debug.Log($"✅ Pie Menu aktualizováno! Počet položek: {menuItems.Count}, Velikost menu: {menuSize}");
    }

    private void ClearExistingMenuItems()
    {
        foreach (Transform child in pieMenu.transform)
        {
            Destroy(child.gameObject);
        }
        menuItems.Clear();
    }

    private void PerformAction(int actionIndex)
    {
        Debug.Log($"🎯 Akce spuštěna: {actions[actionIndex]}");
    }

    public void QuickAction()
    {
        if (actions.Count > 0)
        {
            PerformAction(0);
        }
    }
}
