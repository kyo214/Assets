using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[MMRequiresConstantRepaint]
[AddComponentMenu("More Mountains/Tools/Property Controllers/ShaderController")]
public class ShaderController : MMMonoBehaviour
{
	public enum TargetTypes
	{
		Renderer = 0,
		Image = 1,
		RawImage = 2,
		Text = 3
	}

	public enum PropertyTypes
	{
		Bool = 0,
		Float = 1,
		Int = 2,
		Vector = 3,
		Keyword = 4,
		Color = 5
	}

	public enum ControlModes
	{
		PingPong = 0,
		Random = 1,
		OneTime = 2,
		AudioAnalyzer = 3,
		ToDestination = 4,
		Driven = 5
	}

	[Header("Target")]
	public TargetTypes TargetType;

	[MMEnumCondition("TargetType", new int[] { 0 })]
	public Renderer TargetRenderer;

	[MMEnumCondition("TargetType", new int[] { 0 })]
	public int TargetMaterialID;

	[MMEnumCondition("TargetType", new int[] { 1 })]
	public Image TargetImage;

	[MMEnumCondition("TargetType", new int[] { 1 })]
	public bool UseMaterialForRendering;

	[MMEnumCondition("TargetType", new int[] { 2 })]
	public RawImage TargetRawImage;

	[MMEnumCondition("TargetType", new int[] { 3 })]
	public Text TargetText;

	public bool CacheMaterial = true;

	public bool CreateMaterialInstance;

	public string TargetPropertyName;

	public PropertyTypes PropertyType = PropertyTypes.Float;

	[MMEnumCondition("PropertyType", new int[] { 3 })]
	public bool X;

	[MMEnumCondition("PropertyType", new int[] { 3 })]
	public bool Y;

	[MMEnumCondition("PropertyType", new int[] { 3 })]
	public bool Z;

	[MMEnumCondition("PropertyType", new int[] { 3 })]
	public bool W;

	[Header("Color")]
	[ColorUsage(true, true)]
	public Color FromColor = Color.black;

	[ColorUsage(true, true)]
	public Color ToColor = Color.white;

	[Header("Global Settings")]
	public ControlModes ControlMode;

	public bool AddToInitialValue;

	public bool UseUnscaledTime = true;

	public bool RevertToInitialValueAfterEnd = true;

	[Tooltip("if this is true, this component will use material property blocks instead of working on an instance of the material.")]
	[MMEnumCondition("TargetType", new int[] { 0 })]
	public bool UseMaterialPropertyBlocks;

	public bool SafeMode;

	[Header("Ping Pong")]
	public MMTweenType Curve;

	public float MinValue;

	public float MaxValue = 5f;

	public float Duration = 1f;

	public float PingPongPauseDuration = 1f;

	[Header("Driven")]
	public float DrivenLevel;

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

	[MMInspectorButton("OneTime")]
	public bool OneTimeButton;

	public bool DisableAfterOneTime;

	public bool DisableGameObjectAfterOneTime;

	[Header("AudioAnalyzer")]
	public MMAudioAnalyzer AudioAnalyzer;

	public int BeatID;

	public float AudioAnalyzerMultiplier = 1f;

	public float AudioAnalyzerOffset;

	public float AudioAnalyzerLerp = 60f;

	[Header("ToDestination")]
	public float ToDestinationValue = 1f;

	public float ToDestinationDuration = 1f;

