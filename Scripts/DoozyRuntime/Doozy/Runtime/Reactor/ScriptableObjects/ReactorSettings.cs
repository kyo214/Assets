using System;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects;

public class ReactorSettings : SingletonRuntimeScriptableObject<ReactorSettings>
{
	private const float FPS_FREQUENCY_MODIFIER = 1.001f;

	private const int MIN_FPS = 1;

	public FPS EditorFPS = FPS.FPS_120;

	public int CustomEditorFPS = 20;

	public FPS RuntimeFPS = FPS.FPS_120;

	public int CustomRuntimeFPS = 20;

	public static int editorFPS => GetFPS(SingletonRuntimeScriptableObject<ReactorSettings>.instance.EditorFPS, Mathf.Min(1, SingletonRuntimeScriptableObject<ReactorSettings>.instance.CustomEditorFPS));

	public static int runtimeFPS => GetFPS(SingletonRuntimeScriptableObject<ReactorSettings>.instance.RuntimeFPS, Mathf.Min(1, SingletonRuntimeScriptableObject<ReactorSettings>.instance.CustomRuntimeFPS));

	public static float GetTickInterval(int fps)
	{
		return 1f / ((float)Mathf.Max(1, fps) * 1.001f);
	}

	private static int GetFPS(FPS fps, int customTickInterval)
	{
		return fps switch
		{
			FPS.FPS_120 => (int)fps, 
			FPS.FPS_90 => (int)fps, 
			FPS.FPS_60 => (int)fps, 
			FPS.FPS_30 => (int)fps, 
			FPS.FPS_24 => (int)fps, 
			FPS.CustomFPS => Mathf.Max(1, customTickInterval), 
			FPS.MaxFPS => (int)fps, 
			_ => throw new ArgumentOutOfRangeException("fps", fps, null), 
		};
	}

	public static float GetRuntimeTickInterval()
	{
		return GetTickInterval(runtimeFPS);
	}

	public static float GetEditorTickInterval()
	{
		return GetTickInterval(editorFPS);
	}

	[RestoreData("ReactorSettings")]
	public static ReactorSettings Get()
	{
		return SingletonRuntimeScriptableObject<ReactorSettings>.instance;
	}
}
