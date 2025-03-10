using System;
using System.Collections.Generic;
using UnityEngine;

public class SolSystem : MonoBehaviour
{
    [Header("Sol nastavení")]
    public int currentSol;
    public DateTime currentTime;
    // Seznam definic jednotlivých solů – nastavte jej v Inspectoru (pořadí v listu definuje posloupnost)
    public List<SolData> solDataList;

    [Header("Reference na ostatní systémy")]
    public QuestManager questManager;
    public TimeManager timeManager;

    private void Start()
    {
        // Při spuštění zkontrolujeme, zda máme nějaká data; pokud ano, spustíme první sol
        if (solDataList != null && solDataList.Count > 0)
        {
            StartNewSol(solDataList[0].solNumber);
        }
        else
        {
            Debug.LogError("SolSystem: Seznam solData je prázdný!");
        }
    }

    // Inicializace nového solu
    public void StartNewSol(int solNumber)
    {
        currentSol = solNumber;
        // Nastavení počátečního času – zde 8:00 (datum je irelevantní)
        currentTime = new DateTime(1, 1, 1, 8, 0, 0);
        Debug.Log("Spouštím Sol " + currentSol + " v čase " + currentTime.ToShortTimeString());

        // Vyhledání dat pro aktuální sol
        SolData currentSolData = solDataList.Find(sol => sol.solNumber == solNumber);
        if (currentSolData != null)
        {
            // Inicializace questů pro daný sol
            questManager.InitializeQuests(currentSolData.quests);
        }
        else
        {
            Debug.LogWarning("SolSystem: Data pro sol " + solNumber + " nebyla nalezena!");
        }

        // Resetujeme herní čas v TimeManageru (tím nastavíme také vizuální efekty)
        timeManager.ResetTime();
    }

    // Ukončí aktuální sol a spustí další
    public void EndCurrentSol()
    {
        Debug.Log("Ukončuji Sol " + currentSol);
        int nextSol = GetNextSol();
        if (nextSol != -1)
        {
            StartNewSol(nextSol);
        }
        else
        {
            Debug.Log("SolSystem: Další sol není dostupný. Hra může skončit nebo se spustí konečný stav.");
        }
    }

    // Získá číslo dalšího solu dle pořadí v seznamu (umožňuje přeskočení některých solů)
    public int GetNextSol()
    {
        int currentIndex = solDataList.FindIndex(sol => sol.solNumber == currentSol);
        if (currentIndex >= 0 && currentIndex < solDataList.Count - 1)
        {
            return solDataList[currentIndex + 1].solNumber;
        }
        return -1; // není k dispozici další sol
    }
}