#define DEBUG
using System.Collections.Generic;
using System.Diagnostics;

namespace Fusion.Profiling;

public class FusionSamplers
{
	internal int CurrentStage;

	public Dictionary<int, Dictionary<string, FusionSampler>> PerConnectionSamplers;

	public Dictionary<string, FusionSampler> GeneralSamplers;

	private List<FusionSampler> _allSamplers = new List<FusionSampler>();

	private int _currentFrameId = -1;

	private int _maxFrames = -1;

	internal Timer Timer;

	public int FrameCount => (_maxFrames <= 0) ? (_currentFrameId + 1) : ((_currentFrameId < _maxFrames) ? (_currentFrameId + 1) : _maxFrames);

	[Conditional("Debug")]
	internal void SetCurrentStage(int stage)
	{
		CurrentStage = stage;
	}

	internal FusionSamplers(Timer timer, int maxFrames = -1)
	{
		Timer = timer;
		_maxFrames = maxFrames;
		GeneralSamplers = new Dictionary<string, FusionSampler>();
		PerConnectionSamplers = new Dictionary<int, Dictionary<string, FusionSampler>>();
	}

	[Conditional("DEBUG")]
	public void IncrementFrame()
	{
		_currentFrameId++;
		foreach (FusionSampler allSampler in _allSamplers)
		{
			allSampler.IncrementFrame(_currentFrameId);
		}
	}

	public FusionSampler GetSampler(int connectionId, string samplerName)
	{
		if (!PerConnectionSamplers.TryGetValue(connectionId, out var value))
		{
			value = new Dictionary<string, FusionSampler>();
			PerConnectionSamplers.Add(connectionId, value);
		}
		if (!value.TryGetValue(samplerName, out var value2))
		{
			value2 = new FusionSampler(samplerName, Timer, _currentFrameId, _maxFrames);
			value.Add(samplerName, value2);
			_allSamplers.Add(value2);
		}
		return value2;
	}

	public FusionSampler GetSampler(string samplerName)
	{
		if (!GeneralSamplers.TryGetValue(samplerName, out var value))
		{
			value = new FusionSampler(samplerName, Timer, _currentFrameId, _maxFrames);
			GeneralSamplers.Add(samplerName, value);
			_allSamplers.Add(value);
		}
		return value;
	}

	[Conditional("DEBUG")]
	public void Add(string samplerName, string sampleTag, int tick, double value, SampleSegmentFlag segmentFlag = SampleSegmentFlag.None)
	{
		FusionSampler sampler = GetSampler(samplerName);
		sampler.Add(new Sample
		{
			Name = sampleTag,
			Time = new SampleTime(Timer.ElapsedInSeconds, tick, CurrentStage),
			Data = new SampleData(value, segmentFlag)
		});
	}

	[Conditional("DEBUG")]
	public void Add(int connId, string samplerName, string sampleTag, int tick, double value, SampleSegmentFlag segmentFlag = SampleSegmentFlag.None)
	{
		FusionSampler sampler = GetSampler(connId, samplerName);
		sampler.Add(new Sample
		{
			Name = sampleTag,
			Time = new SampleTime(Timer.ElapsedInSeconds, tick, CurrentStage),
			Data = new SampleData(value, segmentFlag)
		});
	}
}
