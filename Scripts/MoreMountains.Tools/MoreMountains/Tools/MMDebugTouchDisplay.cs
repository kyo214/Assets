using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugTouchDisplay : MonoBehaviour
{
	[Header("Bindings")]
	public Canvas TargetCanvas;

	[Header("Touches")]
	public RectTransform TouchPrefab;

	public int TouchProvision = 6;

	protected List<RectTransform> _touchDisplays;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_touchDisplays = new List<RectTransform>();
		for (int i = 0; i < TouchProvision; i++)
		{
			RectTransform rectTransform = Object.Instantiate(TouchPrefab);
			rectTransform.transform.SetParent(TargetCanvas.transform);
			rectTransform.name = "MMDebugTouchDisplay_" + i;
			rectTransform.gameObject.SetActive(value: false);
			_touchDisplays.Add(rectTransform);
		}
		base.enabled = false;
	}

	protected virtual void Update()
	{
		DisableAllDisplays();
		DetectTouches();
	}

	protected virtual void DetectTouches()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			_touchDisplays[i].gameObject.SetActive(value: true);
			_touchDisplays[i].position = Input.GetTouch(i).position;
		}
	}

	protected virtual void DisableAllDisplays()
	{
		foreach (RectTransform touchDisplay in _touchDisplays)
		{
			touchDisplay.gameObject.SetActive(value: false);
		}
	}

	protected virtual void OnDisable()
	{
		DisableAllDisplays();
	}
}
