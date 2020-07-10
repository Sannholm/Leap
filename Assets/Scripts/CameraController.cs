using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject followedPlayer;
    public Vector3 offset;
    public float smoothTime;
    
    private Vector3 followVelocity;

    void Start()
    {
        transform.position = getTargetPos();
    }

    private Vector3 getTargetPos()
    {
        Vector3 targetPos = followedPlayer.transform.position + offset;
        targetPos = new Vector3(Mathf.Max(0, targetPos.x), targetPos.y, targetPos.z);
        return targetPos;
    }

    void LateUpdate()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = getTargetPos();
        Vector3 p = Vector3.SmoothDamp(currentPos, targetPos, ref followVelocity, smoothTime);
        
        transform.position = p;
    }
}
