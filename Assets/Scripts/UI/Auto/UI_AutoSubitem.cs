using UnityEngine;

public abstract class UI_AutoSubitem : UI_Subitem
{
    protected override void Init()
    {
        Managers.UI.Get<UI_AutoCanvas>().AddSubitem(this);
    }
}
