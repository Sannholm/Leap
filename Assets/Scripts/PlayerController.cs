using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float jumpSpeed = 10f;
    public float onGroundMargin = 0.2f;
    public LayerMask groundLayerMask;

    private Rigidbody rb;
    private CapsuleCollider coll;

    private MovementFunc? currMovementFunction;
    private float movementFunctionTime;
    private Vector2 movementFunctionStartPos;

    private ISet<Platform> visitedPlatforms = new HashSet<Platform>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    void OnJump()
    {
        Platform platform = GetOnPlatform();
        if (platform != null)
        {
            StartMovementFunction(platform.GetJumpFunction());
        }
    }

    void FixedUpdate()
    {
        if (!currMovementFunction.HasValue)
        {
            rb.velocity = new Vector3(movementSpeed, rb.velocity.y, 0);
        }
        else
        {
            Func<float, Vector2> f = currMovementFunction.Value.f;
            Vector2 offset = f(movementFunctionTime);
            rb.MovePosition(movementFunctionStartPos + offset);
            rb.velocity = (f(movementFunctionTime + Time.fixedDeltaTime) - offset) / Time.fixedDeltaTime;
            movementFunctionTime += Time.fixedDeltaTime;
        }
    }

    private void StartMovementFunction(MovementFunc f)
    {
        currMovementFunction = f;
        movementFunctionTime = 0;
        movementFunctionStartPos = rb.position;
    }

    private void StopMovementFunction()
    {
        currMovementFunction = null;
    }

    private Platform GetOnPlatform()
    {
        Vector3 center = coll.bounds.center + Vector3.down * (coll.height/2 - coll.radius + onGroundMargin);
        Collider[] colliders = Physics.OverlapBox(center, Vector3.one*coll.radius, Quaternion.identity, groundLayerMask, QueryTriggerInteraction.Ignore);
        Platform foundPlatform = null;
        foreach (var collider in colliders)
        {
            Platform platform = collider.gameObject.GetComponentInParent<Platform>();
            if (platform != null)
            {
                foundPlatform = platform;
                break;
            }
        }
        return foundPlatform;
    }

    void OnTriggerStay(Collider collider)
    {
        Platform platform = collider.gameObject.GetComponentInParent<Platform>();
        if (platform != null && rb.velocity.y <= 0 && platform.isAbovePlatform(coll.bounds.min.y) && !visitedPlatforms.Contains(platform)) {
            visitedPlatforms.Add(platform);
            platform.OnPlayerLand();
            Debug.Log("Land");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        StopMovementFunction();
    }
}
