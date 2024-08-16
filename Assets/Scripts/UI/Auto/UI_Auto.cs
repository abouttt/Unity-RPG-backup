using UnityEngine;

public abstract class UI_Auto : UI_Base
{
    protected override void Init()
    {
        Managers.UI.Get<UI_AutoCanvas>().AddSubitem(this);
    }
}
