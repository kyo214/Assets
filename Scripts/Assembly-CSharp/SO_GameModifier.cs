using System;
using UnityEngine;

[Serializable]
public abstract class SO_GameModifier : ScriptableObject
{
	public abstract void Apply();
}
