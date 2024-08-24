using UnityEngine;
using DG.Tweening;

public class UI_LoadingFixed : UI_Base
{
    enum Images
    {
        BG,
        Bar,
        FadeImage,
    }

    protected override void Init()
    {
        BindImage(typeof(Images));

        var bg = GetImage((int)Images.BG);
        bg.sprite = SceneSettings.Instance[Managers.Scene.NextSceneAddress].LoadingImage;
        bg.color = bg.sprite != null ? Color.white : Color.black;

        GetImage((int)Images.Bar).fillAmount = 0f;

        var loadingScene = Managers.Scene.CurrentScene as LoadingScene;
        Managers.Scene.LoadCompleteReady += () => GetImage((int)Images.FadeImage).DOFade(1f, loadingScene.LoadNextSceneDuration);
    }

    private void Update()
    {
        GetImage((int)Images.Bar).fillAmount = Managers.Scene.LoadingProgress;
    }
}
