using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct MovementFunc
{
    public Func<float, Vector2> f { get; }
    public float duration { get; }

    public MovementFunc(Func<float, Vector2> f, float duration)
    {
        this.f = f;
        this.duration = duration;
    }

    public static MovementFunc combine(IList<MovementFunc> fs)
    {
        Func<float, Vector2> combinedFunc = t =>
        {
            float accumTime = 0;
            for (int i = 0; i < fs.Count; i++)
            {
                if (accumTime <= t && t < accumTime + fs[i].duration)
                {
                    return fs[i].f(t - accumTime);
                }
                accumTime += fs[i].duration;
            }

            return fs[fs.Count - 1].f(t - accumTime);
        };

        float totalTime = fs.Sum(mf => mf.duration);

        return new MovementFunc(combinedFunc, totalTime);
    }
}