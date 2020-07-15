using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    private MovementFunc? movementPath;
    private float time;

    void Update()
    {
        if (movementPath.HasValue)
        {
            transform.localPosition = movementPath.Value.f(time);
            time += Time.deltaTime;
        }
    }

    public void Follow(MovementFunc path, float startTime)
    {
        movementPath = path;
        time = startTime;
    }
}