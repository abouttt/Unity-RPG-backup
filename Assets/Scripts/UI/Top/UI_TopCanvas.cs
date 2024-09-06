using UnityEngine;

public class UI_TopCanvas : UI_Canvas<UI_TopSubitem>
{
    protected override void Init()
    {
        Managers.UI.Register(this);
    }
}
