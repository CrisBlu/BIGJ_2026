using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RailSegment : MonoBehaviour
{
    public static readonly List<RailSegment> All = new();

    private SplineContainer splineContainer;

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    public Vector3 GetPosition(float t)
    {
        Unity.Mathematics.float3 local = splineContainer.Spline.EvaluatePosition(t);
        return transform.TransformPoint(new Vector3(local.x, local.y, local.z));
    }

    public Vector3 GetDirection(float t)
    {
        Unity.Mathematics.float3 localTangent = splineContainer.Spline.EvaluateTangent(t);
        Vector3 tangent = new Vector3(localTangent.x, localTangent.y, localTangent.z);
        return transform.TransformDirection(tangent).normalized;
    }

    public float Length => splineContainer.Spline.GetLength();

    public float GetClosestT(Vector3 worldPos)
    {
        SplineUtility.GetNearestPoint(splineContainer.Spline, transform.InverseTransformPoint(worldPos), out _, out float t);
        return t;
    }
}
