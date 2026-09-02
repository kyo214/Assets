using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HighlightPlus;

[RequireComponent(typeof(HighlightEffect))]
[DefaultExecutionOrder(100)]
[HelpURL("https://www.dropbox.com/s/1p9h8xys68lm4a3/Documentation.pdf?dl=0")]
public class HighlightManager : MonoBehaviour
{
	[Tooltip("Enables highlight when pointer is over this object.")]
	public bool highlightOnHover = true;

	public LayerMask layerMask = -1;

	public Camera raycastCamera;

	public RayCastSource raycastSource;

	[Tooltip("Minimum distance for target.")]
	public float minDistance;

	[Tooltip("Maximum distance for target. 0 = infinity")]
	public float maxDistance;

	[Tooltip("Blocks interaction if pointer is over an UI element")]
	public bool respectUI = true;

	[Tooltip("If the object will be selected by clicking with mouse or tapping on it.")]
	public bool selectOnClick;

	[Tooltip("Optional profile for objects selected by clicking on them")]
	public HighlightProfile selectedProfile;

	[Tooltip("Profile to use whtn object is selected and highlighted.")]
	public HighlightProfile selectedAndHighlightedProfile;

	[Tooltip("Automatically deselects other previously selected objects")]
	public bool singleSelection;

	[Tooltip("Toggles selection on/off when clicking object")]
	public bool toggle;

	private HighlightEffect baseEffect;

	private HighlightEffect currentEffect;

	private Transform currentObject;

	public static readonly List<HighlightEffect> selectedObjects = new List<HighlightEffect>();

	public static int lastTriggerTime;

	private static HighlightManager _instance;

