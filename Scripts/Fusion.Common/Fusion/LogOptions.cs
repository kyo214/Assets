using System;

namespace Fusion;

public readonly struct LogOptions
{
	public readonly bool UseColorTags;

	private readonly Func<object, int> _getColor;

	public int GetColor(object obj)
	{
		return _getColor?.Invoke(obj) ?? 0;
	}

	public LogOptions(bool useColorTags, Func<object, int> getColor)
	{
		UseColorTags = useColorTags;
		_getColor = getColor;
	}
}
