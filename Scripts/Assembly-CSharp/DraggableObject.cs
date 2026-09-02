using UnityEngine;

public class DraggableObject : MonoBehaviour
{
	public float panSpeed = 15f;

	[SerializeField]
	private Canvas canvasObject;

	private RectTransform canvasRect;

	private Vector3 pan;

	[SerializeField]
	private bool pressing;

	[SerializeField]
	private GameObject ObjectMustHide;

	[SerializeField]
	private bool isHorizontalOnly;

	[Header("Scroll Settings")]
	public bool infiniteScroll;

	public Vector2 minLimit = new Vector2(-500f, -500f);

	public Vector2 maxLimit = new Vector2(500f, 500f);

	private void Start()
	{
		canvasRect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (((ObjectMustHide == null || !ObjectMustHide.activeSelf) && canvasObject != null && canvasObject.isActiveAndEnabled) || (canvasObject == null && base.isActiveAndEnabled))
		{
			if (Input.GetMouseButtonDown(0))
			{
				pressing = true;
				Vector3 mousePosition = Input.mousePosition;
				pan = mousePosition - canvasRect.position;
			}
			if (Input.GetMouseButton(0) || pressing)
			{
				Vector3 position = Input.mousePosition - pan;
				Vector3 vector = canvasRect.parent.InverseTransformPoint(position);
				Vector2 vector2 = Vector2.Lerp(canvasRect.anchoredPosition, new Vector2(vector.x, vector.y), Time.deltaTime * panSpeed);
				if (isHorizontalOnly)
				{
					if (infiniteScroll)
					{
						canvasRect.anchoredPosition = new Vector2(Mathf.RoundToInt(vector2.x), canvasRect.anchoredPosition.y);
					}
					else
					{
						float x = Mathf.Clamp(Mathf.RoundToInt(vector2.x), minLimit.x, maxLimit.x);
						canvasRect.anchoredPosition = new Vector2(x, canvasRect.anchoredPosition.y);
					}
				}
				else if (infiniteScroll)
				{
					canvasRect.anchoredPosition = new Vector2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
				}
				else
				{
					float x2 = Mathf.Clamp(Mathf.RoundToInt(vector2.x), minLimit.x, maxLimit.x);
					float y = Mathf.Clamp(Mathf.RoundToInt(vector2.y), minLimit.y, maxLimit.y);
					canvasRect.anchoredPosition = new Vector2(x2, y);
				}
			}
		}
		if ((canvasObject != null && !canvasObject.enabled) || Input.GetMouseButtonUp(0))
		{
			pressing = false;
		}
	}

	private void OnEnable()
	{
		pressing = false;
	}

	private void OnDisable()
	{
		pressing = false;
	}
}
