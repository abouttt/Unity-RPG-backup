using UnityEngine;

public abstract class UI_Auto : UI_Base
{
    [SerializeField]
    protected GameObject _body;

    protected override void Init()
    {
        if (_body == null)
        {
            _body = transform.GetChild(0).gameObject;
        }
    }

    protected virtual void Start()
    {
        _body.SetActive(false);
        transform.SetParent(Managers.UI.Get<UI_AutoCanvas>().transform);
    }
}
