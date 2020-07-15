using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject visualPart;
    public Collider coll;

    private PlatformInfo platform;

    void Awake()
    {
        visualPart.SetActive(false);
    }

    public void OnPlayerLand() {
        visualPart.SetActive(true);
    }

    public bool isAbovePlatform(float y) {
        return coll.bounds.min.y <= y;
    }

    public void Construct(PlatformInfo platform)
    {
        this.platform = platform;
        transform.localPosition = platform.GetStartPoint();
        transform.localScale = new Vector3(platform.GetLength(), 1, 1);
    }

    public MovementFunc GetJumpFunction()
    {
        return platform.GetJumpFunction();
    }
}
