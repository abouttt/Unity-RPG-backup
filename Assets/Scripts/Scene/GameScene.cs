using UnityEngine;

public class GameScene : BaseScene
{
    private GameObject _player;
    private bool _isUIConnected;

    protected override void Init()
    {
        base.Init();

        InstantiatePackage("PlayerPackage.prefab");
        _player = GameObject.FindWithTag("Player");

        InstantiatePackage("GameUIPackage.prefab");
    }

    private void Start()
    {
        ConnectUI();
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play(SoundType.BGM, SceneSettings.Instance[SceneAddress].BGM);
    }

    private void OnDestroy()
    {
        DeconnectUI();
    }

    private void ConnectUI()
    {
        var itemInventory = _player.GetComponent<ItemInventory>();
        var interactor = _player.GetComponentInChildren<Interactor>();
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();

        Managers.UI.Get<UI_ItemInventoryPopup>().ConnectSystem(itemInventory);
        Managers.UI.Get<UI_LootPopup>().ConnectSystem(itemInventory);
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_Interactor>().ConnectSystem(interactor);
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_LockOn>().ConnectSystem(lockOnFov);
        Managers.UI.Get<UI_BackgroundCanvas>().ConnectSystem(new Inventories(itemInventory));

        _isUIConnected = true;
    }

    private void DeconnectUI()
    {
        if (!_isUIConnected)
        {
            return;
        }

        if (Managers.Instance == null)
        {
            return;
        }

        Managers.UI.Get<UI_ItemInventoryPopup>().DeconnectSystem();
        Managers.UI.Get<UI_LootPopup>().DeconnectSystem();
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_Interactor>().DeconnectSystem();
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_LockOn>().DeconnectSystem();
        Managers.UI.Get<UI_BackgroundCanvas>().DeconnectSystem();
    }
}
