using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GlobalCanvas : UI_Base
{
    enum Images
    {
        FadeImage,
    }

    protected override void Init()
    {
        BindImage(typeof(Images));
        ChangeAlpha(GetImage((int)Images.FadeImage), 0f);
        Managers.UI.Register(this);
    }

    public void FadeIn(float duration)
    {
        var fadeImage = GetImage((int)Images.FadeImage);
        ChangeAlpha(fadeImage, 1f);
        fadeImage.DOFade(0f, duration);
    }

    public void FadeOut(float duration)
    {
        var fadeImage = GetImage((int)Images.FadeImage);
        ChangeAlpha(fadeImage, 0f);
        fadeImage.DOFade(1f, duration);
    }

    private void ChangeAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
