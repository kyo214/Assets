using JetBrains.Annotations;

namespace Dissonance.Networking;

internal readonly struct TextPacket(ushort sender, ChannelType recipientType, ushort recipient, [CanBeNull] string text)
{
	public readonly ushort Sender = sender;

	public readonly ChannelType RecipientType = recipientType;

	public readonly ushort Recipient = recipient;

	[CanBeNull]
	public readonly string Text = text;
}
