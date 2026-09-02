using UnityEngine;

namespace MoreMountains.Tools;

public class MMTransformRandomizer : MonoBehaviour
{
	public enum AutoExecutionModes
	{
		Never = 0,
		OnAwake = 1,
		OnStart = 2,
		OnEnable = 3
	}

	[Header("Position")]
	public bool RandomizePosition = true;

	[MMCondition("RandomizePosition", true)]
	public Vector3 MinRandomPosition;

	[MMCondition("RandomizePosition", true)]
	public Vector3 MaxRandomPosition;

	[Header("Rotation")]
	public bool RandomizeRotation = true;

	[MMCondition("RandomizeRotation", true)]
	public Vector3 MinRandomRotation;

	[MMCondition("RandomizeRotation", true)]
	public Vector3 MaxRandomRotation;

	[Header("Scale")]
	public bool RandomizeScale = true;

	[MMCondition("RandomizeScale", true)]
	public Vector3 MinRandomScale;

	[MMCondition("RandomizeScale", true)]
	public Vector3 MaxRandomScale;

	[Header("Settings")]
	public bool AutoRemoveAfterRandomize;

	public bool RemoveAllColliders;

	public AutoExecutionModes AutoExecutionMode;

	protected virtual void Awake()
	{
		if (Application.isPlaying && AutoExecutionMode == AutoExecutionModes.OnAwake)
		{
			Randomize();
		}
	}

	protected virtual void Start()
	{
		if (Application.isPlaying && AutoExecutionMode == AutoExecutionModes.OnStart)
		{
			Randomize();
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying && AutoExecutionMode == AutoExecutionModes.OnEnable)
		{
			Randomize();
		}
	}

	public virtual void Randomize()
	{
		ProcessRandomizePosition();
		ProcessRandomizeRotation();
		ProcessRandomizeScale();
		RemoveColliders();
		Cleanup();
	}

	protected virtual void ProcessRandomizePosition()
	{
		if (RandomizePosition)
		{
			Vector3 vector = MMMaths.RandomVector3(MinRandomPosition, MaxRandomPosition);
			base.transform.localPosition += vector;
		}
	}

	protected virtual void ProcessRandomizeRotation()
	{
		if (RandomizeRotation)
		{
			Vector3 euler = MMMaths.RandomVector3(MinRandomRotation, MaxRandomRotation);
			base.transform.localRotation = Quaternion.Euler(euler);
		}
	}

	protected virtual void ProcessRandomizeScale()
	{
		if (RandomizeScale)
		{
			Vector3 localScale = MMMaths.RandomVector3(MinRandomScale, MaxRandomScale);
			base.transform.localScale = localScale;
		}
	}

	protected virtual void RemoveColliders()
	{
		_ = RemoveAllColliders;
	}

	protected virtual void Cleanup()
	{
		_ = AutoRemoveAfterRandomize;
	}
}
