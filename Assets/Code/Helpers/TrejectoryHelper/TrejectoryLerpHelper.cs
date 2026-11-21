using System;
using System.Collections;
using UnityEngine;

public static class TrejectoryLerpHelper 
{
    
    public static float Evaluate10Parabola(float t)
    {
        return -Mathf.Pow((2 * t - 1),2) + 1;
    }

    /// <summary>
    /// interpolates between start and end using t.
    /// offset by a trejectory parabola in the uppAxis direction. 
    /// offsetDiffMult is how much of the distance between start and end in the directon of uppAxis is added to the parabola height
    /// offsetAdditionalHeight is added on top of that as a flat heigh value
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <param name="uppAxis"></param>
    /// <param name="offsetDiffMult"></param>
    /// <param name="offsetAdditionalHeight"></param>
    /// <returns></returns>

    public static Vector3 LerpParabolaTrejectory(Vector3 start, Vector3 end, float t, Vector3 uppAxis, float offsetDiffMult = 0.5f, float offsetAdditionalHeight = 1f)
    {
        var lerp = Vector3.LerpUnclamped(start, end, t);

        float parabola = Evaluate10Parabola(t);

        float diff = Vector3.Distance(Vector3.Project(start, uppAxis), Vector3.Project(end, uppAxis));

        float offset = parabola * (diff * offsetDiffMult + offsetAdditionalHeight);

        return lerp += uppAxis * offset;
    }

    public static Vector3 LerpCustomTrejectory(Vector3 start, Vector3 end, float t, Vector3 uppAxis, AnimationCurve customCurve, float offsetDiffMult = 0.5f, float offsetAdditionalHeight = 1f)
    {
        var lerp = Vector3.LerpUnclamped(start, end, t);

        float parabola = customCurve.Evaluate(t);

        float diff = Vector3.Distance(Vector3.Project(start, uppAxis), Vector3.Project(end, uppAxis));

        float offset = parabola * (diff * offsetDiffMult + offsetAdditionalHeight);

        return lerp += uppAxis * offset;
    }

}
