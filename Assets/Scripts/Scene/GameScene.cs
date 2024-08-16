using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        var uiPackage = Managers.Resource.Instantiate("GameUIPackage.prefab");
        uiPackage.transform.DetachChildren();
        Destroy(uiPackage);
    }

    private void Start()
    {
        ConnectUI();
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play(SoundType.BGM, SceneSettings.Instance[SceneAddress].BGM);
    }

    private void ConnectUI()
    {
        Managers.UI.Get<UI_ItemInventoryPopup>().ConnectSystem(Player.ItemInventory);
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_Interactor>().ConnectSystem(Player.Interactor);
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_LockOn>().ConnectSystem(Player.LockOnFov);
    }
}
