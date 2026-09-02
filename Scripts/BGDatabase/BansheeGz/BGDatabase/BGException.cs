using System;

namespace BansheeGz.BGDatabase;

public class BGException : Exception
{
	public BGException(string message, params object[] args)
		: base(BGUtil.Format(message, args))
	{
	}

	public static void ThrowIf(bool condition, string message, params object[] path)
	{
		if (!condition)
		{
			return;
		}
		throw new BGException(message, path);
	}
}
