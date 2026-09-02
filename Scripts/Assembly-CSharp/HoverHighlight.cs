using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHighlight : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerMoveHandler
{
	[SerializeField]
	private bool _isStaticElement;

	[SerializeField]
	private bool _isResetButton;

	private int _idx;

	private void Awake()
	{
		if (_isStaticElement)
		{
			_idx = base.transform.GetSiblingIndex();
		}
	}

	public void SetIndex(int idx)
	{
		_idx = idx;
	}

	public bool GetIsReset()
	{
		return _isResetButton;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!OptionsManager.Instance.TabControlNavMode)
		{
			OptionsManager.Instance.SnapSelectorPosition(_idx, base.transform.position.y);
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		OptionsManager.Instance.TabControlNavMode = false;
	}
}
