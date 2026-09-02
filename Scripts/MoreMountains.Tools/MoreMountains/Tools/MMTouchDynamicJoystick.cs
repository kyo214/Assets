using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Controls/MMTouchDynamicJoystick")]
public class MMTouchDynamicJoystick : MMTouchJoystick, IPointerDownHandler, IEventSystemHandler
{
	[Header("Dynamic Joystick")]
	[MMInformation("Here you can select an image for your joystick's knob, and decide if the joystick's detection zone should reset its position whenever the drag ends.", MMInformationAttribute.InformationType.Info, false)]
	public Sprite JoystickKnobImage;

	public bool RestorePosition = true;

	protected Vector3 _initialPosition;

	protected Vector3 _newPosition;

	protected CanvasGroup _knobCanvasGroup;

	protected override void Start()
	{
		base.Start();
		_initialPosition = GetComponent<RectTransform>().localPosition;
		if (JoystickKnobImage != null)
		{
			GameObject gameObject = new GameObject();
			SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
			gameObject.transform.SetParent(base.gameObject.transform);
			gameObject.name = "DynamicJoystickKnob";
			gameObject.transform.position = base.transform.position;
			gameObject.transform.localScale = base.transform.localScale;
			gameObject.AddComponent<Image>().sprite = JoystickKnobImage;
			_knobCanvasGroup = gameObject.AddComponent<CanvasGroup>();
		}
	}

	public virtual void OnPointerDown(PointerEventData data)
	{
		if (base.ParentCanvasRenderMode == RenderMode.ScreenSpaceCamera)
		{
			_newPosition = TargetCamera.ScreenToWorldPoint(data.position);
		}
		else
		{
			_newPosition = data.position;
		}
		_newPosition.z = base.transform.position.z;
		SetNeutralPosition(_newPosition);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		if (RestorePosition)
		{
			GetComponent<RectTransform>().localPosition = _initialPosition;
		}
	}
}
