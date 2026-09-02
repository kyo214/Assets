using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Property Controllers/FloatController")]
[MMRequiresConstantRepaint]
public class FloatController : MMMonoBehaviour
{
	public enum ControlModes
	{
		PingPong = 0,
		Random = 1,
		OneTime = 2,
		AudioAnalyzer = 3,
		ToDestination = 4,
		Driven = 5
	}

	public enum AudioAnalyzerModes
	{
		Beat = 0,
		NormalizedBufferedBandLevels = 1
	}

	[Header("Target")]
	public MonoBehaviour TargetObject;

	[Header("Global Settings")]
	public ControlModes ControlMode;

	public bool AddToInitialValue;

	public bool UseUnscaledTime = true;

	public bool RevertToInitialValueAfterEnd = true;

	[Header("Driven")]
	public float DrivenLevel;

	[Header("Ping Pong")]
	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	public float MinValue;

	public float MaxValue = 5f;

	public float Duration = 1f;

	public float PingPongPauseDuration;

	[Header("Random")]
	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 Amplitude = new Vector2(0f, 5f);

	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 Frequency = new Vector2(1f, 1f);

	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 Shift = new Vector2(0f, 1f);

	public bool RemapNoiseValues;

	[MMCondition("RemapNoiseValues", true)]
	public float RemapNoiseZero;

	[MMCondition("RemapNoiseValues", true)]
	public float RemapNoiseOne = 1f;

	[Header("OneTime")]
	public float OneTimeDuration = 1f;

	public float OneTimeAmplitude = 1f;

	public float OneTimeRemapMin;

	public float OneTimeRemapMax = 1f;

	public AnimationCurve OneTimeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	public bool DisableAfterOneTime;

	public bool DisableGameObjectAfterOneTime;

	[MMInspectorButton("OneTime")]
	public bool OneTimeButton;

	[Header("ToDestination")]
	public float ToDestinationDuration = 1f;

	public float ToDestinationValue = 1f;

