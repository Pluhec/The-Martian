using UnityEngine;
using Unity.Cinemachine;

public class PathfinderComputer : InteractableObject
{
    [Header("Odkazy")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private CinemachineCamera cmPlayerCam;
    [SerializeField] private CinemachineCamera cmPathfinderCam;
    public GameObject pathfinderRoot;

    private Movement playerMovement;
    private PathfinderHexInput pathfinderHexInput;

    private void Awake()
    {
        if (playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerMovement == null && playerObject != null)
            playerMovement = playerObject.GetComponent<Movement>();

        // 2) Cinemachine kamery
        if (cmPlayerCam == null)
        {
            var go = GameObject.Find("CmCam");
            if (go != null) cmPlayerCam = go.GetComponent<CinemachineCamera>();
        }

        if (cmPathfinderCam == null)
        {
            var go = GameObject.Find("CmPathfinder");
            if (go != null) cmPathfinderCam = go.GetComponent<CinemachineCamera>();
        }

        if (pathfinderRoot != null)
        {
            pathfinderHexInput = pathfinderRoot.GetComponent<PathfinderHexInput>();
            if (pathfinderHexInput != null)
            {
                pathfinderHexInput.enabled = false;
                // Předání referencí na kamery
                pathfinderHexInput.cmPlayerCam = cmPlayerCam;
                pathfinderHexInput.cmPathfinderCam = cmPathfinderCam;
                pathfinderHexInput.playerMovement = playerMovement;
            }
        }

        if (!actions.Contains("Use"))
        {
            actions.Add("Use");
        }
    }

    public override void PerformAction(string action)
    {
        if (action == "Use")
        {
            Debug.Log("Pathfinder aktivován.");

            // 1. Vypnutí pohybu hráče
            if (playerMovement != null)
                playerMovement.enabled = false;

            // 2. Přepnutí kamer pomocí priorit
            if (cmPlayerCam != null)
                cmPlayerCam.Priority = 5;

            if (cmPathfinderCam != null)
                cmPathfinderCam.Priority = 20;

            // 3. Aktivace a spuštění pathfinder systému
            if (pathfinderHexInput != null)
            {
                pathfinderHexInput.enabled = true;
                pathfinderHexInput.StartPathfinder();
            }
        }
        else
        {
            base.PerformAction(action);
        }
    }
}