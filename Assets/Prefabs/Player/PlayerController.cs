using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlatformEvent : UnityEvent<Platform> {}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameConfig gameConfig;

    [SerializeField]
    private float onGroundMargin = 0.2f;
    [SerializeField]
    private RunningCharacterController character;

    public PlatformEvent Jumped;
    public PlatformEvent Landed;

    private Rigidbody rb;
    private CapsuleCollider coll;

    private bool movementStopped = false;
    private float movementSpeed = 0;

    private MovementFunc? currMovementFunction;
    private float movementFunctionTime;
    private Vector2 movementFunctionStartPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void Fall()
    {
        Debug.Log("Falling");
        
        movementStopped = true;
    }

    void OnJump()
    {
        Platform platform = GetOnPlatform();
        if (platform != null)
        {
            MovementFunc jf = platform.GetJumpFunction();
            StartMovementFunction(jf);
            character.Jump(jf.duration);
            Jumped.Invoke(platform);
        }
    }

    void FixedUpdate()
    {
        if (!movementStopped)
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
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Coll");

        StopMovementFunction();

        Platform platform = GetOnPlatform();
        if (platform == null)
        {
            movementStopped = true;
            Debug.Log("Fail");
        }
        else
        {
            if (Landed != null)
            {
                Landed.Invoke(platform);
            }
            Debug.Log("Land");
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
        Collider[] colliders = Physics.OverlapBox(center, Vector3.one*coll.radius, Quaternion.identity, gameConfig.groundLayerMask, QueryTriggerInteraction.Ignore);
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
}