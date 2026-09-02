using JetBrains.Annotations;

namespace Dissonance;

public struct RoomMembership
{
	private readonly RoomName _name;

	internal int Count;

	[NotNull]
	public string RoomName => _name.Name;

	public ushort RoomId { get; }

	internal RoomMembership(RoomName name, int count)
	{
		_name = name;
		RoomId = name.ToRoomId();
		Count = count;
	}
}
