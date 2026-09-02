using System;

namespace DestroyIt;

[Serializable]
public class DamageLevel
{
	public float maxHitPoints;

	public float minHitPoints;

	public int healthPercent;

	public bool hasError;

	public int visibleDamageLevel;
}
