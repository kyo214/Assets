using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HighlightPlus;

[RequireComponent(typeof(HighlightEffect))]
[ExecuteInEditMode]
[HelpURL("https://www.dropbox.com/s/1p9h8xys68lm4a3/Documentation.pdf?dl=0")]
public class HighlightTrigger : MonoBehaviour
{
	[Tooltip("Enables highlight when pointer is over this object.")]
	public bool highlightOnHover = true;

	[Tooltip("Used to trigger automatic highlighting including children objects.")]
	public TriggerMode triggerMode = TriggerMode.RaycastOnThisObjectAndChildren;

	public Camera raycastCamera;

	public RayCastSource raycastSource;

	[Tooltip("Minimum distance for target.")]
	public float minDistance;

	[Tooltip("Maximum distance for target. 0 = infinity")]
	public float maxDistance;

	[Tooltip("Blocks interaction if pointer is over an UI element")]
	public bool respectUI = true;

	public LayerMask volumeLayerMask;

	private const int MAX_RAYCAST_HITS = 100;

	[Tooltip("If the object will be selected by clicking with mouse or tapping on it.")]
	public bool selectOnClick;

	[Tooltip("Profile to use when object is selected by clicking on it.")]
	public HighlightProfile selectedProfile;

	[Tooltip("Profile to use whtn object is selected and highlighted.")]
	public HighlightProfile selectedAndHighlightedProfile;

	[Tooltip("Automatically deselects any other selected object prior selecting this one")]
	public bool singleSelection;

	[Tooltip("Toggles selection on/off when clicking object")]
	public bool toggle;

	[NonSerialized]
	public Collider[] colliders;

	private Collider currentCollider;

	private static RaycastHit[] hits;

	private HighlightEffect hb;

	private TriggerMode currentTriggerMode;

	public HighlightEffect highlightEffect => hb;

	public event OnObjectSelectionEvent OnObjectSelected;

	public event OnObjectSelectionEvent OnObjectUnSelected;

	public event OnObjectHighlightEvent OnObjectHighlightStart;

	public event OnObjectHighlightEvent OnObjectHighlightEnd;

	[RuntimeInitializeOnLoadMethod]
	private void DomainReloadDisabledSupport()
	{
		HighlightManager.selectedObjects.Clear();
	}

	private void OnEnable()
	{
		Init();
	}

