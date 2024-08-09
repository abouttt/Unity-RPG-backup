using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_LockOn : UI_Auto
{
    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        _followTarget = GetComponent<UI_FollowWorldObject>();
    }

    protected override void Start()
    {
        base.Start();
        Camera.main.GetComponent<FieldOfView>().TargetChanged += target =>
        {
            _body.SetActive(target != null);
            _followTarget.Target = target;
        };
    }
}
