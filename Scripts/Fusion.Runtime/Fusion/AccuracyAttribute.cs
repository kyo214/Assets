using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class AccuracyAttribute : Attribute
{
	private float _accuracy;

	internal int CustomHash { get; }

	internal float Accuracy => _accuracy;

	public AccuracyAttribute(double accuracy)
	{
		_accuracy = (float)accuracy;
	}

	public AccuracyAttribute(float accuracy)
	{
		_accuracy = accuracy;
	}

	public AccuracyAttribute(string defaultAccuracyTag)
	{
		CustomHash = defaultAccuracyTag.GetHashDeterministic();
		_accuracy = 0f;
	}
}
