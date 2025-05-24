using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PotatoFieldGenerator : EditorWindow
{
    private GameObject potatoPrefab;
    private GameObject parentObject;
    private BoxCollider2D spawnArea;
    private int potatoCount = 30;
    private float spacingMultiplier = 1.0f;
    private Vector3 rotation = Vector3.zero;
    private Vector3 scale = Vector3.one;

    [MenuItem("Tools/Potato Field Generator")]
    public static void ShowWindow()
    {
        GetWindow<PotatoFieldGenerator>("Potato Field Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generátor bramborového pole", EditorStyles.boldLabel);

        potatoPrefab = EditorGUILayout.ObjectField("Prefab brambory", potatoPrefab, typeof(GameObject), false) as GameObject;
        parentObject = EditorGUILayout.ObjectField("Rodičovský objekt", parentObject, typeof(GameObject), true) as GameObject;
        spawnArea = EditorGUILayout.ObjectField("Oblast generování (BoxCollider2D)", spawnArea, typeof(BoxCollider2D), true) as BoxCollider2D;
        potatoCount = EditorGUILayout.IntField("Počet brambor", potatoCount);
        spacingMultiplier = EditorGUILayout.Slider("Násobič mezer", spacingMultiplier, 0.5f, 2.0f);
        rotation = EditorGUILayout.Vector3Field("Rotace brambor", rotation);
        scale = EditorGUILayout.Vector3Field("Měřítko brambor", scale);

        GUILayout.Space(10);

        if (GUILayout.Button("Generovat bramborové pole"))
        {
            GeneratePotatoField();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Aktualizovat PotatoFieldController"))
        {
            UpdatePotatoFieldController();
        }
    }

    private void GeneratePotatoField()
{
    if (potatoPrefab == null || parentObject == null || spawnArea == null)
    {
        EditorUtility.DisplayDialog("Chyba", "Prosím vyplňte všechna pole.", "OK");
        return;
    }

    // Odstranění existujících brambor
    List<GameObject> existingPotatoes = new List<GameObject>();
    for (int i = 0; i < parentObject.transform.childCount; i++)
    {
        Transform child = parentObject.transform.GetChild(i);
        if (child.name.Contains("Potato"))
            existingPotatoes.Add(child.gameObject);
    }

    foreach (GameObject potato in existingPotatoes)
    {
        DestroyImmediate(potato);
    }

    // Výpočet rozmístění brambor
    Vector2 areaSize = spawnArea.size;
    Vector2 areaCenter = (Vector2)spawnArea.transform.position + spawnArea.offset;

    // Výpočet optimálního počtu řádků a sloupců
    int rows = Mathf.CeilToInt(Mathf.Sqrt(potatoCount));
    int cols = Mathf.CeilToInt((float)potatoCount / rows);

    // Kontrola, zda je dostatek řádků a sloupců
    if (rows == 0) rows = 1;
    if (cols == 0) cols = 1;

    // Výpočet mezer jako jednotné hodnoty pro všechny směry - včetně okrajů
    float xSpacing = areaSize.x / (cols + 1);
    float ySpacing = areaSize.y / (rows + 1);
    
    // Aplikace násobičů mezery
    xSpacing *= spacingMultiplier;
    ySpacing *= spacingMultiplier;
    
    // Vypočítej levý horní roh oblasti (počáteční bod)
    Vector2 topLeft = new Vector2(
        areaCenter.x - areaSize.x / 2,
        areaCenter.y + areaSize.y / 2
    );

    // Generování brambor
    Undo.RegisterCompleteObjectUndo(parentObject, "Generate Potato Field");

    int potatoIndex = 0;
    for (int row = 0; row < rows && potatoIndex < potatoCount; row++)
    {
        for (int col = 0; col < cols && potatoIndex < potatoCount; col++)
        {
            // Počítej pozici s rovnoměrnými mezerami od okrajů i mezi bramborami
            Vector2 position = new Vector2(
                topLeft.x + xSpacing * (col + 1),
                topLeft.y - ySpacing * (row + 1)
            );

            GameObject potato = PrefabUtility.InstantiatePrefab(potatoPrefab) as GameObject;
            potato.transform.SetParent(parentObject.transform);
            potato.transform.position = position;
            potato.transform.eulerAngles = rotation;
            potato.transform.localScale = scale;
            potato.name = "Potato_" + potatoIndex;
            potato.SetActive(false); // Brambory začínají neaktivní

            potatoIndex++;
        }
    }

    EditorUtility.SetDirty(parentObject);
    Debug.Log($"Vygenerováno {potatoIndex} brambor v {rows}x{cols} mřížce.");
}

    private void UpdatePotatoFieldController()
    {
        if (parentObject == null)
        {
            EditorUtility.DisplayDialog("Chyba", "Nejprve vyberte rodičovský objekt.", "OK");
            return;
        }

        PotatoFieldController controller = parentObject.GetComponent<PotatoFieldController>();
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Chyba", "Rodičovský objekt nemá komponentu PotatoFieldController.", "OK");
            return;
        }

        // Hledání všech brambor a přidání do controlleru
        List<GameObject> potatoes = new List<GameObject>();
        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            Transform child = parentObject.transform.GetChild(i);
            if (child.name.Contains("Potato"))
                potatoes.Add(child.gameObject);
        }

        // Seřazení brambor podle názvu (čísla)
        potatoes.Sort((a, b) => {
            int aIndex = int.Parse(a.name.Split('_')[1]);
            int bIndex = int.Parse(b.name.Split('_')[1]);
            return aIndex.CompareTo(bIndex);
        });

        // Aktualizace pole v controlleru
        Undo.RegisterCompleteObjectUndo(controller, "Update PotatoFieldController");
        controller.potatoPrefabs = potatoes.ToArray();
        controller.maxPotatoCount = potatoes.Count;
        
        EditorUtility.SetDirty(controller);
        Debug.Log($"PotatoFieldController byl aktualizován s {potatoes.Count} bramborami.");
    }
}