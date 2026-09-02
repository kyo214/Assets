using System;
using Collections.Unsafe;

namespace Fusion.Sockets;

public struct NetConnectionStats : IStatsBuffer
{
	public struct Entry : ISampleData
	{
		public double Time;

		public double Value;

		public int TickValue => 0;

		public float TimeValue => (float)Time;

		public float FloatValue => (float)Value;
	}

	private unsafe UnsafeRingBuffer* _buffer;

	public unsafe int Count => (_buffer != null) ? UnsafeRingBuffer.Count(_buffer) : (-1);

	public unsafe int Capacity => (_buffer != null) ? UnsafeRingBuffer.Capacity(_buffer) : (-1);

	public unsafe Entry this[int index] => UnsafeRingBuffer.Get<Entry>(_buffer, index);

	public double MinTime => (Count > 0) ? this[0].Time : 0.0;

	public double MaxTime => (Count > 0) ? this[Count - 1].Time : 0.0;

	public FusionGraphVisualization DefaultVisualization => FusionGraphVisualization.IntermittentTime;

	public FusionGraphVisualization VisualizationFlags => FusionGraphVisualization.IntermittentTime | FusionGraphVisualization.ValueHistogram;

	public unsafe void Clear()
	{
		UnsafeRingBuffer.Clear(_buffer);
	}

	public unsafe void Free()
	{
		if (_buffer != null)
		{
			UnsafeRingBuffer* buffer = _buffer;
			_buffer = null;
			UnsafeRingBuffer.Free(buffer);
		}
	}

	public unsafe NetConnectionStats(int capacity)
	{
		_buffer = UnsafeRingBuffer.Allocate<Entry>(capacity, overwrite: true);
	}

	public unsafe UnsafeList.Iterator<Entry> GetIterator()
	{
		return UnsafeRingBuffer.GetIterator<Entry>(_buffer);
	}

	public unsafe void Push(double time, double value)
	{
		UnsafeRingBuffer.Push(_buffer, new Entry
		{
			Time = time,
			Value = value
		});
	}

	public bool CalculateAvgDevMinMax(out double avg, out double dev, out double min, out double max)
	{
		dev = 0.0;
		avg = 0.0;
		if (Count == 0)
		{
			min = 0.0;
			max = 0.0;
			return false;
		}
		min = double.MaxValue;
		max = double.MinValue;
		for (int i = 0; i < Count; i++)
		{
			avg += this[i].Value;
			min = Math.Min(min, this[i].Value);
			max = Math.Max(max, this[i].Value);
		}
		avg /= Count;
		for (int j = 0; j < Count; j++)
		{
			dev += Math.Pow(this[j].Value - avg, 2.0);
		}
		dev = Math.Sqrt(dev / (double)(Count - 1));
		return true;
	}

	public ISampleData GetSampleAtIndex(int index)
	{
		return this[index];
	}
}
