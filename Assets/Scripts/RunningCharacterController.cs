using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningCharacterController : MonoBehaviour
{
    public Animator animator;

    private float prevPosX;

    void Start()
    {
        prevPosX = transform.position.x;
    }

    void Update()
    {
        float velX = (transform.position.x - prevPosX) / Time.deltaTime;
        animator.SetFloat("RunSpeed", velX);

        prevPosX = transform.position.x;
    }

    public void Jump(float duration)
    {
        animator.SetTrigger("Jump");
        animator.SetFloat("JumpSpeedMul", 1/duration);
    }
}
