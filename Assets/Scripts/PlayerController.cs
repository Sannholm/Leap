using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float jumpSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnJump()
    {
        Debug.Log("Jump");
        rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector3.right * movementSpeed * Time.fixedDeltaTime);
    }
}
