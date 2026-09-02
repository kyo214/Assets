using JetBrains.Annotations;

namespace Dissonance;

internal interface IRooms
{
	[CanBeNull]
	string Name(ushort id);
}
