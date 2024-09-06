using UnityEngine;

public abstract class UI_TopSubitem : UI_Subitem
{
    protected override void Init()
    {
        Managers.UI.Get<UI_TopCanvas>().AddSubitem(this);
    }
}
