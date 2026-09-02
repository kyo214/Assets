using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "PlayerStats", menuName = "WMO/ScriptableObjects/Stats/Stats")]
public class PlayerStatsSO : ScriptableObject
{
	public float initialValue;

	public float Value;
}
