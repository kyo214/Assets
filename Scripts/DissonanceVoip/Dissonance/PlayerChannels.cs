using Dissonance.Audio.Capture;
using JetBrains.Annotations;

namespace Dissonance;

public sealed class PlayerChannels : Channels<PlayerChannel, string>
{
	internal PlayerChannels([NotNull] IChannelPriorityProvider priorityProvider)
		: base(priorityProvider)
	{
		base.OpenedChannel += (string id, ChannelProperties _) =>
		{
		};
		base.ClosedChannel += (string id, ChannelProperties _) =>
		{
		};
	}

	protected override PlayerChannel CreateChannel(ushort subscriptionId, string channelId, ChannelProperties properties)
	{
		return new PlayerChannel(subscriptionId, channelId, this, properties);
	}
}
