#define DEBUG
using System;
using Fusion.Photon.Realtime;
using Fusion.Protocol;

namespace Fusion;

internal class CloudCommunicator : CommunicatorBase, IDisposable
{
	private FusionRelayClient _client;

	private readonly byte[] _buffer = new byte[65536];

	public FusionRelayClient Client => _client;

	public override int CommunicatorID => (_client != null) ? _client.LocalPlayer.ActorNumber : (-1);

	public bool WasExtracted { get; set; } = false;

	public CloudCommunicator()
	{
		_client = new FusionRelayClient(this);
	}

	public override void Service()
	{
		if (_client != null)
		{
			_client.Update();
			base.Service();
		}
	}

	public unsafe override bool SendPackage(byte code, int targetActor, bool reliable, byte* buffer, int bufferLength)
	{
		Assert.Check(_client != null, "Photon Client should not be null");
		return _client.SendEvent(targetActor, code, buffer, bufferLength, reliable);
	}

	protected override void ConvertData(object data, out byte[] dataBuffer, out int maxLength)
	{
		dataBuffer = null;
		maxLength = _buffer.Length;
		_client.ExtractData(data, _buffer, ref maxLength);
		if (maxLength > 0)
		{
			dataBuffer = _buffer;
		}
	}

	public void Reset()
	{
		_client.Reset();
		_messageSendQueue.Clear();
		_recvQueue.Clear();
		_callbacks.Clear();
	}

	public void Dispose()
	{
		if (!WasExtracted)
		{
			_client = null;
		}
	}
}
