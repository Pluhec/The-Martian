using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class SlidingAirlockDoorTrigger : MonoBehaviour
{
    private static readonly int SideDoorOpenHash = Animator.StringToHash("SideDoorOpen");
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            animator.SetBool(SideDoorOpenHash, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            animator.SetBool(SideDoorOpenHash, false);
    }
}