	public AnimationCurve ToDestinationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.6f), new Keyframe(1f, 1f));

	public bool DisableAfterToDestination;

	[MMInspectorButton("ToDestination")]
	public bool ToDestinationButton;

	[Header("AudioAnalyzer")]
	public MMAudioAnalyzer AudioAnalyzer;

	public AudioAnalyzerModes AudioAnalyzerMode;

	public int BeatID;

	public int NormalizedLevelID;

	public float AudioAnalyzerMultiplier = 1f;

	[Header("Debug")]
	[MMReadOnly]
	public float InitialValue;

	[MMReadOnly]
	public float CurrentValue;

	[MMReadOnly]
	public float CurrentValueNormalized;

	[HideInInspector]
	public float PingPong;

	[HideInInspector]
	public MonoAttribute TargetAttribute;

	[HideInInspector]
	public string[] AttributeNames;

	[HideInInspector]
	public string PropertyName;

	[HideInInspector]
	public int ChoiceIndex;

	public const string _undefinedString = "<Undefined Attribute>";

	protected List<string> _attributesNamesTempList;

	protected PropertyInfo[] _propertyReferences;

	protected FieldInfo[] _fieldReferences;

	protected bool _attributeFound;

	protected float _randomAmplitude;

	protected float _randomFrequency;

	protected float _randomShift;

	protected float _elapsedTime;

	protected bool _shaking;

	protected float _shakeStartTimestamp;

	protected float _remappedTimeSinceStart;

	protected float _pingPongDirection = 1f;

	protected float _lastPingPongPauseAt;

	protected float _initialValue;

	protected MonoBehaviour _targetObjectLastFrame;

	protected MonoAttribute _targetAttributeLastFrame;

	public virtual bool FindAttribute(string propertyName)
	{
		FieldInfo fieldInfo = null;
		PropertyInfo propertyInfo = null;
		TargetAttribute = null;
		propertyInfo = TargetObject.GetType().GetProperty(propertyName);
		if (propertyInfo == null)
		{
			fieldInfo = TargetObject.GetType().GetField(propertyName);
		}
		if (propertyInfo != null)
		{
			TargetAttribute = new MonoAttribute(TargetObject, MonoAttribute.MemberTypes.Property, propertyInfo, null, propertyName);
		}
		if (fieldInfo != null)
		{
			TargetAttribute = new MonoAttribute(TargetObject, MonoAttribute.MemberTypes.Field, null, fieldInfo, propertyName);
		}
		if (PropertyName == "<Undefined Attribute>")
		{
			Debug.LogError("FloatController " + base.name + " : you need to pick a property from the Property list");
			return false;
		}
		if (propertyInfo == null && fieldInfo == null)
		{
			Debug.LogError("FloatController " + base.name + " couldn't find any property or field named " + propertyName + " on " + TargetObject.name);
			return false;
		}
		if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Property)
		{
			TargetAttribute.MemberPropertyInfo = TargetObject.GetType().GetProperty(TargetAttribute.MemberName);
		}
		else if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Field)
		{
			TargetAttribute.MemberFieldInfo = TargetObject.GetType().GetField(TargetAttribute.MemberName);
		}
		return true;
	}

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void OnEnable()
	{
		InitialValue = GetInitialValue();
	}

	public virtual void Initialization()
	{
		_attributeFound = FindAttribute(PropertyName);
		if (_attributeFound && !(TargetObject == null) && !string.IsNullOrEmpty(TargetAttribute.MemberName))
		{
			_elapsedTime = 0f;
			_randomAmplitude = Random.Range(Amplitude.x, Amplitude.y);
			_randomFrequency = Random.Range(Frequency.x, Frequency.y);
			_randomShift = Random.Range(Shift.x, Shift.y);
			InitialValue = GetInitialValue();
			_shaking = false;
		}
	}

	protected virtual float GetInitialValue()
	{
		if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Property)
		{
			return (float)TargetAttribute.MemberPropertyInfo.GetValue(TargetObject);
		}
		if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Field)
		{
			return (float)TargetAttribute.MemberFieldInfo.GetValue(TargetObject);
		}
		return 0f;
	}

	public virtual void SetDrivenLevelAbsolute(float level)
	{
		DrivenLevel = level;
	}

	public virtual void SetDrivenLevelNormalized(float normalizedLevel, float remapZero, float remapOne)
	{
		DrivenLevel = MMMaths.Remap(normalizedLevel, 0f, 1f, remapZero, remapOne);
	}

	public virtual void OneTime()
	{
		if (!(TargetObject == null) && TargetAttribute != null)
		{
			base.gameObject.SetActive(value: true);
			base.enabled = true;
			_shakeStartTimestamp = GetTime();
			_shaking = true;
		}
	}

	public virtual void ToDestination()
	{
		if (!(TargetObject == null) && TargetAttribute != null)
		{
			base.enabled = true;
			ControlMode = ControlModes.ToDestination;
			_shakeStartTimestamp = GetTime();
			_shaking = true;
			_initialValue = GetInitialValue();
		}
	}

	protected float GetDeltaTime()
	{
		if (!UseUnscaledTime)
		{
			return Time.deltaTime;
		}
		return Time.unscaledDeltaTime;
	}

	protected float GetTime()
	{
		if (!UseUnscaledTime)
		{
			return Time.time;
		}
		return Time.unscaledTime;
	}

	protected virtual void Update()
	{
		_targetObjectLastFrame = TargetObject;
		_targetAttributeLastFrame = TargetAttribute;
		if (TargetObject == null || TargetAttribute == null || !_attributeFound)
		{
			return;
		}
		switch (ControlMode)
		{
		case ControlModes.PingPong:
			if (GetTime() - _lastPingPongPauseAt < PingPongPauseDuration)
			{
				return;
			}
			PingPong += GetDeltaTime() * _pingPongDirection;
			if (PingPong < 0f)
			{
				PingPong = 0f;
				_pingPongDirection = 0f - _pingPongDirection;
				_lastPingPongPauseAt = GetTime();
			}
			if (PingPong > Duration)
			{
				PingPong = Duration;
				_pingPongDirection = 0f - _pingPongDirection;
				_lastPingPongPauseAt = GetTime();
			}
			CurrentValue = MMTween.Tween(PingPong, 0f, Duration, MinValue, MaxValue, Curve);
			CurrentValueNormalized = MMMaths.Remap(CurrentValue, MinValue, MaxValue, 0f, 1f);
			break;
		case ControlModes.Random:
			_elapsedTime += GetDeltaTime();
			CurrentValueNormalized = Mathf.PerlinNoise(_randomFrequency * _elapsedTime, _randomShift);
			if (RemapNoiseValues)
			{
				CurrentValue = CurrentValueNormalized;
				CurrentValue = MMMaths.Remap(CurrentValue, 0f, 1f, RemapNoiseZero, RemapNoiseOne);
			}
			else
			{
				CurrentValue = (CurrentValueNormalized * 2f - 1f) * _randomAmplitude;
			}
			break;
		case ControlModes.OneTime:
			if (!_shaking)
			{
				return;
			}
			_remappedTimeSinceStart = MMMaths.Remap(GetTime() - _shakeStartTimestamp, 0f, OneTimeDuration, 0f, 1f);
			CurrentValueNormalized = OneTimeCurve.Evaluate(_remappedTimeSinceStart);
			CurrentValue = MMMaths.Remap(CurrentValueNormalized, 0f, 1f, OneTimeRemapMin, OneTimeRemapMax);
			CurrentValue *= OneTimeAmplitude;
			break;
		case ControlModes.AudioAnalyzer:
			if (AudioAnalyzerMode == AudioAnalyzerModes.Beat)
			{
				CurrentValue = AudioAnalyzer.Beats[BeatID].CurrentValue * AudioAnalyzerMultiplier;
			}
			else
			{
				CurrentValue = AudioAnalyzer.NormalizedBufferedBandLevels[NormalizedLevelID] * AudioAnalyzerMultiplier;
			}
			CurrentValueNormalized = Mathf.Clamp(CurrentValue, 0f, 1f);
			break;
		case ControlModes.Driven:
			CurrentValue = DrivenLevel;
			CurrentValueNormalized = Mathf.Clamp(CurrentValue, 0f, 1f);
			break;
		case ControlModes.ToDestination:
		{
			if (!_shaking)
			{
				return;
			}
			_remappedTimeSinceStart = MMMaths.Remap(GetTime() - _shakeStartTimestamp, 0f, ToDestinationDuration, 0f, 1f);
			float t = ToDestinationCurve.Evaluate(_remappedTimeSinceStart);
			CurrentValue = Mathf.LerpUnclamped(_initialValue, ToDestinationValue, t);
			CurrentValueNormalized = MMMaths.Remap(CurrentValue, _initialValue, ToDestinationValue, 0f, 1f);
			break;
		}
		}
		if (AddToInitialValue)
		{
			CurrentValue += InitialValue;
		}
		if (ControlMode == ControlModes.OneTime && _shaking && GetTime() - _shakeStartTimestamp > OneTimeDuration)
		{
			_shaking = false;
			if (RevertToInitialValueAfterEnd)
			{
				CurrentValue = InitialValue;
				TargetAttribute.SetValue(CurrentValue);
			}
			else
			{
				CurrentValue = OneTimeCurve.Evaluate(1f);
				CurrentValue = MMMaths.Remap(CurrentValue, 0f, 1f, OneTimeRemapMin, OneTimeRemapMax);
				CurrentValue *= OneTimeAmplitude;
				TargetAttribute.SetValue(CurrentValue);
			}
			if (DisableAfterOneTime)
			{
				base.enabled = false;
			}
			if (DisableGameObjectAfterOneTime)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (ControlMode == ControlModes.ToDestination && _shaking && GetTime() - _shakeStartTimestamp > ToDestinationDuration)
		{
			_shaking = false;
			if (RevertToInitialValueAfterEnd)
			{
				CurrentValue = InitialValue;
			}
			else
			{
				CurrentValue = ToDestinationValue;
			}
			TargetAttribute.SetValue(CurrentValue);
			if (DisableAfterOneTime)
			{
				base.enabled = false;
			}
			if (DisableGameObjectAfterOneTime)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else
		{
			TargetAttribute.SetValue(CurrentValue);
		}
	}

	protected virtual void OnValidate()
	{
		FillDropDownList();
		if (Application.isPlaying && (_targetAttributeLastFrame != TargetAttribute || _targetObjectLastFrame != TargetObject))
		{
			Initialization();
		}
	}

	protected virtual void OnDisable()
	{
		if (RevertToInitialValueAfterEnd)
		{
			CurrentValue = InitialValue;
			TargetAttribute.SetValue(CurrentValue);
		}
	}

	public virtual void Stop()
	{
		_shaking = false;
		base.enabled = false;
	}

	public virtual void FillDropDownList()
	{
		AttributeNames = new string[0];
		if (TargetObject == null)
		{
			return;
		}
		_propertyReferences = TargetObject.GetType().GetProperties();
		_attributesNamesTempList = new List<string>();
		_attributesNamesTempList.Add("<Undefined Attribute>");
		PropertyInfo[] propertyReferences = _propertyReferences;
		foreach (PropertyInfo propertyInfo in propertyReferences)
		{
			if (propertyInfo.PropertyType.Name == "Single")
			{
				_attributesNamesTempList.Add(propertyInfo.Name);
			}
		}
		_fieldReferences = TargetObject.GetType().GetFields();
		FieldInfo[] fieldReferences = _fieldReferences;
		foreach (FieldInfo fieldInfo in fieldReferences)
		{
			if (fieldInfo.FieldType.Name == "Single")
			{
				_attributesNamesTempList.Add(fieldInfo.Name);
			}
		}
		AttributeNames = _attributesNamesTempList.ToArray();
	}
}
