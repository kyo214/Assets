using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[ExecuteAlways]
public class MMTwoSidedUI : MonoBehaviour
{
	public enum Axis
	{
		x = 0,
		y = 1,
		z = 2
	}

	[Header("Bindings")]
	public GameObject Front;

	public GameObject Back;

	[Header("Axis")]
	public Axis FlipAxis;

	public float ScaleThreshold;

	[Header("Events")]
	public UnityEvent OnFlip;

	[Header("Debug")]
	public bool DebugMode;

	[Range(-1f, 1f)]
	public float ScaleValue;

	[MMReadOnly]
	public bool BackVisible;

	protected RectTransform _rectTransform;

	protected bool _initialized;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_rectTransform = base.gameObject.GetComponent<RectTransform>();
		_initialized = true;
		float scaleValue = GetScaleValue();
		BackVisible = scaleValue < ScaleThreshold;
		Front.SetActive(!BackVisible);
		Back.SetActive(BackVisible);
	}

	protected virtual void Update()
	{
		float scaleValue = GetScaleValue();
		if (scaleValue < ScaleThreshold != BackVisible)
		{
			Front.SetActive(BackVisible);
			Back.SetActive(!BackVisible);
			OnFlip?.Invoke();
		}
		BackVisible = scaleValue < ScaleThreshold;
	}

	protected virtual void IfEditor()
	{
		if (!_initialized)
		{
			Initialization();
		}
		if (DebugMode)
		{
			switch (FlipAxis)
			{
			case Axis.x:
				_rectTransform.localScale = new Vector3(ScaleValue, _rectTransform.localScale.y, _rectTransform.localScale.z);
				break;
			case Axis.y:
				_rectTransform.localScale = new Vector3(_rectTransform.localScale.x, ScaleValue, _rectTransform.localScale.z);
				break;
			case Axis.z:
				_rectTransform.localScale = new Vector3(_rectTransform.localScale.x, _rectTransform.localScale.y, ScaleValue);
				break;
			}
		}
	}

	protected virtual float GetScaleValue()
	{
		return FlipAxis switch
		{
			Axis.x => _rectTransform.localScale.x, 
			Axis.y => _rectTransform.localScale.y, 
			Axis.z => _rectTransform.localScale.z, 
			_ => 0f, 
		};
	}
}
