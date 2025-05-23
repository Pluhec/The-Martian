using UnityEngine;
using System.Collections.Generic;

public class PotatoAppearanceManager : MonoBehaviour
{
    [System.Serializable]
    public class PotatoSpriteMapping
    {
        public int solNumber;
        public Sprite potatoSprite;
    }
    
    [Header("Nastavení změny vzhledu brambor")]
    public List<PotatoSpriteMapping> solSpriteMapping = new List<PotatoSpriteMapping>();
    public GameObject[] potatoPrefabs;  // Reference na herní objekty brambor
    
    [Header("Další nastavení")]
    public Sprite defaultPotatoSprite;  // Výchozí sprite, když není mapování pro daný sol

    private SolSystem solSystem;
    private int lastCheckedSol = -1;

    private void Awake()
    {
        solSystem = SolSystem.Instance;
        if (solSystem == null)
            Debug.LogWarning("SolSystem není k dispozici");
            
        // Pokud nejsou nastaveny reference na brambory, zkusíme je najít
        if (potatoPrefabs == null || potatoPrefabs.Length == 0)
        {
            PotatoFieldController controller = GetComponent<PotatoFieldController>();
            if (controller != null)
            {
                potatoPrefabs = controller.potatoPrefabs;
            }
        }
    }

    private void Start()
    {
        // Počáteční aktualizace vzhledu
        UpdatePotatoesAppearance();
    }

    private void Update()
    {
        // Kontrola změny solu
        if (solSystem != null && solSystem.currentSol != lastCheckedSol)
        {
            UpdatePotatoesAppearance();
            lastCheckedSol = solSystem.currentSol;
        }
    }

    // Metoda pro aktualizaci vzhledu všech aktivních brambor
    public void UpdatePotatoesAppearance()
    {
        if (solSystem == null || potatoPrefabs == null)
            return;

        int currentSol = solSystem.currentSol;
        
        // Hledání mapování pro aktuální sol
        PotatoSpriteMapping mapping = solSpriteMapping.Find(m => m.solNumber == currentSol);
        Sprite spriteToUse = (mapping != null && mapping.potatoSprite != null) ? 
                              mapping.potatoSprite : defaultPotatoSprite;
        
        if (spriteToUse != null)
        {
            foreach (GameObject potato in potatoPrefabs)
            {
                if (potato.activeSelf)  // Aktualizuj pouze aktivní brambory
                {
                    SpriteRenderer renderer = potato.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.sprite = spriteToUse;
                    }
                }
            }
            Debug.Log($"Aktualizován vzhled brambor pro sol {currentSol}");
        }
    }

    // Metoda, kterou lze volat z PotatoFieldController při zasazení nové brambory
    public void UpdateSinglePotato(GameObject potato)
    {
        if (solSystem == null || potato == null)
            return;
            
        int currentSol = solSystem.currentSol;
        PotatoSpriteMapping mapping = solSpriteMapping.Find(m => m.solNumber == currentSol);
        Sprite spriteToUse = (mapping != null && mapping.potatoSprite != null) ? 
                              mapping.potatoSprite : defaultPotatoSprite;
                              
        if (spriteToUse != null)
        {
            SpriteRenderer renderer = potato.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = spriteToUse;
            }
        }
    }
}