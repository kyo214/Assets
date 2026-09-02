using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

public class MMParallaxUI : MonoBehaviour
{
	[Serializable]
	public class ParallaxLayer
	{
		public RectTransform Rect;

		public float Speed = 2f;

		public float Amplitude = 50f;

		[HideInInspector]
		public Vector2 StartPosition;

		public bool Active = true;
	}

	public enum Modes
	{
		Mouse = 0,
		Gyroscope = 1,
		Script = 2
	}

	public Modes Mode;

	public float AmplitudeMultiplier = 1f;

	public float SpeedMultiplier = 1f;

	public List<ParallaxLayer> ParallaxLayers;

	protected Vector2 _referencePosition;

	protected Vector3 _newPosition;

	protected Vector2 _mousePosition;

	protected virtual void Start()
	{
		Initialization();
	}

	public virtual void Initialization()
	{
		foreach (ParallaxLayer parallaxLayer in ParallaxLayers)
		{
			parallaxLayer.StartPosition = parallaxLayer.Rect.position;
		}
	}

	protected virtual void Update()
	{
		MoveLayers();
	}

	protected virtual void MoveLayers()
	{
		switch (Mode)
		{
		case Modes.Gyroscope:
			_referencePosition = MMGyroscope.CalibratedInputAcceleration;
			break;
		case Modes.Mouse:
			_mousePosition = Mouse.current.position.ReadValue();
			_referencePosition = Camera.main.ScreenToViewportPoint(_mousePosition);
			break;
		}
		foreach (ParallaxLayer parallaxLayer in ParallaxLayers)
		{
			if (parallaxLayer.Active)
			{
				_newPosition.x = Mathf.Lerp(parallaxLayer.Rect.position.x, parallaxLayer.StartPosition.x + _referencePosition.x * parallaxLayer.Amplitude * AmplitudeMultiplier, parallaxLayer.Speed * SpeedMultiplier * Time.deltaTime);
				_newPosition.y = Mathf.Lerp(parallaxLayer.Rect.position.y, parallaxLayer.StartPosition.y + _referencePosition.y * parallaxLayer.Amplitude * AmplitudeMultiplier, parallaxLayer.Speed * SpeedMultiplier * Time.deltaTime);
				_newPosition.z = 0f;
				parallaxLayer.Rect.position = _newPosition;
			}
		}
	}

	public virtual void SetReferencePosition(Vector3 newReferencePosition)
	{
		_referencePosition = newReferencePosition;
	}
}
