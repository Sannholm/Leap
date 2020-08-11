using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    [SerializeField]
    private GameConfig gameConfig;
    [SerializeField]
    private float onGroundMargin = 0.1f;
    [SerializeField]
    private RunningCharacterController character;

    private FollowPath followPath;

    private Platform latestPlatform;

    void OnDrawGizmos()
    {
        float radius = 0.5f;
        Vector3 center = transform.position + Vector3.down * (-radius + onGroundMargin);
        Gizmos.DrawWireCube(center, Vector3.one*radius*2);
    }

    void Awake()
    {
        followPath = GetComponent<FollowPath>();
    }

    void Update()
    {
        Platform currentPlatform = GetOnPlatform();
        bool jumped = currentPlatform == null && latestPlatform != null;
        if (jumped)
        {
            character.Jump(latestPlatform.GetJumpFunction().duration);
        }

        latestPlatform = currentPlatform;
    }

    private Platform GetOnPlatform()
    {
        float radius = 0.5f;
        Vector3 center = transform.position + Vector3.down * (-radius + onGroundMargin);
        Collider[] colliders = Physics.OverlapBox(center, Vector3.one*radius, Quaternion.identity, gameConfig.groundLayerMask, QueryTriggerInteraction.Ignore);
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

    public void Follow(MovementFunc path, float startTime)
    {
        followPath.Follow(path, startTime);
    }
}
