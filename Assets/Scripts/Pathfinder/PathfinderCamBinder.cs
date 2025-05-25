using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PathfinderCamBinder : MonoBehaviour
{
    CinemachineCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(AssignWhenReady());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(AssignWhenReady());
    }

    IEnumerator AssignWhenReady()
    {
        PathfinderComputer pf = null;
        while ((pf = FindObjectOfType<PathfinderComputer>()) == null)
            yield return null;

        vcam.Follow = pf.transform;
        vcam.LookAt = pf.transform;
    }
}