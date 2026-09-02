using System;

namespace Fusion.Protocol;

internal interface ICommunicator
{
	int CommunicatorID { get; }

	unsafe bool SendPackage(byte code, int targetActor, bool reliable, byte* buffer, int bufferLength);

	unsafe int ReceivePackage(out int senderActor, byte* buffer, int bufferLength);

	bool Poll();

	void PushPackage(int senderActor, int eventCode, object data);

	void RegisterPackageCallback<K>(Action<int, K> callback) where K : IMessage;

	void SendMessage(int targetActor, IMessage message);

	void Service();
}