	public AnimationCurve ToDestinationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.6f), new Keyframe(1f, 1f));

	[MMInspectorButton("ToDestination")]
	public bool ToDestinationButton;

	public bool DisableAfterToDestination;

	[Header("Debug")]
	[MMReadOnly]
	public float InitialValue;

	[MMReadOnly]
	public float CurrentValue;

	[MMReadOnly]
	public float CurrentValueNormalized;

	[MMReadOnly]
	public Color InitialColor;

	[MMReadOnly]
	public int PropertyID;

	[MMReadOnly]
	public bool PropertyFound;

	[MMReadOnly]
	public Material TargetMaterial;

	[HideInInspector]
	public float PingPong;

	protected float _randomAmplitude;

	protected float _randomFrequency;

	protected float _randomShift;

	protected float _elapsedTime;

	protected bool _shaking;

	protected float _startedTimestamp;

	protected float _remappedTimeSinceStart;

	protected Color _currentColor;

	protected Vector4 _vectorValue;

	protected float _pingPongDirection = 1f;

	protected float _lastPingPongPauseAt;

	protected float _initialValue;

	protected Color _fromColorStorage;

	protected bool _activeLastFrame;

	protected MaterialPropertyBlock _propertyBlock;

	public virtual bool FindShaderProperty(string propertyName)
	{
		if (TargetType == TargetTypes.Renderer)
		{
			if (CreateMaterialInstance)
			{
				TargetRenderer.materials[TargetMaterialID] = new Material(TargetRenderer.materials[TargetMaterialID]);
			}
			TargetMaterial = (UseMaterialPropertyBlocks ? TargetRenderer.sharedMaterials[TargetMaterialID] : TargetRenderer.materials[TargetMaterialID]);
		}
		else if (TargetType == TargetTypes.Image)
		{
			if (CreateMaterialInstance)
			{
				TargetImage.material = new Material(TargetImage.material);
			}
			TargetMaterial = TargetImage.material;
		}
		else if (TargetType == TargetTypes.RawImage)
		{
			if (CreateMaterialInstance)
			{
				TargetRawImage.material = new Material(TargetRawImage.material);
			}
			TargetMaterial = TargetRawImage.material;
		}
		else if (TargetType == TargetTypes.Text)
		{
			if (CreateMaterialInstance)
			{
				TargetText.material = new Material(TargetText.material);
			}
			TargetMaterial = TargetText.material;
		}
		if (PropertyType == PropertyTypes.Keyword)
		{
			PropertyFound = true;
			return true;
		}
		if (TargetMaterial.HasProperty(propertyName))
		{
			PropertyID = Shader.PropertyToID(propertyName);
			PropertyFound = true;
			return true;
		}
		return false;
	}

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void OnEnable()
	{
		InitialValue = GetInitialValue();
		if (PropertyType == PropertyTypes.Color)
		{
			InitialColor = TargetMaterial.GetColor(PropertyID);
		}
	}

	protected virtual bool RendererIsNull()
	{
		if (TargetType == TargetTypes.Renderer && TargetRenderer == null)
		{
			return true;
		}
		if (TargetType == TargetTypes.Image && TargetImage == null)
		{
			return true;
		}
		if (TargetType == TargetTypes.RawImage && TargetRawImage == null)
		{
			return true;
		}
		if (TargetType == TargetTypes.Text && TargetText == null)
		{
			return true;
		}
		return false;
	}

	public virtual void Initialization()
	{
		if (RendererIsNull() || string.IsNullOrEmpty(TargetPropertyName))
		{
			return;
		}
		if (TargetType != TargetTypes.Renderer)
		{
			UseMaterialPropertyBlocks = false;
		}
		PropertyFound = FindShaderProperty(TargetPropertyName);
		if (PropertyFound)
		{
			_elapsedTime = 0f;
			_randomAmplitude = Random.Range(Amplitude.x, Amplitude.y);
			_randomFrequency = Random.Range(Frequency.x, Frequency.y);
			_randomShift = Random.Range(Shift.x, Shift.y);
			if (TargetType == TargetTypes.Renderer && UseMaterialPropertyBlocks)
			{
				_propertyBlock = new MaterialPropertyBlock();
				TargetRenderer.GetPropertyBlock(_propertyBlock, TargetMaterialID);
			}
			InitialValue = GetInitialValue();
			if (PropertyType == PropertyTypes.Color)
			{
				InitialColor = TargetMaterial.GetColor(PropertyID);
			}
			_shaking = false;
			if (ControlMode == ControlModes.OneTime)
			{
				base.enabled = false;
			}
		}
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
		if (!CacheMaterial)
		{
			Initialization();
		}
		if (!RendererIsNull() && PropertyFound)
		{
			base.gameObject.SetActive(value: true);
			base.enabled = true;
			ControlMode = ControlModes.OneTime;
			_startedTimestamp = GetTime();
			_shaking = true;
		}
	}

	public virtual void ToDestination()
	{
		if (!CacheMaterial)
		{
			Initialization();
		}
		if (!RendererIsNull() && PropertyFound)
		{
			base.enabled = true;
			if (PropertyType == PropertyTypes.Color)
			{
				_fromColorStorage = FromColor;
				FromColor = TargetMaterial.GetColor(PropertyID);
			}
			ControlMode = ControlModes.ToDestination;
			_startedTimestamp = GetTime();
			_shaking = true;
			_initialValue = GetInitialValue();
		}
	}

	public void SetFromColor(Color newColor)
	{
		FromColor = newColor;
	}

	public void SetToColor(Color newColor)
	{
		ToColor = newColor;
	}

	public virtual void SetRemapOneTimeMin(float newValue)
	{
		OneTimeRemapMin = newValue;
	}

	public virtual void SetRemapOneTimeMax(float newValue)
	{
		OneTimeRemapMax = newValue;
	}

	public virtual void SetToDestinationValue(float newValue)
	{
		ToDestinationValue = newValue;
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
		UpdateValue();
	}

	protected virtual void OnDisable()
	{
		if (RevertToInitialValueAfterEnd)
		{
			CurrentValue = InitialValue;
			_currentColor = InitialColor;
			SetValue(CurrentValue);
		}
	}

	protected virtual void UpdateValue()
	{
		if (SafeMode && (RendererIsNull() || !PropertyFound))
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
			_remappedTimeSinceStart = MMMaths.Remap(GetTime() - _startedTimestamp, 0f, OneTimeDuration, 0f, 1f);
			CurrentValueNormalized = OneTimeCurve.Evaluate(_remappedTimeSinceStart);
			CurrentValue = MMMaths.Remap(CurrentValueNormalized, 0f, 1f, OneTimeRemapMin, OneTimeRemapMax);
			CurrentValue *= OneTimeAmplitude;
			break;
		case ControlModes.AudioAnalyzer:
			CurrentValue = Mathf.Lerp(CurrentValue, AudioAnalyzer.Beats[BeatID].CurrentValue * AudioAnalyzerMultiplier + AudioAnalyzerOffset, AudioAnalyzerLerp * GetDeltaTime());
			CurrentValueNormalized = Mathf.Clamp(AudioAnalyzer.Beats[BeatID].CurrentValue, 0f, 1f);
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
			_remappedTimeSinceStart = MMMaths.Remap(GetTime() - _startedTimestamp, 0f, ToDestinationDuration, 0f, 1f);
			float t = ToDestinationCurve.Evaluate(_remappedTimeSinceStart);
			CurrentValue = Mathf.LerpUnclamped(_initialValue, ToDestinationValue, t);
			CurrentValueNormalized = MMMaths.Remap(CurrentValue, _initialValue, ToDestinationValue, 0f, 1f);
			break;
		}
		}
		if (PropertyType == PropertyTypes.Color)
		{
			_currentColor = Color.Lerp(FromColor, ToColor, CurrentValue);
		}
		if (AddToInitialValue)
		{
			CurrentValue += InitialValue;
		}
		if (ControlMode == ControlModes.OneTime && _shaking && GetTime() - _startedTimestamp > OneTimeDuration)
		{
			_shaking = false;
			if (RevertToInitialValueAfterEnd)
			{
				CurrentValue = InitialValue;
				if (PropertyType == PropertyTypes.Color)
				{
					_currentColor = InitialColor;
				}
			}
			else
			{
				CurrentValue = OneTimeCurve.Evaluate(1f);
				CurrentValue = MMMaths.Remap(CurrentValue, 0f, 1f, OneTimeRemapMin, OneTimeRemapMax);
				CurrentValue *= OneTimeAmplitude;
			}
			SetValue(CurrentValue);
			if (DisableAfterOneTime)
			{
				base.enabled = false;
			}
			if (DisableGameObjectAfterOneTime)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (ControlMode == ControlModes.ToDestination && _shaking && GetTime() - _startedTimestamp > ToDestinationDuration)
		{
			_shaking = false;
			FromColor = _fromColorStorage;
			if (RevertToInitialValueAfterEnd)
			{
				CurrentValue = InitialValue;
				if (PropertyType == PropertyTypes.Color)
				{
					_currentColor = InitialColor;
				}
			}
			else
			{
				CurrentValue = ToDestinationValue;
			}
			SetValue(CurrentValue);
			if (DisableAfterToDestination)
			{
				base.enabled = false;
			}
		}
		else
		{
			SetValue(CurrentValue);
		}
	}

	protected virtual float GetInitialValue()
	{
		if (TargetMaterial == null)
		{
			Debug.LogWarning("Material is null", this);
			return 0f;
		}
		switch (PropertyType)
		{
		case PropertyTypes.Bool:
			return TargetMaterial.GetInt(PropertyID);
		case PropertyTypes.Int:
			return TargetMaterial.GetInt(PropertyID);
		case PropertyTypes.Float:
			return TargetMaterial.GetFloat(PropertyID);
		case PropertyTypes.Vector:
			return TargetMaterial.GetVector(PropertyID).x;
		case PropertyTypes.Keyword:
			if (!TargetMaterial.IsKeywordEnabled(TargetPropertyName))
			{
				return 0f;
			}
			return 1f;
		case PropertyTypes.Color:
			if (ControlMode != ControlModes.ToDestination)
			{
				InitialColor = TargetMaterial.GetColor(PropertyID);
			}
			return 0f;
		default:
			return 0f;
		}
	}

	protected virtual void SetValue(float newValue)
	{
		if (TargetType == TargetTypes.Image && UseMaterialForRendering)
		{
			if (SafeMode && TargetImage == null)
			{
				return;
			}
			TargetMaterial = TargetImage.materialForRendering;
		}
		switch (PropertyType)
		{
		case PropertyTypes.Bool:
		{
			newValue = ((newValue > 0f) ? 1f : 0f);
			int value = Mathf.RoundToInt(newValue);
			if (UseMaterialPropertyBlocks)
			{
				TargetRenderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetInt(PropertyID, value);
				TargetRenderer.SetPropertyBlock(_propertyBlock, TargetMaterialID);
			}
			else
			{
				TargetMaterial.SetInt(PropertyID, value);
			}
			break;
		}
		case PropertyTypes.Keyword:
			newValue = ((newValue > 0f) ? 1f : 0f);
			if (newValue == 0f)
			{
				TargetMaterial.DisableKeyword(TargetPropertyName);
			}
			else
			{
				TargetMaterial.EnableKeyword(TargetPropertyName);
			}
			break;
		case PropertyTypes.Int:
		{
			int value2 = Mathf.RoundToInt(newValue);
			if (UseMaterialPropertyBlocks)
			{
				TargetRenderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetInt(PropertyID, value2);
				TargetRenderer.SetPropertyBlock(_propertyBlock, TargetMaterialID);
			}
			else
			{
				TargetMaterial.SetInt(PropertyID, value2);
			}
			break;
		}
		case PropertyTypes.Float:
			if (UseMaterialPropertyBlocks)
			{
				TargetRenderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetFloat(PropertyID, newValue);
				TargetRenderer.SetPropertyBlock(_propertyBlock, TargetMaterialID);
			}
			else
			{
				TargetMaterial.SetFloat(PropertyID, newValue);
			}
			break;
		case PropertyTypes.Vector:
			_vectorValue = TargetMaterial.GetVector(PropertyID);
			if (X)
			{
				_vectorValue.x = newValue;
			}
			if (Y)
			{
				_vectorValue.y = newValue;
			}
			if (Z)
			{
				_vectorValue.z = newValue;
			}
			if (W)
			{
				_vectorValue.w = newValue;
			}
			if (UseMaterialPropertyBlocks)
			{
				TargetRenderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetVector(PropertyID, _vectorValue);
				TargetRenderer.SetPropertyBlock(_propertyBlock, TargetMaterialID);
			}
			else
			{
				TargetMaterial.SetVector(PropertyID, _vectorValue);
			}
			break;
		case PropertyTypes.Color:
			if (UseMaterialPropertyBlocks)
			{
				TargetRenderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetColor(PropertyID, _currentColor);
				TargetRenderer.SetPropertyBlock(_propertyBlock, TargetMaterialID);
			}
			else
			{
				TargetMaterial.SetColor(PropertyID, _currentColor);
			}
			break;
		}
	}

	public virtual void Stop()
	{
		_shaking = false;
		base.enabled = false;
	}
}
