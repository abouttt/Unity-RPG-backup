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

    public void Fade(float start, float end, float duration)
    {
        var fadeImage = GetImage((int)Images.FadeImage);
        ChangeAlpha(fadeImage, start);
        fadeImage.DOFade(end, duration);
    }

    private void ChangeAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
