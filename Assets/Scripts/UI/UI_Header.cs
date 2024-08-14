using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Header : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [field: SerializeField]
    public bool CanDrag { get; set; } = true;

    [SerializeField]
    private RectTransform _target;  // ����� ��� �����̸� ���� UI�鵵 �����̰� �ϱ����� �߽� Ÿ��.

    private Vector2 _beginPoint;    // ����� ���� ���� ��ġ.
    private Vector2 _moveBegin;     // ��� ������ ���� ��ġ.

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag)
        {
            eventData.pointerDrag = null;
            return;
        }

        _beginPoint = _target.anchoredPosition;
        _moveBegin = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _target.anchoredPosition = _beginPoint + (eventData.position - _moveBegin);
    }
}
