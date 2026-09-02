using UnityEngine;

namespace MoreMountains.Tools;

public class MMProgressBarDemoAuto : MonoBehaviour
{
	public enum TestModes
	{
		Permanent = 0,
		OneTime = 1
	}

	public TestModes TestMode;

	[MMEnumCondition("TestMode", new int[] { 0 })]
	public float CurrentValue;

	[MMEnumCondition("TestMode", new int[] { 0 })]
	public float MinValue;

	[MMEnumCondition("TestMode", new int[] { 0 })]
	public float MaxValue = 100f;

	[MMEnumCondition("TestMode", new int[] { 0 })]
	public float Speed = 1f;

	[MMEnumCondition("TestMode", new int[] { 1 })]
	public float OneTimeNewValue;

	[MMEnumCondition("TestMode", new int[] { 1 })]
	public float OneTimeMinValue;

	[MMEnumCondition("TestMode", new int[] { 1 })]
	public float OneTimeMaxValue;

	[MMEnumCondition("TestMode", new int[] { 1 })]
	[MMInspectorButton("OneTime")]
	public bool OneTimeButton;

	protected float _direction = 1f;

	protected MMProgressBar _progressBar;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_progressBar = GetComponent<MMProgressBar>();
	}

	protected virtual void Update()
	{
		if (TestMode == TestModes.Permanent)
		{
			_progressBar.UpdateBar(CurrentValue, MinValue, MaxValue);
			CurrentValue += Speed * Time.deltaTime * _direction;
			if (CurrentValue <= MinValue || CurrentValue >= MaxValue)
			{
				_direction *= -1f;
			}
		}
	}

	protected virtual void OneTime()
	{
		_progressBar.UpdateBar(OneTimeNewValue, OneTimeMinValue, OneTimeMaxValue);
	}
}
