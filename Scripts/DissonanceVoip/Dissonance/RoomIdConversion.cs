using System;
using Dissonance.Extensions;
using JetBrains.Annotations;

namespace Dissonance;

public static class RoomIdConversion
{
	public static ushort ToRoomId(this string name)
	{
		return new RoomName(name).ToRoomId();
	}

	public static ushort ToRoomId(this RoomName name)
	{
		if (name.Name == null)
		{
			throw new ArgumentNullException("name");
		}
		return Hash16(name.Name);
	}

	private static ushort Hash16([NotNull] string str)
	{
		int fnvHashCode = str.GetFnvHashCode();
		ushort num = (ushort)(fnvHashCode >> 16);
		ushort num2 = (ushort)fnvHashCode;
		return (ushort)(num * 5791 + num2 * 7639);
	}
}
