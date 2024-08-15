using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_LockOn : UI_Auto, ISystemConnectable<FieldOfView>
{
    public FieldOfView SystemRef { get; private set; }

    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        base.Init();
        _followTarget = GetComponent<UI_FollowWorldObject>();
        Managers.UI.Get<UI_AutoCanvas>().AddUniqueAutoUI(this);
    }

    public void ConnectSystem(FieldOfView lockOnFov)
    {
        SystemRef = lockOnFov;
        lockOnFov.TargetChanged += SetTarget;
    }

    public void DeconnectSystem()
    {
        if (SystemRef == null)
        {
            return;
        }

        SystemRef.TargetChanged -= SetTarget;
        SystemRef = null;
    }

    private void SetTarget(Transform target)
    {
        _body.SetActive(target != null);
        _followTarget.Target = target;
    }

    private void OnDestroy()
    {
        DeconnectSystem();
    }
}
