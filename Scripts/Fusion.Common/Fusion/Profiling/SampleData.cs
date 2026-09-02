using System;
using System.Runtime.InteropServices;

namespace Fusion.Profiling;

[StructLayout(LayoutKind.Explicit)]
public struct SampleData
{
	[FieldOffset(0)]
	public SampleSegmentFlag SegmentFlag;

	[FieldOffset(4)]
	public SampleDataTypes DataType;

	[FieldOffset(8)]
	public float AsFloat;

	[FieldOffset(8)]
	public int AsInt;

	[FieldOffset(8)]
	public uint AsUInt;

	[FieldOffset(8)]
	public double AsDouble;

	public SampleData(float value, SampleSegmentFlag type = SampleSegmentFlag.None)
	{
		this = default;
		AsFloat = value;
		DataType = SampleDataTypes.Float;
		SegmentFlag = type;
	}

	public SampleData(double value, SampleSegmentFlag type = SampleSegmentFlag.None)
	{
		this = default;
		AsDouble = value;
		DataType = SampleDataTypes.Double;
		SegmentFlag = type;
	}

	public SampleData(int value, SampleSegmentFlag type = SampleSegmentFlag.None)
	{
		this = default;
		AsInt = value;
		DataType = SampleDataTypes.Int;
		SegmentFlag = type;
	}

	public SampleData(uint value, SampleSegmentFlag type = SampleSegmentFlag.None)
	{
		this = default;
		AsUInt = value;
		DataType = SampleDataTypes.UInt;
		SegmentFlag = type;
	}

	public static implicit operator double(SampleData data)
	{
		return data.DataType switch
		{
			SampleDataTypes.Float => data.AsFloat, 
			SampleDataTypes.Int => data.AsInt, 
			SampleDataTypes.UInt => data.AsUInt, 
			SampleDataTypes.Double => data.AsDouble, 
			_ => throw new InvalidOperationException(), 
		};
	}

	public override string ToString()
	{
		return DataType switch
		{
			SampleDataTypes.Float => AsFloat.ToString(), 
			SampleDataTypes.Int => AsInt.ToString(), 
			SampleDataTypes.UInt => AsUInt.ToString(), 
			SampleDataTypes.Double => AsDouble.ToString(), 
			_ => base.ToString(), 
		};
	}
}