	private void OnValidate()
	{
		if (currentTriggerMode == triggerMode)
		{
			return;
		}
		currentTriggerMode = triggerMode;
		if (currentTriggerMode != TriggerMode.RaycastOnThisObjectAndChildren)
		{
			return;
		}
		colliders = GetComponentsInChildren<Collider>();
		if (hits == null || hits.Length != 100)
		{
			hits = new RaycastHit[100];
		}
		if (Application.isPlaying)
		{
			StopAllCoroutines();
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(DoRayCast());
			}
		}
	}

	public void Init()
	{
		if (raycastCamera == null)
		{
			raycastCamera = HighlightManager.GetCamera();
		}
		currentTriggerMode = triggerMode;
		if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren)
		{
			colliders = GetComponentsInChildren<Collider>();
		}
		if (hb == null)
		{
			hb = GetComponent<HighlightEffect>();
		}
		InputProxy.Init();
	}

	private void Start()
	{
		if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren)
		{
			if (raycastCamera == null)
			{
				raycastCamera = HighlightManager.GetCamera();
				if (raycastCamera == null)
				{
					Debug.LogError("Highlight Trigger on " + base.gameObject.name + ": no camera found!");
				}
			}
			if (colliders != null && colliders.Length != 0)
			{
				hits = new RaycastHit[100];
				if (Application.isPlaying)
				{
					StartCoroutine(DoRayCast());
				}
			}
		}
		else if (GetComponent<Collider>() == null && GetComponent<MeshFilter>() != null)
		{
			base.gameObject.AddComponent<MeshCollider>();
		}
	}

	private IEnumerator DoRayCast()
	{
		yield return null;
		while (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren)
		{
			if (raycastCamera == null)
			{
				yield return null;
				continue;
			}
			bool flag = false;
			int count;
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
				count = list.Count;
				bool flag2 = false;
				for (int i = 0; i < count; i++)
				{
					if (list[i].module is GraphicRaycaster)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					yield return null;
					continue;
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
					for (int k = 0; k < colliders.Length; k++)
					{
						if (colliders[k].gameObject == gameObject)
						{
							Collider collider = colliders[k];
							flag = true;
							if (selectOnClick && InputProxy.GetMouseButtonDown(0))
							{
								ToggleSelection();
								break;
							}
							if (collider != currentCollider)
							{
								SwitchCollider(collider);
								j = count;
								break;
							}
						}
					}
				}
			}
			Ray ray = ((raycastSource != RayCastSource.MousePosition) ? new Ray(raycastCamera.transform.position, raycastCamera.transform.forward) : raycastCamera.ScreenPointToRay(InputProxy.mousePosition));
			count = ((!(maxDistance > 0f)) ? Physics.RaycastNonAlloc(ray, hits) : Physics.RaycastNonAlloc(ray, hits, maxDistance));
			for (int l = 0; l < count; l++)
			{
				if (Vector3.Distance(hits[l].point, ray.origin) < minDistance)
				{
					continue;
				}
				Collider collider2 = hits[l].collider;
				for (int m = 0; m < colliders.Length; m++)
				{
					if (colliders[m] == collider2)
					{
						flag = true;
						if (selectOnClick && InputProxy.GetMouseButtonDown(0))
						{
							ToggleSelection();
							break;
						}
						if (collider2 != currentCollider)
						{
							SwitchCollider(collider2);
							l = count;
							break;
						}
					}
				}
			}
			if (!flag && currentCollider != null)
			{
				SwitchCollider(null);
			}
			yield return null;
		}
	}

	private EventSystem CreateEventSystem()
	{
		return new GameObject("Event System created by Highlight Plus", typeof(EventSystem), typeof(InputSystemUIInputModule)).GetComponent<EventSystem>();
	}

	private void SwitchCollider(Collider newCollider)
	{
		if (highlightOnHover || hb.isSelected)
		{
			currentCollider = newCollider;
			if (currentCollider != null)
			{
				Highlight(state: true);
			}
			else
			{
				Highlight(state: false);
			}
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

	private void OnMouseDown()
	{
		if (base.isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject && CanInteract())
		{
			if (selectOnClick && InputProxy.GetMouseButtonDown(0))
			{
				ToggleSelection();
			}
			else
			{
				Highlight(state: true);
			}
		}
	}

	private void OnMouseEnter()
	{
		if (base.isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject && CanInteract())
		{
			Highlight(state: true);
		}
	}

	private void OnMouseExit()
	{
		if (base.isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject && CanInteract())
		{
			Highlight(state: false);
		}
	}

	private void Highlight(bool state)
	{
		if (state)
		{
			if (!hb.highlighted && OnObjectHighlightStart != null && hb.target != null && !OnObjectHighlightStart(hb.target.gameObject))
			{
				return;
			}
		}
		else if (hb.highlighted && OnObjectHighlightEnd != null && hb.target != null)
		{
			OnObjectHighlightEnd(hb.target.gameObject);
		}
		if (selectOnClick)
		{
			if (hb.isSelected)
			{
				if (state && selectedAndHighlightedProfile != null)
				{
					selectedAndHighlightedProfile.Load(hb);
				}
				else if (selectedProfile != null)
				{
					selectedProfile.Load(hb);
				}
				else
				{
					hb.previousSettings.Load(hb);
				}
				if (hb.highlighted)
				{
					hb.UpdateMaterialProperties();
				}
				else
				{
					hb.SetHighlighted(state: true);
				}
				return;
			}
			if (!highlightOnHover)
			{
				hb.SetHighlighted(state: false);
				return;
			}
		}
		hb.SetHighlighted(state);
	}

	private void ToggleSelection()
	{
		HighlightManager.lastTriggerTime = Time.frameCount;
		bool flag = !toggle || !hb.isSelected;
		if (flag)
		{
			if (OnObjectSelected != null && !OnObjectSelected(base.gameObject))
			{
				return;
			}
		}
		else if (OnObjectUnSelected != null && !OnObjectUnSelected(base.gameObject))
		{
			return;
		}
		if (singleSelection & flag)
		{
			HighlightManager.DeselectAll();
		}
		hb.isSelected = flag;
		if (flag && !HighlightManager.selectedObjects.Contains(hb))
		{
			HighlightManager.selectedObjects.Add(hb);
		}
		else if (!flag && HighlightManager.selectedObjects.Contains(hb))
		{
			HighlightManager.selectedObjects.Remove(hb);
		}
		if (hb.isSelected)
		{
			if (hb.previousSettings == null)
			{
				hb.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
			}
			hb.previousSettings.Save(hb);
		}
		else
		{
			hb.RestorePreviousHighlightEffectSettings();
		}
		Highlight(state: true);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (triggerMode == TriggerMode.Volume && ((int)volumeLayerMask & (1 << other.gameObject.layer)) != 0)
		{
			Highlight(state: true);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (triggerMode == TriggerMode.Volume && ((int)volumeLayerMask & (1 << other.gameObject.layer)) != 0)
		{
			Highlight(state: false);
		}
	}
}
