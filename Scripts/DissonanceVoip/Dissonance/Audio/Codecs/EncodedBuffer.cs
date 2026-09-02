using System;

namespace Dissonance.Audio.Codecs;

public struct EncodedBuffer(ArraySegment<byte>? encoded, bool packetLost)
{
	public readonly ArraySegment<byte>? Encoded = encoded;

	public readonly bool PacketLost = packetLost;
}
