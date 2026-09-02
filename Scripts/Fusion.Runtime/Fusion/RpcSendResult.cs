using System;
using System.Text;

namespace Fusion;

[Serializable]
public struct RpcSendResult
{
	public RpcSendMessageResult Result;

	public int MessageSize;

	public PlayerRefSet Receivers;

	public PlayerRefSet CulledReceivers;

	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();
		builder.Append("[");
		builder.Append(Result.ToString());
		builder.Append(", Size: ");
		builder.Append(MessageSize);
		builder.Append(", Receivers: {");
		AppendPlayers(Receivers);
		builder.Append("}, Culled: {");
		AppendPlayers(CulledReceivers);
		builder.Append("}");
		builder.Append("]");
		return builder.ToString();
		void AppendPlayers(PlayerRefSet players)
		{
			bool flag = true;
			foreach (PlayerRef item in players)
			{
				if (!flag)
				{
					builder.Append(", ");
				}
				builder.Append(item.PlayerId);
				flag = false;
			}
		}
	}
}
