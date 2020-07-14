using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject visualPart;
    public Collider coll;

    private PlatformInfo platform;

    void Start()
    {
        //visualPart.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnPlayerLand() {
        visualPart.SetActive(true);
        //coll.enabled = true;
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

    public Func<float, Vector2> GetJumpFunction()
    {
        return platform.GetJumpFunction();
    }
}
