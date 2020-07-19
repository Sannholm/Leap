using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Collider coll;
    public GameObject visualPart;

    private PlatformInfo platform;

    void Awake()
    {
        //visualPart.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (platform != null)
        {
            Vector3 land = gameObject.transform.parent.TransformPoint(platform.GetLandPoint());
            Vector3 jump = gameObject.transform.parent.TransformPoint(platform.GetJumpPoint());
            Gizmos.DrawLine(land, land + Vector3.up * 2);
            Gizmos.DrawLine(jump, jump + Vector3.up * 2);
        }
    }

    public void Construct(PlatformInfo platform)
    {
        this.platform = platform;
        transform.localPosition = platform.GetStartPoint();
        transform.localScale = new Vector3(platform.GetLength(), 1, 1);
    }

    public void OnPlayerLand() {
        visualPart.SetActive(true);
    }

    public bool isAbovePlatform(float y) {
        return coll.bounds.min.y <= y;
    }

    public MovementFunc GetJumpFunction()
    {
        return platform.GetJumpFunction();
    }
}
