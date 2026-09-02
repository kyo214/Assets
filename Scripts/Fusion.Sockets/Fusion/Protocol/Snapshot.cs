#define DEBUG
using System;

namespace Fusion.Protocol;

internal class Snapshot : Message
{
	public int Tick { get; private set; }

	public uint NetworkID { get; private set; }

	public SnapshotType SnapshotType { get; private set; }

	public int TotalSize { get; private set; }

	public bool IsValid => CRC == ComputeCRC(Data);

	public byte[] Data { get; private set; }

	public ulong CRC { get; private set; }

	public byte[] SnapshotBuffer { get; private set; }

	public int SnapshotSize { get; private set; }

	public Snapshot()
	{
	}

	public Snapshot(int tick, uint networkID, SnapshotType snapshotType, int snapshotSize, byte[] data, ProtocolMessageVersion protocolVersion = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null)
		: base(protocolVersion, serializationVersion)
	{
		Tick = tick;
		NetworkID = networkID;
		SnapshotType = snapshotType;
		SnapshotBuffer = null;
		SnapshotSize = snapshotSize;
		SetData(data);
	}

	protected override void SerializeProtected(BitStream stream)
	{
		int value = Tick;
		uint value2 = NetworkID;
		byte value3 = (byte)SnapshotType;
		ulong value4 = CRC;
		int value5 = SnapshotSize;
		int value6 = 0;
		byte[] array = null;
		if (stream.Writing && Data != null)
		{
			array = CompressionUtils.Compress(Data);
			value6 = array.Length;
		}
		stream.Serialize(ref value);
		stream.Serialize(ref value2);
		stream.Serialize(ref value3);
		stream.Serialize(ref value5);
		stream.Serialize(ref value4);
		stream.Serialize(ref value6);
		stream.Serialize(ref array, ref value6);
		Tick = value;
		NetworkID = value2;
		SnapshotType = (SnapshotType)value3;
		CRC = value4;
		SnapshotSize = value5;
		if (stream.Reading && Data == null && array != null)
		{
			Data = CompressionUtils.Decompress(array);
			TotalSize = Data.Length;
		}
	}

	private unsafe ulong ComputeCRC(byte[] data)
	{
		if (data == null)
		{
			return 0uL;
		}
		fixed (byte* data2 = data)
		{
			return CRC64.Compute(data2, data.Length);
		}
	}

	public unsafe void Merge(Snapshot snapshot)
	{
		Assert.Always(snapshot.IsValid, "Snapshot not valid");
		Tick = snapshot.Tick;
		NetworkID = snapshot.NetworkID;
		if (SnapshotBuffer == null)
		{
			SnapshotBuffer = new byte[SnapshotSize];
			Log.Debug($"Creating Snapshot Buffer. Total Size: {SnapshotBuffer.Length}");
		}
		fixed (byte* snapshotBuffer = SnapshotBuffer)
		{
			fixed (byte* data = snapshot.Data)
			{
				CompressionUtils.SnapshotDecompress((int*)snapshotBuffer, (int*)data, snapshot.TotalSize / 4);
			}
		}
		CRC = ComputeCRC(Data);
		Log.Debug($"Merge Snapshot: {snapshot}");
	}

	public void CompressSnapshot()
	{
		byte[] snapshotBuffer = SnapshotBuffer;
		SetData(snapshotBuffer);
	}

	private void SetData(byte[] data)
	{
		TotalSize = (data?.Length).GetValueOrDefault();
		Data = data;
		CRC = ComputeCRC(Data);
	}

	public override Message Clone()
	{
		return new Snapshot();
	}

	public override string ToString()
	{
		return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}, {7}={8}, {9}={10}, CRC={11}, {12}={13}, {14}]", "Snapshot", "Tick", Tick, "NetworkID", NetworkID, "SnapshotType", SnapshotType, "SnapshotSize", SnapshotSize, "TotalSize", TotalSize, CRC, "IsValid", IsValid, base.ToString());
	}
}
