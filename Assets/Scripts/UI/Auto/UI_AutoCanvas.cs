using UnityEngine;

public class UI_AutoCanvas : UI_Canvas<UI_AutoSubitem>
{
    protected override void Init()
    {
        Managers.UI.Register(this);
    }
}
