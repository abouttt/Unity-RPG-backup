using UnityEngine;

public class GameScene : BaseScene
{
    [SerializeField]
    private Vector3 DefaultPosition;

    [SerializeField]
    private float DefalutRotationYaw;

    private GameObject _player;

    protected override void Init()
    {
        base.Init();
        InitPlayer();
        InstantiatePackage("GameUIPackage.prefab");
    }

    private void Start()
    {
        ConnectRef();
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play(SoundType.BGM, SceneSettings.Instance[SceneAddress].BGM);
    }

    private void OnDestroy()
    {
        DeconnectRef();
    }

    private void InitPlayer()
    {
        var playerPackagePrefab = Managers.Resource.Load<GameObject>("PlayerPackage.prefab");
        var playerPackage = Instantiate(playerPackagePrefab, DefaultPosition, Quaternion.Euler(0, DefalutRotationYaw, 0));
        _player = playerPackage.FindChildWithTag("Player");
        playerPackage.transform.DetachChildren();
        Destroy(playerPackage);
    }

    private void ConnectRef()
    {
        var itemInventory = _player.GetComponent<ItemInventory>();
        var equipmentInventory = _player.GetComponent<EquipmentInventory>();
        var interactor = _player.GetComponentInChildren<Interactor>();
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();
        var inventories = new Inventories(itemInventory, equipmentInventory);

        Item.SetInventoryRef(inventories);
        Managers.UI.Get<UI_ItemInventoryPopup>().ConnectSystem(itemInventory);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().ConnectSystem(equipmentInventory);
        Managers.UI.Get<UI_LootPopup>().ConnectSystem(itemInventory);
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_Interactor>().ConnectSystem(interactor);
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_LockOn>().ConnectSystem(lockOnFov);
        Managers.UI.Get<UI_BackgroundCanvas>().ConnectSystem(new Inventories(itemInventory, equipmentInventory));
    }

    private void DeconnectRef()
    {
        if (Managers.Instance == null)
        {
            return;
        }

        if (Managers.UI.Count == 0)
        {
            return;
        }

        Managers.UI.Get<UI_ItemInventoryPopup>().DeconnectSystem();
        Managers.UI.Get<UI_EquipmentInventoryPopup>().DeconnectSystem();
        Managers.UI.Get<UI_LootPopup>().DeconnectSystem();
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_Interactor>().DeconnectSystem();
        Managers.UI.Get<UI_AutoCanvas>().GetSubitem<UI_LockOn>().DeconnectSystem();
        Managers.UI.Get<UI_BackgroundCanvas>().DeconnectSystem();
    }
}
