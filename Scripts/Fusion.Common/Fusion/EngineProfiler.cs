#define ENABLE_PROFILER
using System;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace Fusion;

public static class EngineProfiler
{
	public static Action<float> InterpolationOffsetCallback;

	public static Action<float> InterpolationTimeScaleCallback;

	public static Action<float> InterpolationMultiplierCallback;

	public static Action<float> InterpolationUncertaintyCallback;

	public static Action<int> WorldSnapshotSizeCallback;

	public static Action<int> ResimulationsCallback;

	public static Action<int> InputSizeCallback;

	public static Action<int> InputQueueCallback;

	public static Action<int> RpcInCallback;

	public static Action<int> RpcOutCallback;

	public static Action<float> RoundTripTimeCallback;

	public static Action<float> SimualtionTimeScaleCallback;

	public static Action<float> InputOffsetCallback;

	public static Action<float> InputOffsetDeviationCallback;

	public static Action<float> InputRecvDeltaCallback;

	public static Action<float> InputRecvDeltaDeviationCallback;

	[Conditional("ENABLE_PROFILER")]
	public static void Begin(string sample)
	{
		Profiler.BeginSample(sample);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void End()
	{
		Profiler.EndSample();
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InterpolationOffset(float value)
	{
		InterpolationOffsetCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InterpolationMultiplier(float value)
	{
		InterpolationMultiplierCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InterpolationTimeScale(float value)
	{
		InterpolationTimeScaleCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InterpolationUncertainty(float value)
	{
		InterpolationUncertaintyCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void Resimulations(int value)
	{
		ResimulationsCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void WorldSnapshotSize(int value)
	{
		WorldSnapshotSizeCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void RoundTripTime(float value)
	{
		RoundTripTimeCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InputSize(int value)
	{
		InputSizeCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InputQueue(int value)
	{
		InputQueueCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void RpcIn(int value)
	{
		RpcInCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void RpcOut(int value)
	{
		RpcOutCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void SimulationTimeScale(float value)
	{
		SimualtionTimeScaleCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InputOffset(float value)
	{
		InputOffsetCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InputOffsetDeviation(float value)
	{
		InputOffsetDeviationCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InputRecvDelta(float value)
	{
		InputRecvDeltaCallback?.Invoke(value);
	}

	[Conditional("ENABLE_PROFILER")]
	public static void InputRecvDeltaDeviation(float value)
	{
		InputRecvDeltaDeviationCallback?.Invoke(value);
	}
}
