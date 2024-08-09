using UnityEngine;

public abstract class UI_Auto : UI_Base
{
    [SerializeField]
    protected GameObject _body;

    protected virtual void Start()
    {
        if (_body == null)
        {
            _body = transform.GetChild(0).gameObject;
        }

        _body.SetActive(false);
        transform.SetParent(Managers.UI.Get<UI_AutoCanvas>().transform);
    }
}
