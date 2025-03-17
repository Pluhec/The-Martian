using System.Collections.Generic;
using UnityEngine;
using System;

public class SolSystem : MonoBehaviour
{
    public static SolSystem Instance { get; private set; }

    [Header("Sol nastavení")]
    public int currentSol;
    public DateTime currentTime;
    public List<SolData> solDataList;

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

        SolData currentSolData = solDataList.Find(sol => sol.solNumber == solNumber);
        if (currentSolData != null)
        {
            QuestManager.Instance.InitializeQuests(currentSolData.quests);
        }
        else
        {
            Debug.LogWarning("SolSystem: Data pro sol " + solNumber + " nebyla nalezena!");
        }
    }

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
            Debug.Log("SolSystem: Další sol není dostupný.");
        }
    }

    public int GetNextSol()
    {
        int currentIndex = solDataList.FindIndex(sol => sol.solNumber == currentSol);
        if (currentIndex >= 0 && currentIndex < solDataList.Count - 1)
            return solDataList[currentIndex + 1].solNumber;
        return -1;
    }
}