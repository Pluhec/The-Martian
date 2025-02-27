using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;

    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;

    private Vector2 input;
    private Vector2 lastMoveDirection;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInputs();
        Animate();
    }

    private void FixedUpdate()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        body.linearVelocity = new Vector2(input.x * speed, input.y * speed);
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            lastMoveDirection = new Vector2(moveX, moveY);
        }

        input.x = moveX;
        input.y = moveY;
        input.Normalize();
    }

    void Animate()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
        anim.SetBool("IsRunning", isRunning);
    }
}