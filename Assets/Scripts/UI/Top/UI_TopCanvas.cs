using UnityEngine;
using DG.Tweening;

public class UI_TopCanvas : UI_Base
{
    enum Images
    {
        FadeImage,
    }

    protected override void Init()
    {
        BindImage(typeof(Images));
        Managers.UI.Register(this);
    }

    public void FadeIn(float duration)
    {
        GetImage((int)Images.FadeImage).DOFade(0f, duration);
    }
}
