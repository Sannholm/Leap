using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class LevelGeneratorParams
{
    public float levelDuration;
    public float runSpeed;

    // Platform
    public float startAvgLength, endAvgLength;
    public float lengthVariation;
    public float minSpacing, maxSpacing;
    public float minOffsetY, maxOffsetY;
    public float minTiltAngle, maxTiltAngle;

    // Jump Points
    public float minEarlyLandMargin, maxEarlyLandMargin;
    public float minLateJumpMargin, maxLateJumpMargin;

    // Jump Arc
    public float jumpHeightRatio;
    public float jumpTimePerDistance;
}

public class LevelGenerator
{
    public IList<PlatformInfo> GeneratePlatforms(System.Random rand, PlatformInfo startPlatform, LevelGeneratorParams p)
    {
        Assert.IsTrue(p.maxEarlyLandMargin + p.maxLateJumpMargin <= 1, "Early landing and late jump margin overlapping");

        IList<PlatformInfo> platforms = new List<PlatformInfo>();

        ChooseActionPoints(rand, p, startPlatform);
        platforms.Add(startPlatform);
        
        float accumLevelTime = Vector2.Distance(startPlatform.GetStartPoint(), startPlatform.GetJumpPoint()) / p.runSpeed;
        while (accumLevelTime < p.levelDuration)
        {
            float progress = accumLevelTime / p.levelDuration;

            PlatformInfo plat = PlaceNextPlatform(rand, p, platforms[platforms.Count - 1], progress);
            ChooseActionPoints(rand, p, plat);
            platforms.Add(plat);

            MovementFunc jf = SetupJumpFunction(p, platforms, platforms[platforms.Count - 2], platforms[platforms.Count - 1]);

            accumLevelTime += Vector2.Distance(plat.GetLandPoint(), plat.GetJumpPoint()) / p.runSpeed + jf.duration;
        }

        platforms[platforms.Count - 1].jumpFunction = new MovementFunc(_ => Vector2.zero, 0);

        return platforms;
    }

    private PlatformInfo PlaceNextPlatform(System.Random rand, LevelGeneratorParams p, PlatformInfo prevPlatform, float progress)
    {
        Vector2 prevPlatformEndpoint = prevPlatform.GetEndPoint();
        float spacing = Mathf.Lerp(p.minSpacing, p.maxSpacing, (float)rand.NextDouble());
        float x = prevPlatformEndpoint.x + spacing;

        float offsetY = Mathf.Lerp(p.minOffsetY, p.maxOffsetY, (float)rand.NextDouble());
        float y = prevPlatformEndpoint.y + offsetY;
        float tiltAngle = Mathf.Lerp(p.minTiltAngle, p.maxTiltAngle, (float)rand.NextDouble());

        float avgLength = Mathf.Lerp(p.startAvgLength, p.endAvgLength, progress);
        float length = avgLength + Mathf.Lerp(-p.lengthVariation, p.lengthVariation, (float)rand.NextDouble());

        Vector2 start = new Vector2(x, y);
        Vector2 end = start + (Vector2)(Quaternion.Euler(0, 0, tiltAngle) * new Vector2(length, 0));
        return PlatformInfo.FromEndpoints(start, end);
    }

    private void ChooseActionPoints(System.Random rand, LevelGeneratorParams p, PlatformInfo plat)
    {
        float earlyLandMargin = Mathf.Lerp(p.minEarlyLandMargin, p.maxEarlyLandMargin, (float)rand.NextDouble());
        float lateJumpMargin = Mathf.Lerp(p.minLateJumpMargin, p.maxLateJumpMargin, (float)rand.NextDouble());

        plat.landPoint = Vector2.Lerp(plat.GetStartPoint(), plat.GetEndPoint(), earlyLandMargin);
        plat.jumpPoint = Vector2.Lerp(plat.GetStartPoint(), plat.GetEndPoint(), 1 - lateJumpMargin);
    }

    private MovementFunc GenerateJumpFunction(LevelGeneratorParams p, Vector2 jumpStart, Vector2 jumpEnd)
    {
        float dx = jumpEnd.x - jumpStart.x;
        float dy = jumpEnd.y - jumpStart.y;
        float h = Mathf.Max(0, dy) + dx * p.jumpHeightRatio;
        float jt = Vector2.Distance(jumpStart, jumpEnd) * p.jumpTimePerDistance;

        float a = (dy - 2 * (h + Mathf.Sqrt(h * (-dy + h)))) / (jt * jt);
        float b = (2 * (h + Mathf.Sqrt(h * (-dy + h)))) / jt;
        float vx = dx / jt;
        return new MovementFunc(t => new Vector2(vx * t, (a * t + b) * t), jt);
    }

    private MovementFunc SetupJumpFunction(LevelGeneratorParams p, IList<PlatformInfo> platforms, PlatformInfo from, PlatformInfo to)
    {
        Vector2 jumpStart = from.jumpPoint;
        Vector2 jumpEnd = to.landPoint;
        MovementFunc jf = GenerateJumpFunction(p, jumpStart, jumpEnd);
        from.jumpFunction = jf;
        return jf;
    }

    public MovementFunc GenerateGuidePath(IList<PlatformInfo> platforms, float runSpeed)
    {
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