	public static HighlightManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<HighlightManager>();
			}
			return _instance;
		}
	}

	public event OnObjectSelectionEvent OnObjectSelected;

	public event OnObjectSelectionEvent OnObjectUnSelected;

	public event OnObjectHighlightEvent OnObjectHighlightStart;

	public event OnObjectHighlightEvent OnObjectHighlightEnd;

	[RuntimeInitializeOnLoadMethod]
	private void DomainReloadDisabledSupport()
	{
		selectedObjects.Clear();
		lastTriggerTime = 0;
		_instance = null;
	}

	private void OnEnable()
	{
		currentObject = null;
		currentEffect = null;
		if (baseEffect == null)
		{
			baseEffect = GetComponent<HighlightEffect>();
			if (baseEffect == null)
			{
				baseEffect = base.gameObject.AddComponent<HighlightEffect>();
			}
		}
		raycastCamera = GetComponent<Camera>();
		if (raycastCamera == null)
		{
			raycastCamera = GetCamera();
			if (raycastCamera == null)
			{
				Debug.LogError("Highlight Manager: no camera found!");
			}
		}
		InputProxy.Init();
	}

	private void OnDisable()
	{
		SwitchesCollider(null);
		internal_DeselectAll();
	}

	private void Update()
	{
		if (raycastCamera == null)
		{
			return;
		}
		if (respectUI)
		{
			EventSystem eventSystem = EventSystem.current;
			if (eventSystem == null)
			{
				eventSystem = CreateEventSystem();
			}
			List<RaycastResult> list = new List<RaycastResult>();
			PointerEventData pointerEventData = new PointerEventData(eventSystem);
			Vector3 position = raycastCamera.transform.position;
			if (raycastSource == RayCastSource.MousePosition)
			{
				pointerEventData.position = InputProxy.mousePosition;
			}
			else
			{
				pointerEventData.position = new Vector2((float)raycastCamera.pixelWidth * 0.5f, (float)raycastCamera.pixelHeight * 0.5f);
			}
			eventSystem.RaycastAll(pointerEventData, list);
			int count = list.Count;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				if (list[i].module is GraphicRaycaster)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return;
			}
			for (int j = 0; j < count; j++)
			{
				RaycastResult raycastResult = list[j];
				float num = Vector3.Distance(raycastResult.worldPosition, position);
				if (num < minDistance || (maxDistance > 0f && num > maxDistance))
				{
					continue;
				}
				GameObject gameObject = raycastResult.gameObject;
				if (((int)layerMask & (1 << raycastResult.gameObject.layer)) == 0)
				{
					continue;
				}
				if (gameObject.GetComponent<HighlightTrigger>() != null)
				{
					return;
				}
				Transform transform = gameObject.transform;
				if (InputProxy.GetMouseButtonDown(0))
				{
					if (selectOnClick)
					{
						ToggleSelection(transform, !toggle);
					}
					else if (lastTriggerTime < Time.frameCount)
					{
						internal_DeselectAll();
					}
				}
				else if (transform != currentObject)
				{
					SwitchesCollider(transform);
				}
				return;
			}
		}
		Ray ray = ((raycastSource != RayCastSource.MousePosition) ? new Ray(raycastCamera.transform.position, raycastCamera.transform.forward) : raycastCamera.ScreenPointToRay(InputProxy.mousePosition));
		if (Physics.Raycast(ray, out var hitInfo, (maxDistance > 0f) ? maxDistance : raycastCamera.farClipPlane, layerMask) && Vector3.Distance(hitInfo.point, ray.origin) >= minDistance)
		{
			Transform transform2 = hitInfo.collider.transform;
			transform2.TryGetComponent<HighlightTrigger>(out var component);
			if (component != null)
			{
				return;
			}
			if (InputProxy.GetMouseButtonDown(0))
			{
				if (selectOnClick)
				{
					ToggleSelection(transform2, !toggle);
				}
				else if (lastTriggerTime < Time.frameCount)
				{
					internal_DeselectAll();
				}
			}
			else if (transform2 != currentObject)
			{
				SwitchesCollider(transform2);
			}
		}
		else
		{
			if (InputProxy.GetMouseButtonDown(0) && lastTriggerTime < Time.frameCount)
			{
				internal_DeselectAll();
			}
			SwitchesCollider(null);
		}
	}

	private EventSystem CreateEventSystem()
	{
		return new GameObject("Event System created by Highlight Plus", typeof(EventSystem), typeof(InputSystemUIInputModule)).GetComponent<EventSystem>();
	}

	private void SwitchesCollider(Transform newObject)
	{
		if (currentEffect != null)
		{
			if (highlightOnHover)
			{
				Highlight(state: false);
			}
			currentEffect = null;
		}
		currentObject = newObject;
		if (newObject == null)
		{
			return;
		}
		HighlightTrigger component = newObject.GetComponent<HighlightTrigger>();
		if (component != null && component.enabled)
		{
			return;
		}
		HighlightEffect component2 = newObject.GetComponent<HighlightEffect>();
		if (component2 == null)
		{
			HighlightEffect componentInParent = newObject.GetComponentInParent<HighlightEffect>();
			if (componentInParent != null && componentInParent.Includes(newObject))
			{
				currentEffect = componentInParent;
				if (highlightOnHover)
				{
					Highlight(state: true);
				}
				return;
			}
		}
		currentEffect = ((component2 != null) ? component2 : baseEffect);
		baseEffect.enabled = currentEffect == baseEffect;
		currentEffect.SetTarget(currentObject);
		if (highlightOnHover)
		{
			Highlight(state: true);
		}
	}

	private bool CanInteract()
	{
		if (!respectUI)
		{
			return true;
		}
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return true;
		}
		if (Application.isMobilePlatform && InputProxy.touchCount > 0 && current.IsPointerOverGameObject(InputProxy.GetFingerIdFromTouch(0)))
		{
			return false;
		}
		if (current.IsPointerOverGameObject(-1))
		{
			return false;
		}
		return true;
	}

	private void ToggleSelection(Transform t, bool forceSelection)
	{
		HighlightEffect highlightEffect = t.GetComponent<HighlightEffect>();
		if (highlightEffect == null)
		{
			HighlightEffect componentInParent = t.GetComponentInParent<HighlightEffect>();
			if (componentInParent != null && componentInParent.Includes(t))
			{
				highlightEffect = componentInParent;
				if (highlightEffect.previousSettings == null)
				{
					highlightEffect.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
				}
				highlightEffect.previousSettings.Save(highlightEffect);
			}
			else
			{
				highlightEffect = t.gameObject.AddComponent<HighlightEffect>();
				highlightEffect.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
				highlightEffect.previousSettings.Save(baseEffect);
				highlightEffect.previousSettings.Load(highlightEffect);
			}
		}
		bool isSelected = highlightEffect.isSelected;
		bool flag = forceSelection || !isSelected;
		if (flag == isSelected)
		{
			return;
		}
		if (flag)
		{
			if (OnObjectSelected != null && !OnObjectSelected(t.gameObject))
			{
				return;
			}
		}
		else if (OnObjectUnSelected != null && !OnObjectUnSelected(t.gameObject))
		{
			return;
		}
		if (singleSelection)
		{
			internal_DeselectAll();
		}
		currentEffect = highlightEffect;
		currentEffect.isSelected = flag;
		baseEffect.enabled = false;
		if (currentEffect.isSelected)
		{
			if (currentEffect.previousSettings == null)
			{
				currentEffect.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
			}
			highlightEffect.previousSettings.Save(highlightEffect);
			if (!selectedObjects.Contains(currentEffect))
			{
				selectedObjects.Add(currentEffect);
			}
		}
		else
		{
			if (currentEffect.previousSettings != null)
			{
				currentEffect.previousSettings.Load(highlightEffect);
			}
			if (selectedObjects.Contains(currentEffect))
			{
				selectedObjects.Remove(currentEffect);
			}
		}
		Highlight(state: true);
	}

	private void Highlight(bool state)
	{
		if (state)
		{
			if (!currentEffect.highlighted && OnObjectHighlightStart != null && currentEffect.target != null && !OnObjectHighlightStart(currentEffect.target.gameObject))
			{
				return;
			}
		}
		else if (currentEffect.highlighted && OnObjectHighlightEnd != null && currentEffect.target != null)
		{
			OnObjectHighlightEnd(currentEffect.target.gameObject);
		}
		if (selectOnClick)
		{
			if (currentEffect.isSelected)
			{
				if (state && selectedAndHighlightedProfile != null)
				{
					selectedAndHighlightedProfile.Load(currentEffect);
				}
				else if (selectedProfile != null)
				{
					selectedProfile.Load(currentEffect);
				}
				else
				{
					currentEffect.previousSettings.Load(currentEffect);
				}
				if (currentEffect.highlighted)
				{
					currentEffect.UpdateMaterialProperties();
				}
				else
				{
					currentEffect.SetHighlighted(state: true);
				}
				return;
			}
			if (!highlightOnHover)
			{
				currentEffect.SetHighlighted(state: false);
				return;
			}
		}
		currentEffect.SetHighlighted(state);
	}

	public static Camera GetCamera()
	{
		Camera camera = Camera.main;
		if (camera == null)
		{
			camera = Object.FindObjectOfType<Camera>();
		}
		return camera;
	}

	private void internal_DeselectAll()
	{
		foreach (HighlightEffect selectedObject in selectedObjects)
		{
			if (selectedObject != null && selectedObject.gameObject != null && (OnObjectUnSelected == null || OnObjectUnSelected(selectedObject.gameObject)))
			{
				selectedObject.RestorePreviousHighlightEffectSettings();
				selectedObject.isSelected = false;
				selectedObject.SetHighlighted(state: false);
			}
		}
		selectedObjects.Clear();
	}

	public static void DeselectAll()
	{
		foreach (HighlightEffect selectedObject in selectedObjects)
		{
			if (selectedObject != null && selectedObject.gameObject != null)
			{
				selectedObject.isSelected = false;
				if (selectedObject.highlighted && _instance != null)
				{
					_instance.Highlight(state: false);
				}
				else
				{
					selectedObject.SetHighlighted(state: false);
				}
			}
		}
		selectedObjects.Clear();
	}

	public void SelectObject(Transform t)
	{
		ToggleSelection(t, forceSelection: true);
	}

	public void ToggleObject(Transform t)
	{
		ToggleSelection(t, forceSelection: false);
	}

	public void UnselectObject(Transform t)
	{
		if (!(t == null))
		{
			HighlightEffect component = t.GetComponent<HighlightEffect>();
			if (!(component == null) && selectedObjects.Contains(component) && (OnObjectUnSelected == null || OnObjectUnSelected(component.gameObject)))
			{
				component.isSelected = false;
				component.SetHighlighted(state: false);
				selectedObjects.Remove(component);
			}
		}
	}
}
