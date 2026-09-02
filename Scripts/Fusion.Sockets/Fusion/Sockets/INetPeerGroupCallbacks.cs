namespace Fusion.Sockets;

public interface INetPeerGroupCallbacks
{
	unsafe void OnConnected(NetConnection* connection);

	unsafe void OnDisconnected(NetConnection* connection, NetDisconnectReason reason);

	unsafe void OnUnreliableData(NetConnection* connection, NetBitBuffer* buffer);

	unsafe void OnUnconnectedData(NetBitBuffer* buffer);

	unsafe void OnNotifyData(NetConnection* connection, NetBitBuffer* buffer);

	unsafe void OnNotifyLost(NetConnection* connection, NetSendEnvelope envelope);

	unsafe void OnNotifyDelivered(NetConnection* connection, NetSendEnvelope envelope);

	void OnNotifyDispose(NetSendEnvelope envelope);

	unsafe void OnReliableData(NetConnection* connection, int key, byte* data, int length);

	bool OnConnectionRequest(NetAddress remoteAddress, byte[] token);

	void OnConnectionFailed(NetAddress address, NetConnectFailedReason reason);

	unsafe void OnConnectionAttempt(NetConnection* connection, int attempts, int totalConnectAttempts);
}
