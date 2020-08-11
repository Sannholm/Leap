using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private Collider coll;
    [SerializeField]
    private UnityEvent Activated;
    [SerializeField]
    private UnityEvent Hid;

    private PlatformInfo platform;
    private bool alwaysOn;
    private bool active = true;

    void OnDrawGizmos()
    {
        if (platform != null)
        {
            Vector3 start = gameObject.transform.parent.TransformPoint(platform.GetStartPoint());
            Vector3 end = gameObject.transform.parent.TransformPoint(platform.GetEndPoint());
            Vector3 land = gameObject.transform.parent.TransformPoint(platform.GetLandPoint());
            Vector3 jump = gameObject.transform.parent.TransformPoint(platform.GetJumpPoint());
            Gizmos.DrawWireSphere(start, 0.1f);
            Gizmos.DrawWireSphere(end, 0.1f);
            Gizmos.DrawLine(land, land + Vector3.up * 2);
            Gizmos.DrawLine(jump, jump + Vector3.up * 2);
        }
    }

    public void Construct(PlatformInfo platform, bool alwaysOn)
    {
        this.platform = platform;
        this.alwaysOn = alwaysOn;

        transform.localPosition = platform.GetStartPoint();
        transform.localScale = new Vector3(platform.GetLength(), 1, 1);
        float angle = Vector3.SignedAngle(Vector3.right, platform.GetEndPoint() - platform.GetStartPoint(), Vector3.back);
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.back);
    }

    void Start()
    {
        if (!alwaysOn)
        {
            Hide();
        }
    }

    private void Hide()
    {
        active = false;
        if (Hid != null)
        {
            Hid.Invoke();
        }
    }

    private void Activate()
    {
        active = true;
        if (Activated != null)
        {
            Activated.Invoke();
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (!active)
        {
            Activate();
        }
    }

    public MovementFunc GetJumpFunction()
    {
        return platform.GetJumpFunction();
    }

    public Vector2 GetJumpPoint()
    {
        return platform.GetJumpPoint();
    }
}
