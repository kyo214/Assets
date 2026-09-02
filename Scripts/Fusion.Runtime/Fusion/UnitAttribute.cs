using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field)]
public sealed class UnitAttribute : PropertyAttribute
{
	public int DecimalPlaces = 5;

	public bool ClampMin;

	public bool ClampMax;

	public bool UseInverse;

	public string InverseName;

	public Units InverseUnit;

	public int InverseDecimalPlaces = 5;

	public bool UseSlider;

	internal Units Unit { get; }

	internal double Min { get; }

	internal double Max { get; }

	public UnitAttribute(Units unit)
	{
		Unit = unit;
	}

	public UnitAttribute(Units unit, double min, double max)
	{
		Unit = unit;
		Min = min;
		Max = max;
		ClampMin = true;
		ClampMax = true;
		UseSlider = min != 0.0 || max != 0.0;
	}
}
