using System;
using System.Collections.Generic;
using UnityEngine;

public class SolSystem : MonoBehaviour
{
    
    public static SolSystem Instance { get; private set; }

    [Header("Sol nastavení")]
    public int currentSol;
    public DateTime currentTime;
    public List<SolData> solDataList;

    [Header("Reference na ostatní systémy")]
    public QuestManager questManager;
    public TimeManager timeManager;
    public TerminalManager TerminalManager;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // sposteni solo ktery je nastaven v inspektoru
    private void Start()
    {
        if (solDataList != null && solDataList.Count > 0)
        {
            SolData foundSol = solDataList.Find(s => s.solNumber == currentSol);

            if (foundSol != null)
            {
                StartNewSol(currentSol);
            }
            else
            {
                // pokud sol v inspektoru neni nastaveny, spusti se prvni sol v seznamu
                Debug.LogWarning("SolSystem: Zadaný currentSol v Inspectoru (" + currentSol + ") nebyl nalezen v solDataList. Spouštím první sol v seznamu.");
                StartNewSol(solDataList[0].solNumber);
            }
        }
        else
        {
            Debug.LogError("SolSystem: Seznam solData je prázdný!");
        }
    }
    
    public void StartNewSol(int solNumber)
    {
        currentSol = solNumber;
        currentTime = new DateTime(1, 1, 1, 8, 0, 0);
        Debug.Log("spousitm Sol " + currentSol + " v case " + currentTime.ToShortTimeString());

        // vyhledani dat pro dany sol
        SolData currentSolData = solDataList.Find(sol => sol.solNumber == solNumber);
        if (currentSolData != null)
        {
            questManager.InitializeQuests(currentSolData.quests);
        }
        else
        {
            Debug.LogWarning("SolSystem: Data pro sol " + solNumber + " nebyla nalezena!");
        }

        // restart herniho casu a tim se zmeni i svetlo
        timeManager.ResetTime();
    }

    // ukonceni solu a prechod na dalsi
    public void EndCurrentSol()
    {
        Debug.Log("Ukončuji Sol " + currentSol);
        int nextSol = GetNextSol();
        if (nextSol != -1)
        {
            StartNewSol(nextSol);
            
            TerminalManager.DisplayGoodMorning();
        }
        else
        {
            Debug.Log("SolSystem: dalsi sol neni dostupny");
        }
    }

    // ziskani dalsiho solu v seznamu
    public int GetNextSol()
    {
        int currentIndex = solDataList.FindIndex(sol => sol.solNumber == currentSol);
        if (currentIndex >= 0 && currentIndex < solDataList.Count - 1)
        {
            return solDataList[currentIndex + 1].solNumber;
        }
        return -1;
    }
}