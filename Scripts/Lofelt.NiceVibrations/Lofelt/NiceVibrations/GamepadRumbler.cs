using System.Diagnostics;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lofelt.NiceVibrations;

public static class GamepadRumbler
{
	private static GamepadRumble loadedRumble;

	private static bool rumbleLoaded = false;

	private static System.Timers.Timer rumbleTimer = new System.Timers.Timer();

	private static int rumbleIndex = -1;

	private static long rumblePositionMs = 0L;

	private static Stopwatch playbackWatch = new Stopwatch();

	public static float lowFrequencyMotorSpeedMultiplication = 1f;

	public static float highFrequencyMotorSpeedMultiplication = 1f;

	private static int currentGamepadID = -1;

	public static void Init()
	{
		SynchronizationContext syncContext = SynchronizationContext.Current;
		rumbleTimer.Elapsed += (object obj, ElapsedEventArgs args) =>
		{
			syncContext.Post((object _) =>
			{
				ProcessNextRumble();
			}, null);
		};
	}

	public static bool CanPlay()
	{
		if (IsConnected() && rumbleLoaded)
		{
			return loadedRumble.IsValid();
		}
		return false;
	}

	private static Gamepad GetGamepad(int gamepadID)
	{
		if (gamepadID >= 0)
		{
			if (gamepadID >= Gamepad.all.Count)
			{
				return Gamepad.current;
			}
			return Gamepad.all[gamepadID];
		}
		return Gamepad.current;
	}

	public static void SetCurrentGamepad(int gamepadID)
	{
		if (gamepadID < Gamepad.all.Count)
		{
			currentGamepadID = gamepadID;
		}
	}

	public static bool IsConnected()
	{
		return GetGamepad(currentGamepadID) != null;
	}

	public static void Load(GamepadRumble rumble)
	{
		if (rumble.IsValid())
		{
			loadedRumble = rumble;
			rumbleLoaded = true;
			lowFrequencyMotorSpeedMultiplication = 1f;
			highFrequencyMotorSpeedMultiplication = 1f;
		}
		else
		{
			Unload();
		}
	}

	public static void Play()
	{
		if (CanPlay())
		{
			rumbleIndex = 0;
			rumblePositionMs = 0L;
			playbackWatch.Restart();
			ProcessNextRumble();
		}
	}

	public static void Stop()
	{
		if (GetGamepad(currentGamepadID) != null)
		{
			GetGamepad(currentGamepadID).ResetHaptics();
		}
		rumbleTimer.Enabled = false;
		rumbleIndex = -1;
		rumblePositionMs = 0L;
		playbackWatch.Stop();
	}

	public static void Unload()
	{
		loadedRumble.highFrequencyMotorSpeeds = null;
		loadedRumble.lowFrequencyMotorSpeeds = null;
		loadedRumble.durationsMs = null;
		rumbleLoaded = false;
		Stop();
	}

	private static bool IncreaseRumbleIndex()
	{
		rumblePositionMs += loadedRumble.durationsMs[rumbleIndex];
		rumbleIndex++;
		if (rumbleIndex == loadedRumble.durationsMs.Length)
		{
			Stop();
			return false;
		}
		return true;
	}

	private static void ProcessNextRumble()
	{
		if (rumbleIndex == -1)
		{
			return;
		}
		if (rumbleIndex == loadedRumble.durationsMs.Length)
		{
			Stop();
			return;
		}
		long elapsedMilliseconds = playbackWatch.ElapsedMilliseconds;
		long num = 0L;
		while (true)
		{
			long num2 = loadedRumble.durationsMs[rumbleIndex];
			long num3 = elapsedMilliseconds - rumblePositionMs;
			num = num2 - num3;
			if (num > 0)
			{
				break;
			}
			if (!IncreaseRumbleIndex())
			{
				return;
			}
		}
		float lowFrequency = loadedRumble.lowFrequencyMotorSpeeds[rumbleIndex] * Mathf.Max(lowFrequencyMotorSpeedMultiplication, 0f);
		float highFrequency = loadedRumble.highFrequencyMotorSpeeds[rumbleIndex] * Mathf.Max(highFrequencyMotorSpeedMultiplication, 0f);
		Gamepad gamepad = GetGamepad(currentGamepadID);
		if (gamepad != null)
		{
			gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
			rumblePositionMs += loadedRumble.durationsMs[rumbleIndex];
			rumbleIndex++;
			rumbleTimer.Interval = num;
			rumbleTimer.AutoReset = false;
			rumbleTimer.Enabled = true;
		}
	}
}
