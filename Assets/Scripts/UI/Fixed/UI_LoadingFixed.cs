using UnityEngine;

public class UI_LoadingFixed : UI_Base
{
    enum Images
    {
        BG,
        Bar,
    }

    protected override void Init()
    {
        BindImage(typeof(Images));

        var bg = GetImage((int)Images.BG);
        bg.sprite = SceneSettings.Instance[Managers.Scene.NextSceneAddress].LoadingImage;
        bg.color = bg.sprite != null ? Color.white : Color.black;

        GetImage((int)Images.Bar).fillAmount = 0f;
    }

    private void Update()
    {
        GetImage((int)Images.Bar).fillAmount = Managers.Scene.LoadingProgress;
    }
}
