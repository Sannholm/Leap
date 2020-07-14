using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    public IList<PlatformInfo> GeneratePlatforms(PlatformInfo startPlatform, System.Random rand)
    {
        int numPlatforms = 1000;
        float startAvgLength = 10, endAvgLength = 3;
        float lengthVariation = 2;
        float minSpacing = 2, maxSpacing = 20;
        float minOffsetY = -10, maxOffsetY = 10;
        float jumpHeightRatio = 0.5f;
        float jumpTimePerDistance = 0.1f;

        IList<PlatformInfo> platforms = new List<PlatformInfo>(numPlatforms + 1);
        platforms.Add(startPlatform);

        for (int i = 0; i < numPlatforms; i++)
        {
            float progress = (float)i / numPlatforms;

            Vector2 prevPlatformEndpoint = platforms[platforms.Count - 1].GetEndPoint();
            float spacing = Mathf.Lerp(minSpacing, maxSpacing, (float)rand.NextDouble());
            float x = prevPlatformEndpoint.x + spacing;

            float offsetY = Mathf.Lerp(minOffsetY, maxOffsetY, (float)rand.NextDouble());
            float y = prevPlatformEndpoint.y + offsetY;

            float avgLength = Mathf.Lerp(startAvgLength, endAvgLength, progress);
            float length = avgLength + (float)(rand.NextDouble() * 2 - 1) * lengthVariation;

            platforms.Add(PlatformInfo.FromLength(new Vector2(x, y), length));
        }

        for (int i = 0; i < platforms.Count - 1; i++)
        {
            Vector2 jumpStart = platforms[i].GetEndPoint() + Vector2.left*0.2f;
            Vector2 jumpEnd = platforms[i + 1].GetStartPoint() + Vector2.right*0.2f;

            float dx = jumpEnd.x - jumpStart.x;
            float dy = jumpEnd.y - jumpStart.y;
            float h = Mathf.Max(0, dy) + dx*jumpHeightRatio;
            float jt = Vector2.Distance(jumpStart, jumpEnd) * jumpTimePerDistance;

            float a = (dy - 2*(h + Mathf.Sqrt(h*(-dy + h))))/(jt*jt);
            float b = (2*(h + Mathf.Sqrt(h*(-dy + h))))/jt;
            float vx = dx/jt;
            platforms[i].jumpFunction = t => new Vector2(vx*t, (a*t+b)*t);
        }

        return platforms;
    }
}

public class PlatformInfo
{
    private Vector2 startPoint;
    private Vector2 endPoint;
    public Func<float, Vector2> jumpFunction;

    public static PlatformInfo FromEndpoints(Vector2 startPoint, Vector2 endPoint)
    {
        PlatformInfo platform = new PlatformInfo();
        platform.startPoint = startPoint;
        platform.endPoint = endPoint;
        return platform;
    }

    public static PlatformInfo FromLength(Vector2 start, float length)
    {
        PlatformInfo platform = new PlatformInfo();
        platform.startPoint = start;
        platform.endPoint = start + Vector2.right * length;
        return platform;
    }

    public Vector2 GetStartPoint()
    {
        return startPoint;
    }

    public Vector2 GetEndPoint()
    {
        return endPoint;
    }

    public float GetLength()
    {
        return Vector2.Distance(startPoint, endPoint);
    }

    public Func<float, Vector2> GetJumpFunction()
    {
        return jumpFunction;
    }
}