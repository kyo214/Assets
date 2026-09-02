using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/GUI/MMPSBToUIConverter")]
public class MMPSBToUIConverter : MonoBehaviour
{
	[Header("Target")]
	public Canvas TargetCanvas;

	public float ScaleFactor = 100f;

	public bool ReplicateNesting;

	[Header("Size")]
	public float TargetWidth = 2048f;

	public float TargetHeight = 1152f;

	[Header("Conversion")]
	[MMInspectorButton("ConvertToCanvas")]
	public bool ConvertToCanvasButton;

	public Vector3 ChildImageOffset = new Vector3(-1024f, -576f, 0f);

	protected Transform _topLevel;

	protected Dictionary<Transform, int> _sortingOrders;

	public virtual void ConvertToCanvas()
	{
		Screen.SetResolution((int)TargetWidth, (int)TargetHeight, fullscreen: true, 60);
		_sortingOrders = new Dictionary<Transform, int>();
		foreach (Transform item in TargetCanvas.transform)
		{
			if (item.name == base.name)
			{
				item.MMDestroyAllChildren();
				Object.DestroyImmediate(item.gameObject);
			}
		}
		CanvasScaler component = TargetCanvas.GetComponent<CanvasScaler>();
		if (component != null)
		{
			component.referenceResolution = new Vector2(TargetWidth, TargetHeight);
		}
		GameObject gameObject = new GameObject(base.name, typeof(RectTransform));
		gameObject.transform.SetParent(TargetCanvas.transform);
		RectTransform component2 = gameObject.GetComponent<RectTransform>();
		SetupForStretch(component2);
		_topLevel = gameObject.transform;
		CreateImageForChildren(base.transform, gameObject.transform);
		foreach (KeyValuePair<Transform, int> sortingOrder in _sortingOrders)
		{
			sortingOrder.Key.SetSiblingIndex(sortingOrder.Value);
		}
	}

	protected virtual void CreateImageForChildren(Transform root, Transform parent)
	{
		foreach (Transform item in root)
		{
			GameObject gameObject = new GameObject(item.name, typeof(RectTransform));
			gameObject.transform.localPosition = ScaleFactor * item.transform.localPosition;
			if (ReplicateNesting)
			{
				gameObject.transform.SetParent(parent);
			}
			else
			{
				gameObject.transform.SetParent(_topLevel);
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.x += TargetWidth / 2f;
				gameObject.transform.localPosition = localPosition;
			}
			SpriteRenderer component = item.gameObject.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				Image image = gameObject.AddComponent<Image>();
				image.sprite = component.sprite;
				_sortingOrders.Add(image.transform, component.sortingOrder);
				image.SetNativeSize();
				RectTransform component2 = gameObject.GetComponent<RectTransform>();
				Vector3 localPosition2 = component2.localPosition;
				localPosition2 += ChildImageOffset;
				localPosition2.z = 0f;
				component2.localPosition = localPosition2;
			}
			else
			{
				gameObject.name += " - NODE";
				RectTransform component3 = gameObject.GetComponent<RectTransform>();
				component3.sizeDelta = new Vector2(TargetWidth, TargetHeight);
				component3.localPosition = Vector3.zero;
			}
			gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
			CreateImageForChildren(item, gameObject.transform);
		}
	}

	protected virtual void SetupForStretch(RectTransform rect)
	{
		rect.localPosition = Vector3.zero;
		rect.anchorMin = new Vector2(0f, 0f);
		rect.anchorMax = new Vector2(1f, 1f);
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		rect.localScale = Vector3.one;
	}
}
