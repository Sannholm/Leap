using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private RenderTexture lightMaskRT;
    [SerializeField]
    private GameObject followedPlayer;
    [SerializeField]
    private GameObject guidePlayer;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float minPosX = 0;
    [SerializeField]
    private float smoothTime;
    
    private Vector3 followVelocity;

    void Start()
    {
        transform.position = getTargetPos();
    }

    void Update()
    {
        if (lightMaskRT.width != mainCamera.pixelWidth || lightMaskRT.height != mainCamera.pixelHeight)
        {
            if (lightMaskRT.IsCreated())
            {
                lightMaskRT.Release();
            }
            lightMaskRT.width = mainCamera.pixelWidth;
            lightMaskRT.height = mainCamera.pixelHeight;
        }
    }

    private Vector3 getTargetPos()
    {
        Vector3 targetPos = (followedPlayer.transform.position + guidePlayer.transform.position) * 0.5f + offset;
        targetPos = new Vector3(Mathf.Max(minPosX, targetPos.x), targetPos.y, targetPos.z);
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
