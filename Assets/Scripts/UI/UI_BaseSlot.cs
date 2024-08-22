using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_BaseSlot : UI_Base,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private enum Images
    {
        SlotImage,
        TempImage,
    }

    public static bool IsDragging { get; private set; }

    [field: SerializeField]
    public SlotType SlotType { get; private set; }

    public object ObjectRef { get; private set; }
    public bool HasObject { get; private set; }

    [field: SerializeField]
    protected bool _canDrag = true;

    protected bool _isPointerDown = false;

    protected static bool s_isInitInventoryRef;
    protected static ItemInventory s_itemInventoryRef;
    protected static EquipmentInventory s_equipmentInventoryRef;

    protected override void Init()
    {
        BindImage(typeof(Images));
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!s_isInitInventoryRef)
        {
            s_itemInventoryRef = Managers.UI.Get<UI_ItemInventoryPopup>().ItemInventoryRef;
            s_equipmentInventoryRef = Managers.UI.Get<UI_EquipmentInventoryPopup>().EquipmentInventoryRef;
            s_isInitInventoryRef = true;
        }
    }

    protected void SetObject(object obj, Sprite image)
    {
        if (obj == null)
        {
            return;
        }

        ObjectRef = obj;
        HasObject = true;
        SetImage(image);
    }

    protected virtual void Clear()
    {
        ObjectRef = null;
        HasObject = false;
        SetImage(null);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if ((eventData.button != PointerEventData.InputButton.Left) || !HasObject || !_canDrag)
        {
            _isPointerDown = false;
            eventData.pointerDrag = null;
            return;
        }

        IsDragging = true;
        GetImage((int)Images.TempImage).gameObject.SetActive(true);
        GetImage((int)Images.TempImage).transform.SetParent(Managers.UI.Get<UI_TopCanvas>().transform);
        GetImage((int)Images.TempImage).transform.SetAsLastSibling();
        GetImage((int)Images.SlotImage).color -= new Color(0f, 0f, 0f, 0.7f);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        GetImage((int)Images.TempImage).rectTransform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
        GetImage((int)Images.TempImage).transform.SetParent(transform);
        GetImage((int)Images.TempImage).rectTransform.position = transform.position;
        GetImage((int)Images.SlotImage).color = Color.white;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if ((eventData.button != PointerEventData.InputButton.Left) || !HasObject)
        {
            return;
        }

        _isPointerDown = true;
    }

    public abstract void OnPointerEnter(PointerEventData eventData);
    public abstract void OnPointerExit(PointerEventData eventData);
    public abstract void OnPointerUp(PointerEventData eventData);

    protected bool CanPointerUp()
    {
        if (!_isPointerDown)
        {
            return false;
        }

        _isPointerDown = false;

        if (IsDragging)
        {
            return false;
        }

        return true;
    }

    private void SetImage(Sprite image)
    {
        GetImage((int)Images.SlotImage).sprite = image;
        GetImage((int)Images.TempImage).sprite = image;
        GetImage((int)Images.SlotImage).gameObject.SetActive(image != null);
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }
}
