using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float jumpSpeed = 10f;
    public float onGroundMargin = 0.2f;
    public LayerMask groundLayerMask;

    private Rigidbody rb;
    private CapsuleCollider coll;

    private float currSpeed;

    private ISet<Platform> visitedPlatforms = new HashSet<Platform>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        
        currSpeed = movementSpeed;
    }

    void OnJump()
    {
        Platform platform = GetOnPlatform();
        if (platform != null)
        {
            rb.AddForce(Vector3.up * platform.GetJumpVelocity().y, ForceMode.VelocityChange);
            currSpeed = platform.GetJumpVelocity().x;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(currSpeed, rb.velocity.y, 0);
        //rb.MovePosition(rb.position + Vector3.right * currSpeed * Time.fixedDeltaTime);
    }

    private Platform GetOnPlatform()
    {
        Vector3 center = coll.bounds.center + Vector3.down * (coll.height/2 - coll.radius + onGroundMargin);
        Collider[] colliders = Physics.OverlapSphere(center, coll.radius, groundLayerMask, QueryTriggerInteraction.Ignore);
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

    void OnTriggerStay(Collider collider) {
        Platform platform = collider.gameObject.GetComponentInParent<Platform>();
        if (platform != null && rb.velocity.y <= 0 && platform.isAbovePlatform(coll.bounds.min.y) && !visitedPlatforms.Contains(platform)) {
            visitedPlatforms.Add(platform);
            platform.OnPlayerLand();
            currSpeed = movementSpeed;
        }
    }
}
