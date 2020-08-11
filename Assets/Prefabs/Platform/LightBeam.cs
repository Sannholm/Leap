using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    private MeshFilter meshFilter;

    private float activationDuration;
    private float activationTime = -1;
    private Vector3[] lightBeamVertices = new Vector3[3];

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        float avgActivationDuration = 0.025f;
        float activationDurationVariation = 0.005f;
        activationDuration = avgActivationDuration + UnityEngine.Random.Range(-activationDurationVariation, activationDurationVariation);

        SetupGeometry();
    }

    private void SetupGeometry()
    {
        float minOffset = -2, maxOffset = 2;
        float topOffset = UnityEngine.Random.Range(minOffset, maxOffset);
        float topHeight = 18f * 3;
        lightBeamVertices[2] = new Vector3(0.5f + topOffset, topHeight, 0);

        meshFilter.mesh = new Mesh();
        AnimateGeometry(1);
        meshFilter.mesh.triangles = new int[] { 0, 2, 1 };
    }

    void Update()
    {
        if (activationTime >= 0)
        {
            AnimateGeometry(Mathf.Clamp01(activationTime / activationDuration));

            if (activationTime > activationDuration)
            {
                activationTime = -1;
            }
            else
            {
                activationTime += Time.deltaTime;
            }    
        }
    }

    private void AnimateGeometry(float activationProgress)
    {
        lightBeamVertices[0] = new Vector3(Mathf.Lerp(0.5f, 0, activationProgress), 0, 0);
        lightBeamVertices[1] = new Vector3(Mathf.Lerp(0.5f, 1, activationProgress), 0, 0);
        meshFilter.mesh.vertices = lightBeamVertices;
    }

    public void Hide()
    {
        activationTime = -1;
        AnimateGeometry(0);
    }

    public void Activate()
    {
        activationTime = 0;
    }
}