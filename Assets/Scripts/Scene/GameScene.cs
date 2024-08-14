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
        var player = GameObject.FindWithTag("Player");
        Managers.UI.Get<UI_ItemInventoryPopup>().Setup(player.GetComponent<ItemInventory>());
    }
}
