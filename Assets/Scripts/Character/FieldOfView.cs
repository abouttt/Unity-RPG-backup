using System;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public event Action<Transform> TargetChanged;

    public Transform Target
    {
        get => _target;
        set
        {
            _target = value;
            TargetChanged?.Invoke(_target);
        }
    }

    public bool HasTarget => _target != null;

    [field: Header("Find")]
    [field: SerializeField]
    public float ViewRadius { get; set; }

    [field: SerializeField, Range(0f, 360f)]
    public float ViewAngle { get; set; }

    [field: SerializeField]
    public LayerMask TargetLayers { get; set; }

    [field: SerializeField]
    public LayerMask ObstacleLayers { get; set; }

    [field: Header("Tracking")]
    [field: SerializeField]
    public bool TrackingByDistance { get; set; } = true;

    [field: SerializeField]
    public bool TrackingByObstacle { get; set; } = true;

    [field: SerializeField]
    public bool TrackingByHorizontalAngle { get; set; } = true;

    [field: SerializeField]
    public bool TrackingByVerticalAngle { get; set; } = true;

    [field: SerializeField]
    public float TrackingByHorizontalAngleValue { get; set; }

    [field: SerializeField]
    public float TrackingByVerticalAngleValue { get; set; }

    private Transform _target;

    private void LateUpdate()
    {
        TrackingTarget();
    }

    public void FindTarget()
    {
        Transform finalTarget = null;
        float shortestAngle = Mathf.Infinity;

        var targets = Physics.OverlapSphere(transform.position, ViewRadius, TargetLayers);
        foreach (var target in targets)
        {
            var directionToTarget = (target.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle > ViewAngle * 0.5f)
            {
                continue;
            }

            if (angle >= shortestAngle)
            {
                continue;
            }

            if (Physics.Linecast(transform.position, target.transform.position, ObstacleLayers))
            {
                continue;
            }

            finalTarget = target.transform;
            shortestAngle = angle;
        }

        Target = finalTarget;
    }

    private void TrackingTarget()
    {
        if (!HasTarget)
        {
            return;
        }

        if (TrackingByDistance && Vector3.Distance(transform.position, _target.position) > ViewRadius)
        {
            Target = null;
            return;
        }

        if (TrackingByObstacle && Physics.Linecast(transform.position, _target.position, ObstacleLayers))
        {
            Target = null;
            return;
        }

        if (TrackingByHorizontalAngle &&
            !IsAngleInRange(transform.eulerAngles.y, -TrackingByHorizontalAngleValue, TrackingByHorizontalAngleValue))
        {
            Target = null;
            return;
        }

        if (TrackingByVerticalAngle &&
            !IsAngleInRange(transform.eulerAngles.x, -TrackingByVerticalAngleValue, TrackingByVerticalAngleValue))
        {
            Target = null;
        }
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
