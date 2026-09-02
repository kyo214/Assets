namespace Dissonance.Networking;

public struct TextMessage(string sender, ChannelType recipientType, string recipient, string message)
{
	public readonly string Sender = sender;

	public readonly ChannelType RecipientType = recipientType;

	public readonly string Recipient = recipient;

	public readonly string Message = message;
}
