using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_BaseSlot : UI_Base,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private enum Images
    {
        SlotImage,
        TempImage,
    }

    [field: SerializeField]
    public SlotType SlotType { get; private set; }

    [field: SerializeField]
    private bool _canDrag = true;

    private bool _isPointerDown;

    protected override void Init()
    {
        BindImage(typeof(Images));
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    protected void SetImage(Sprite image)
    {
        GetImage((int)Images.SlotImage).sprite = image;
        GetImage((int)Images.TempImage).sprite = image;
        GetImage((int)Images.SlotImage).gameObject.SetActive(image != null);
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canDrag)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (!GetImage((int)Images.SlotImage).isActiveAndEnabled)
        {
            eventData.pointerDrag = null;
            return;
        }

        GetImage((int)Images.TempImage).gameObject.SetActive(true);
        GetImage((int)Images.TempImage).transform.SetParent(Managers.UI.Get<UI_TopCanvas>().transform);
        GetImage((int)Images.TempImage).transform.SetAsLastSibling();
        GetImage((int)Images.SlotImage).color -= new Color(0f, 0f, 0f, 0.7f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        GetImage((int)Images.TempImage).rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
        GetImage((int)Images.TempImage).transform.SetParent(transform);
        GetImage((int)Images.TempImage).rectTransform.position = transform.position;
        GetImage((int)Images.SlotImage).color = Color.white;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!GetImage((int)Images.SlotImage).isActiveAndEnabled)
        {
            return;
        }

        _isPointerDown = true;
    }

    public abstract void OnPointerUp(PointerEventData eventData);

    protected bool CanPointerUp(PointerEventData eventData)
    {
        if (!_isPointerDown)
        {
            return false;
        }

        _isPointerDown = false;

        if (eventData.dragging)
        {
            return false;
        }

        return true;
    }
}
