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
        // pokud se scéna znovu načte, zkusíme přiřadit znovu
        SceneManager.sceneLoaded += OnSceneLoaded;
        // a hned při startu (první načtení)
        StartCoroutine(AssignWhenReady());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // po načtení scény znovu čekáme, až Pathfinder existuje
        StartCoroutine(AssignWhenReady());
    }

    IEnumerator AssignWhenReady()
    {
        // čekej, dokud se v hierarchii neobjeví PathfinderComputer
        PathfinderComputer pf = null;
        while ((pf = FindObjectOfType<PathfinderComputer>()) == null)
            yield return null;

        // jakmile je, přiřaď transform jako follow (a lookAt)
        vcam.Follow = pf.transform;
        vcam.LookAt = pf.transform;
    }
}