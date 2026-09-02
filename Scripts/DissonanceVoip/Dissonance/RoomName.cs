using System;
using JetBrains.Annotations;

namespace Dissonance;

public struct RoomName : IEquatable<RoomName>
{
	[NotNull]
	public string Name { get; set; }

	internal bool SuppressDuplicateCheck { get; set; }

	internal RoomName([NotNull] string name, bool suppress = false)
	{
		Name = name ?? throw new ArgumentNullException("name");
		SuppressDuplicateCheck = suppress;
	}

	public RoomName([NotNull] string name)
	{
		Name = name ?? throw new ArgumentNullException("name");
		SuppressDuplicateCheck = false;
	}

	public bool Equals(RoomName other)
	{
		return string.Equals(Name, other.Name);
	}

	public static implicit operator RoomName(string name)
	{
		return new RoomName(name);
	}

	public static implicit operator string(RoomName name)
	{
		return name.Name;
	}
}
