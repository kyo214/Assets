using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRefactorTool : MonoBehaviour
{
	public enum Vector3Preset
	{
		Default = 0,
		Coordinate0 = 1,
		Coordinate45 = 2,
		Coordinate90 = 3,
		Coordinate135 = 4,
		Coordinate180 = 5,
		Coordinate225 = 6,
		Coordinate270 = 7,
		Coordinate315 = 8
	}

	[Serializable]
	public class TransformData
	{
		public Vector3 position;

		public Vector3 rotation;

		public Vector3 scale = Vector3.one;
	}

	[Header("Nudge Step Amounts")]
	public Vector3 posNudgeAmount = new Vector3(0.01f, 0.01f, 0.01f);

	public Vector3 rotNudgeAmount = new Vector3(5f, 5f, 5f);

	public Vector3 sclNudgeAmount = new Vector3(0.1f, 0.1f, 0.1f);

	[HideInInspector]
	public List<TransformData> presetData = new List<TransformData>();

	public Vector3Preset currentPreset;

	private void OnValidate()
	{
		int num = Enum.GetNames(typeof(Vector3Preset)).Length;
		while (presetData.Count < num)
		{
			presetData.Add(new TransformData());
		}
	}
}
