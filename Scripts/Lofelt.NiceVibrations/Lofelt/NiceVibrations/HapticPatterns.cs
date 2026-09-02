using System.Globalization;
using System.Text;
using UnityEngine;

namespace Lofelt.NiceVibrations;

public static class HapticPatterns
{
	public enum PresetType
	{
		Selection = 0,
		Success = 1,
		Warning = 2,
		Failure = 3,
		LightImpact = 4,
		MediumImpact = 5,
		HeavyImpact = 6,
		RigidImpact = 7,
		SoftImpact = 8,
		None = -1
	}

	private struct Pattern
	{
		public float[] time;

		public float[] amplitude;

		private static string clipJsonTemplate;

		static Pattern()
		{
			clipJsonTemplate = (Resources.Load("nv-pattern-template") as TextAsset).text;
		}

		public Pattern(float[] time, float[] amplitude)
		{
			this.time = time;
			this.amplitude = amplitude;
		}

		public GamepadRumble ToRumble()
		{
			GamepadRumble result = default;
			if (time.Length <= 1)
			{
				return result;
			}
			int num = time.Length - 1;
			result.durationsMs = new int[num];
			result.lowFrequencyMotorSpeeds = new float[num];
			result.highFrequencyMotorSpeeds = new float[num];
			result.totalDurationMs = 0;
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)((time[i + 1] - time[i]) * 1000f);
				result.durationsMs[i] = num2;
				result.lowFrequencyMotorSpeeds[i] = amplitude[i];
				result.highFrequencyMotorSpeeds[i] = amplitude[i];
				result.totalDurationMs += result.durationsMs[i];
			}
			return result;
		}

		public string ToClip()
		{
			if (clipJsonTemplate == null)
			{
				return "";
			}
			string text = "";
			for (int i = 0; i < time.Length; i++)
			{
				float num = Mathf.Clamp(amplitude[i], 0f, 1f);
				text = text + "{ \"time\":" + time[i].ToString(numberFormat) + ",\"amplitude\":" + num.ToString(numberFormat) + "}";
				if (i + 1 < time.Length)
				{
					text += ",";
				}
			}
			return clipJsonTemplate.Replace("{amplitude-envelope}", text);
		}
	}

	private struct Preset
	{
		public PresetType type;

		public float[] maximumAmplitudePattern;

		public byte[] jsonClip;

		public GamepadRumble gamepadRumble;

		public Preset(PresetType type, float[] time, float[] amplitude)
		{
			Pattern pattern = new Pattern(time, amplitude);
			this.type = type;
			maximumAmplitudePattern = pattern.time;
			gamepadRumble = pattern.ToRumble();
			jsonClip = Encoding.UTF8.GetBytes(pattern.ToClip());
		}

		public float GetDuration()
		{
			if (maximumAmplitudePattern.Length != 0)
			{
				return maximumAmplitudePattern[maximumAmplitudePattern.Length - 1];
			}
			return 0f;
		}
	}

	private static string emphasisTemplate;

	private static string constantTemplate;

	private static NumberFormatInfo numberFormat;

	private static float[] constantPatternTime;

	private static Preset Selection;

	private static Preset Light;

	private static Preset Medium;

	private static Preset Heavy;

	private static Preset Rigid;

	private static Preset Soft;

	private static Preset Success;

	private static Preset Failure;

	private static Preset Warning;

	static HapticPatterns()
	{
		constantPatternTime = new float[2];
		emphasisTemplate = (Resources.Load("nv-emphasis-template") as TextAsset).text;
		constantTemplate = (Resources.Load("nv-constant-template") as TextAsset).text;
		numberFormat = new NumberFormatInfo();
		numberFormat.NumberDecimalSeparator = ".";
		Selection = new Preset(PresetType.Selection, new float[2] { 0f, 0.04f }, new float[2] { 0.471f, 0.471f });
		Light = new Preset(PresetType.LightImpact, new float[2] { 0f, 0.04f }, new float[2] { 0.156f, 0.156f });
		Medium = new Preset(PresetType.MediumImpact, new float[2] { 0f, 0.08f }, new float[2] { 0.471f, 0.471f });
		Heavy = new Preset(PresetType.HeavyImpact, new float[2] { 0f, 0.16f }, new float[2] { 1f, 1f });
		Rigid = new Preset(PresetType.RigidImpact, new float[2] { 0f, 0.04f }, new float[2] { 1f, 1f });
		Soft = new Preset(PresetType.SoftImpact, new float[2] { 0f, 0.16f }, new float[2] { 0.156f, 0.156f });
		Success = new Preset(PresetType.Success, new float[4] { 0f, 0.04f, 0.08f, 0.24f }, new float[4] { 0f, 0.157f, 0f, 1f });
		Failure = new Preset(PresetType.Failure, new float[8] { 0f, 0.08f, 0.12f, 0.2f, 0.24f, 0.4f, 0.44f, 0.48f }, new float[8] { 0f, 0.47f, 0f, 0.47f, 0f, 1f, 0f, 0.157f });
		Warning = new Preset(PresetType.Warning, new float[4] { 0f, 0.12f, 0.24f, 0.28f }, new float[4] { 0f, 1f, 0f, 0.47f });
	}

	public static void PlayEmphasis(float amplitude, float frequency)
	{
		if (emphasisTemplate != null && HapticController.hapticsEnabled)
		{
			if (HapticController.Init() || GamepadRumbler.IsConnected())
			{
				float num = Mathf.Clamp(amplitude, 0f, 1f);
				float num2 = Mathf.Clamp(frequency, 0f, 1f);
				string s = emphasisTemplate.Replace("{amplitude}", num.ToString(numberFormat)).Replace("{frequency}", num2.ToString(numberFormat)).Replace("{duration}", 0.1f.ToString(numberFormat));
				HapticController.Load(rumble: new GamepadRumble
				{
					durationsMs = new int[1] { 100 },
					lowFrequencyMotorSpeeds = new float[1] { num },
					highFrequencyMotorSpeeds = new float[1] { num2 }
				}, json: Encoding.UTF8.GetBytes(s));
				HapticController.Loop(enabled: false);
				HapticController.Play();
			}
			else
			{
				_ = DeviceCapabilities.isVersionSupported;
			}
		}
	}

	private static PresetType presetTypeForEmphasis(float amplitude)
	{
		if (amplitude > 0.5f)
		{
			return PresetType.HeavyImpact;
		}
		if (amplitude <= 0.5f && (double)amplitude > 0.3)
		{
			return PresetType.MediumImpact;
		}
		return PresetType.LightImpact;
	}

	public static void PlayConstant(float amplitude, float frequency, float duration)
	{
		if (constantTemplate != null && HapticController.hapticsEnabled)
		{
			float clipLevel = Mathf.Clamp(amplitude, 0f, 1f);
			float clipFrequencyShift = Mathf.Clamp(frequency, 0f, 1f);
			float num = Mathf.Max(duration, 0f);
			string s = constantTemplate.Replace("{duration}", num.ToString(numberFormat));
			GamepadRumble rumble = default;
			int num2 = (int)(num * 1000f) / 16;
			rumble.durationsMs = new int[num2];
			rumble.lowFrequencyMotorSpeeds = new float[num2];
			rumble.highFrequencyMotorSpeeds = new float[num2];
			for (int i = 0; i < num2; i++)
			{
				rumble.durationsMs[i] = 16;
				rumble.lowFrequencyMotorSpeeds[i] = 1f;
				rumble.highFrequencyMotorSpeeds[i] = 1f;
			}
			if (HapticController.Init() || GamepadRumbler.IsConnected())
			{
				HapticController.Load(Encoding.UTF8.GetBytes(s), rumble);
				HapticController.Loop(enabled: false);
				HapticController.clipLevel = clipLevel;
				HapticController.clipFrequencyShift = clipFrequencyShift;
				HapticController.Play();
			}
			else
			{
				_ = DeviceCapabilities.isVersionSupported;
			}
		}
	}

	private static Preset GetPresetForType(PresetType type)
	{
		return type switch
		{
			PresetType.Selection => Selection, 
			PresetType.LightImpact => Light, 
			PresetType.MediumImpact => Medium, 
			PresetType.HeavyImpact => Heavy, 
			PresetType.RigidImpact => Rigid, 
			PresetType.SoftImpact => Soft, 
			PresetType.Success => Success, 
			PresetType.Failure => Failure, 
			PresetType.Warning => Warning, 
			_ => Medium, 
		};
	}

	public static void PlayPreset(PresetType presetType)
	{
		if (HapticController.hapticsEnabled && presetType != PresetType.None)
		{
			Preset presetForType = GetPresetForType(presetType);
			if (HapticController.Init() || GamepadRumbler.IsConnected())
			{
				HapticController.Load(presetForType.jsonClip, presetForType.gamepadRumble);
				HapticController.Loop(enabled: false);
				HapticController.Play();
			}
			else
			{
				_ = DeviceCapabilities.isVersionSupported;
			}
		}
	}

	public static float GetPresetDuration(PresetType presetType)
	{
		if (presetType == PresetType.None)
		{
			return 0f;
		}
		return GetPresetForType(presetType).GetDuration();
	}
}
