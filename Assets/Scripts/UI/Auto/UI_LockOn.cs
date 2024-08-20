using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_LockOn : UI_Auto, ISystemConnectable<FieldOfView>
{
    private FieldOfView _lockOnFovRef;
    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        base.Init();
        _followTarget = GetComponent<UI_FollowWorldObject>();
    }

    public void ConnectSystem(FieldOfView lockOnFov)
    {
        _lockOnFovRef = lockOnFov;
        lockOnFov.TargetChanged += SetTarget;
    }

    public void DeconnectSystem()
    {
        if (_lockOnFovRef != null)
        {
            _lockOnFovRef.TargetChanged -= SetTarget;
            _lockOnFovRef = null;
        }
    }

    private void SetTarget(Transform target)
    {
        _followTarget.Target = target;
        gameObject.SetActive(target != null);
    }
}
