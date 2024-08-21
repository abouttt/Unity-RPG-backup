using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_BaseSlot, IDropHandler
{
    enum Imagess
    {
        BG = 2,
    }

    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [SerializeField]
    private Sprite _backgroundImage;

    protected override void Init()
    {
        base.Init();
        BindImage(typeof(Imagess));
        GetImage((int)Imagess.BG).sprite = _backgroundImage;
        Refresh(null);
    }

    public void Refresh(EquipmentItem equipmentItem)
    {
        if (equipmentItem != null)
        {
            SetObject(equipmentItem, equipmentItem.Data.ItemImage);
        }
        else
        {
            Clear();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // TODO : Tooltip On
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // TODO : Tooltip Off
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        if (ObjectRef is IUsableItem usable)
        {
            usable.Use(s_itemInventoryRef, s_equipmentInventoryRef);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherItemSlot))
        {
            if (otherItemSlot.ObjectRef is not EquipmentItem otherEquipmentItem)
            {
                return;
            }

            if (EquipmentType != otherEquipmentItem.EquipmentData.EquipmentType)
            {
                return;
            }

            if (otherEquipmentItem is not IUsableItem otherUsable)
            {
                return;
            }

            otherUsable.Use(s_itemInventoryRef, s_equipmentInventoryRef);
        }
    }
}
