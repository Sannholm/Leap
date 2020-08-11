using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningCharacterController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private float prevPosX;

    void Start()
    {
        prevPosX = transform.position.x;
    }

    void Update()
    {
        float velX = Time.deltaTime != 0 ? (transform.position.x - prevPosX) / Time.deltaTime : 0;
        animator.SetFloat("RunSpeed", velX);

        prevPosX = transform.position.x;
    }

    public void Jump(float duration)
    {
        animator.SetTrigger("Jump");
        animator.SetFloat("JumpSpeedMul", 1 / duration);
    }
}
