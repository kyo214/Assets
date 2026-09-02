using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
[InlineHelp]
[NetworkStructWeaved(20)]
public struct NetworkObjectHeader : INetworkStruct
{
	internal const int ACTIVE_FLAG = 1;

	public const int WORD_COUNT = 20;

	public const int REPLICATE_WORD_OFFSET = 11;

	public const int INPUT_AUTHORITY_OFFSET = 12;

	public const int STATE_AUTHORITY_OFFSET = 13;

	[FieldOffset(0)]
	public NetworkId Id;

	[FieldOffset(4)]
	public NetworkPrefabId Type;

	[FieldOffset(8)]
	[InlineHelp]
	public NetworkId NestingRoot;

	[FieldOffset(12)]
	public NetworkObjectNestingKey NestingKey;

	[FieldOffset(16)]
	public Guid SceneGuid;

	[FieldOffset(32)]
	public int WordCount;

	[FieldOffset(36)]
	public int TransformOffset;

	[FieldOffset(40)]
	public NetworkObjectHeaderFlags Flags;

	[FieldOffset(44)]
	public NetworkObjectPredictionKey PredictionKey;

	[FieldOffset(48)]
	public PlayerRef InputAuthority;

	[FieldOffset(52)]
	public PlayerRef StateAuthority;

	[FieldOffset(56)]
	public int AreaOfInterestLayerMask;

	[FieldOffset(60)]
	private unsafe fixed int _reserved[5];

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		stringBuilder.Append("Id").Append(": ").Append(Id.ToString());
		if (Type.IsValid)
		{
			stringBuilder.Append(", ").Append("Type").Append(": ")
				.Append(Type.ToString());
		}
		if (NestingRoot.IsValid)
		{
			stringBuilder.Append(", ").Append("NestingRoot").Append(": ")
				.Append(NestingRoot.ToString());
		}
		if (NestingKey.IsValid)
		{
			stringBuilder.Append(", ").Append("NestingKey").Append(": ")
				.Append(NestingKey.ToString());
		}
		if (SceneGuid != default(Guid))
		{
			stringBuilder.Append(", ").Append("SceneGuid").Append(": ")
				.Append(SceneGuid.ToString());
		}
		if (WordCount != 0)
		{
			stringBuilder.Append(", ").Append("WordCount").Append(": ")
				.Append(WordCount);
		}
		if (TransformOffset != 0)
		{
			stringBuilder.Append(", ").Append("TransformOffset").Append(": ")
				.Append(TransformOffset.ToString());
		}
		if (Flags != 0)
		{
			stringBuilder.Append(", ").Append("Flags").Append(": ")
				.Append(Flags.ToString());
		}
		if (PredictionKey != default(NetworkObjectPredictionKey))
		{
			stringBuilder.Append(", ").Append("PredictionKey").Append(": ")
				.Append(PredictionKey.ToString());
		}
		if (InputAuthority.IsValid)
		{
			stringBuilder.Append(", ").Append("InputAuthority").Append(": ")
				.Append(InputAuthority.ToString());
		}
		if (StateAuthority.IsValid)
		{
			stringBuilder.Append(", ").Append("StateAuthority").Append(": ")
				.Append(StateAuthority.ToString());
		}
		if (AreaOfInterestLayerMask != 0)
		{
			stringBuilder.Append(", ").Append("AreaOfInterestLayerMask").Append(": ")
				.Append(AreaOfInterestLayerMask);
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
