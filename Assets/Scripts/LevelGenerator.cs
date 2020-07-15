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
        float minSpacing = 5, maxSpacing = 20;
        float minOffsetY = -10, maxOffsetY = 10;
        float jumpHeightRatio = 0.25f;
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
            platforms[i].jumpPoint = jumpStart;
            platforms[i + 1].landPoint = jumpEnd;

            float dx = jumpEnd.x - jumpStart.x;
            float dy = jumpEnd.y - jumpStart.y;
            float h = Mathf.Max(0, dy) + dx*jumpHeightRatio;
            float jt = Vector2.Distance(jumpStart, jumpEnd) * jumpTimePerDistance;

            float a = (dy - 2*(h + Mathf.Sqrt(h*(-dy + h))))/(jt*jt);
            float b = (2*(h + Mathf.Sqrt(h*(-dy + h))))/jt;
            float vx = dx/jt;
            platforms[i].jumpFunction = new MovementFunc(t => new Vector2(vx*t, (a*t+b)*t), jt);
        }

        return platforms;
    }

    public MovementFunc GenerateGuidePath(IList<PlatformInfo> platforms)
    {
        float runSpeed = 4.9f;

        IList<MovementFunc> segments = new List<MovementFunc>(platforms.Count * 2);

        foreach (var platform in platforms)
        {
            Vector2 runFrom = platform.GetLandPoint(), runTo = platform.GetJumpPoint();
            float runDuration = Vector2.Distance(runFrom, runTo) / runSpeed;
            segments.Add(new MovementFunc(t => Vector2.Lerp(runFrom, runTo, t / runDuration), runDuration));

            MovementFunc jf = platform.GetJumpFunction();
            segments.Add(new MovementFunc(t => runTo + jf.f(t), jf.duration));
        }

        return MovementFunc.combine(segments);
    }
}

public class PlatformInfo
{
    private Vector2 startPoint;
    private Vector2 endPoint;

    public Vector2 landPoint;
    public Vector2 jumpPoint;
    public MovementFunc jumpFunction;

    public static PlatformInfo FromEndpoints(Vector2 startPoint, Vector2 endPoint)
    {
        PlatformInfo p = new PlatformInfo();
        p.startPoint = startPoint;
        p.endPoint = endPoint;
        p.landPoint = p.startPoint;
        p.jumpPoint = p.endPoint;
        return p;
    }

    public static PlatformInfo FromLength(Vector2 start, float length)
    {
        PlatformInfo p = new PlatformInfo();
        p.startPoint = start;
        p.endPoint = start + Vector2.right * length;
        p.landPoint = p.startPoint;
        p.jumpPoint = p.endPoint;
        return p;
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

    public Vector2 GetLandPoint()
    {
        return landPoint;
    }

    public Vector2 GetJumpPoint()
    {
        return jumpPoint;
    }

    public MovementFunc GetJumpFunction()
    {
        return jumpFunction;
    }
}