using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class UI_BaseSlot : UI_Base,
    IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    protected enum Images
    {
        SlotImage,
        TempImage,
    }

    [field: SerializeField]
    public SlotType SlotType { get; private set; }

    [field: SerializeField]
    protected bool _canDrag = true;

    private RectTransform _rt;
    private bool _isPointerDown;
    private bool _isMouseOver;

    protected override void Init()
    {
        BindImage(typeof(Images));
        _rt = GetComponent<RectTransform>();
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(_rt, Mouse.current.position.ReadValue()))
        {
            OnPointerEnter(null);
        }
    }

    private void OnDisable()
    {
        if (_isMouseOver)
        {
            OnPointerExit(null);
        }
    }

    protected void SetImage(Sprite image)
    {
        GetImage((int)Images.SlotImage).sprite = image;
        GetImage((int)Images.TempImage).sprite = image;
        GetImage((int)Images.SlotImage).gameObject.SetActive(image != null);
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
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
        GetImage((int)Images.SlotImage).color = new Color(1f, 1f, 1f, 0.3f);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        GetImage((int)Images.TempImage).rectTransform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        GetImage((int)Images.TempImage).gameObject.SetActive(false);
        GetImage((int)Images.TempImage).transform.SetParent(transform);
        GetImage((int)Images.TempImage).rectTransform.position = transform.position;
        GetImage((int)Images.SlotImage).color = Color.white;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
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
