using Fusion.Sockets;

namespace Fusion;

public static class NetworkRunnerCallbackArgs
{
	public class ConnectRequest
	{
		internal bool? Accepted;

		public NetAddress RemoteAddress { get; set; }

		public void Accept()
		{
			Accepted = true;
		}

		public void Refuse()
		{
			Accepted = false;
		}
	}
}
