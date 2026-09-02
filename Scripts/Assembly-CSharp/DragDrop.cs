using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField]
	private Canvas canvas;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
