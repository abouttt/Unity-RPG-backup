using UnityEngine;

public class GameScene : BaseScene
{
    private void Start()
    {
        var uiPackage = Managers.Resource.Instantiate("GameUIPackage.prefab");
        uiPackage.transform.DetachChildren();
        Destroy(uiPackage);
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play(SoundType.BGM, SceneSettings.Instance[SceneAddress].BGM);
    }
}
