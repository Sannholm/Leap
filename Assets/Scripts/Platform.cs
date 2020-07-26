using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Collider coll;
    public GameObject visualRoot;
    public GameObject lightBeam;
    public AudioSource turnOnAudioSource;

    private PlatformInfo platform;
    private bool alwaysOn;

    private float activationTime = -1;
    private float activationDuration;
    private Vector3[] lightBeamVertices = new Vector3[3];

    void OnDrawGizmos()
    {
        if (platform != null)
        {
            Vector3 start = gameObject.transform.parent.TransformPoint(platform.GetStartPoint());
            Vector3 end = gameObject.transform.parent.TransformPoint(platform.GetEndPoint());
            Vector3 land = gameObject.transform.parent.TransformPoint(platform.GetLandPoint());
            Vector3 jump = gameObject.transform.parent.TransformPoint(platform.GetJumpPoint());
            Gizmos.DrawLine(land, land + Vector3.up * 2);
            Gizmos.DrawLine(jump, jump + Vector3.up * 2);
            Gizmos.DrawWireSphere(start, 0.1f);
            Gizmos.DrawWireSphere(end, 0.1f);
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

        float avgActivationDuration = 0.025f;
        float activationDurationVariation = 0.005f;
        activationDuration = avgActivationDuration + UnityEngine.Random.Range(-activationDurationVariation, activationDurationVariation);

        SetupLightBeam();

        if (alwaysOn)
        {
            activationTime = activationDuration;
        }
        else
        {
            visualRoot.SetActive(false);
        }
    }

    private void SetupLightBeam()
    {
        float minOffset = -2, maxOffset = 2;
        float topOffset = UnityEngine.Random.Range(minOffset, maxOffset);
        lightBeamVertices[2] = new Vector3(0.5f + topOffset, 18f * 3, 0);

        Mesh mesh = lightBeam.GetComponent<MeshFilter>().mesh = new Mesh();

        AnimateLightBeam(activationDuration);
        mesh.triangles = new int[] { 0, 2, 1 };
    }

    void Update()
    {
        if (activationTime != -1)
        {
            AnimateLightBeam(activationTime);
            activationTime += Time.deltaTime;
        }
    }

    private void AnimateLightBeam(float time)
    {
        float activationProgress = Mathf.Clamp01(time / activationDuration);

        lightBeamVertices[0] = new Vector3(Mathf.Lerp(0.5f, 0, activationProgress), 0, 0);
        lightBeamVertices[1] = new Vector3(Mathf.Lerp(0.5f, 1, activationProgress), 0, 0);
        lightBeam.GetComponent<MeshFilter>().mesh.vertices = lightBeamVertices;
    }

    public void Activate()
    {
        if (!alwaysOn)
        {
            visualRoot.SetActive(true);
            activationTime = 0;
            turnOnAudioSource.Play();
        }
    }

    public bool isAbovePlatform(float y)
    {
        return coll.bounds.min.y <= y;
    }

    public MovementFunc GetJumpFunction()
    {
        return platform.GetJumpFunction();
    }
}
