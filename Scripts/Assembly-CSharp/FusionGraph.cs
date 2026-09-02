using System;
using System.Collections.Generic;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.UI;

public class FusionGraph : FusionGraphBase
{
	public enum Layouts
	{
		Auto = 0,
		FullAuto = 1,
		FullNoOverlap = 2,
		CenteredAuto = 3,
		CenteredNoGraph = 4,
		CenteredNoOverlap = 5,
		CompactAuto = 6,
		CompactNoGraph = 7
	}

	public enum ShowGraphOptions
	{
		Never = 0,
		OverlayOnly = 1,
		Always = 2
	}

	private enum ShaderType
	{
		None = 0,
		Overlay = 1,
		GameObject = 2
	}

	private const int GRPH_TOP_PAD = 36;

	private const int GRPH_BTM_PAD = 36;

	private const int HIDE_XTRAS_WDTH = 200;

	private const int INTERMITTENT_DATA_ARRAYSIZE = 128;

	private const int EXPAND_GRPH_THRESH = 112;

	private const int COMPACT_THRESH = 52;

	[SerializeField]
	[HideInInspector]
	public float Height = 50f;

	[InlineHelp]
	[SerializeField]
	[Header("Graph Layout")]
	private Layouts _layout;

	[InlineHelp]
	[SerializeField]
	private ShowGraphOptions _showGraph = ShowGraphOptions.Always;

	[InlineHelp]
	public float Padding = 5f;

	[InlineHelp]
	[SerializeField]
	private bool _alwaysExpandGraph;

	[InlineHelp]
	[SerializeField]
	private bool _showUITargets;

	[DrawIf("_showUITargets", Hide = true)]
	public Image GraphImg;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelMin;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelMax;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelAvg;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelLast;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelPer;

	[DrawIf("_showUITargets", Hide = true)]
	public Dropdown _viewDropdown;

	[DrawIf("_showUITargets", Hide = true)]
	public Button _avgBttn;

	private float _min;

	private float _max;

	private float[] _values;

	private float[] _intensity;

	private float[] _histogram;

	private List<int> DropdownLookup = new List<int>();

	[InlineHelp]
	private FusionGraphVisualization _graphVisualization;

	private ShaderType _currentShader;

	private (int tick, float value)[] _cachedValues;

	private double _lastCachedTickTime;

	private int _lastCachedTick;

	private int _histoHighestUsedBucketIndex;

	private int _histoAvgSampleCount;

	private double _histoStepInverse;

	private double _histoAvg;

	private static Shader Shader => Resources.Load<Shader>("FusionGraphShader");

	public Layouts Layout
	{
		get
		{
			return _layout;
		}
		set
		{
			_layout = value;
			CalculateLayout();
		}
	}

	public ShowGraphOptions ShowGraph
	{
		get
		{
			return _showGraph;
		}
		set
		{
			_showGraph = value;
			CalculateLayout();
			_layoutDirty = true;
		}
	}

	public bool AlwaysExpandGraph
	{
		get
		{
			return _alwaysExpandGraph;
		}
		set
		{
			_alwaysExpandGraph = value;
			CalculateLayout();
			_layoutDirty = true;
		}
	}

	public FusionGraphVisualization GraphVisualization
	{
		set
		{
			_graphVisualization = value;
			Reset();
		}
	}

	protected override bool TryConnect()
	{
		if (base.TryConnect())
		{
			FusionGraphVisualization visualizationFlags = _statsBuffer.VisualizationFlags;
			DropdownLookup.Clear();
			_viewDropdown.ClearOptions();
			for (int i = 0; i < 16; i++)
			{
				if (((uint)visualizationFlags & (uint)(1 << i)) != 0)
				{
					DropdownLookup.Add(1 << i);
					_viewDropdown.options.Add(new Dropdown.OptionData(FusionStatsUtilities.CachedTelemetryNames[i + 1]));
					if (((uint)(1 << i) & (uint)_statsBuffer.DefaultVisualization) != 0)
					{
						_viewDropdown.value = i - 1;
					}
				}
			}
			SetPerText();
			return true;
		}
		return false;
	}

	private void Reset()
	{
		_values = null;
		_histogram = null;
		_intensity = null;
		_min = 0f;
		_max = 0f;
		ResetGraphShader();
	}

	public void Clear()
	{
		if (_values != null && _values.Length != 0)
		{
			Array.Clear(_values, 0, _values.Length);
			Array.Clear(_histogram, 0, _histogram.Length);
			for (int i = 0; i < _intensity.Length; i++)
			{
				_intensity[i] = -2f;
			}
			_min = 0f;
			_max = 0f;
			_histoHighestUsedBucketIndex = 0;
			_histoAvg = 0.0;
			_histoAvgSampleCount = 0;
		}
	}

	public override void Initialize()
	{
		_viewDropdown?.onValueChanged.AddListener(OnDropdownChanged);
		_avgBttn?.onClick.AddListener(CyclePer);
	}

	public void OnDropdownChanged(int value)
	{
		GraphVisualization = (FusionGraphVisualization)DropdownLookup[value];
		SetPerText();
	}

	[BehaviourButtonAction("ResetShader", null, null)]
	private void ResetShaderButton()
	{
		_intensity = new float[200];
		_values = new float[200];
		for (int i = 0; i < _values.Length; i++)
		{
			_values[i] = (float)i / (float)_values.Length;
			_intensity[i] = (float)i / 200f;
		}
		GraphImg.material.SetFloat("_ZeroCenter", 0.3f);
		GraphImg.material.SetFloatArray("_Data", _values);
		GraphImg.material.SetFloatArray("_Intensity", _intensity);
		GraphImg.material.SetInt("_Count", _values.Length);
	}

	private void ResetGraphShader()
	{
		if ((bool)GraphImg)
		{
			ShaderType shaderType = ((LocateParentFusionStats() != null) ? ((_fusionStats.CanvasType != FusionStats.StatCanvasTypes.GameObject) ? ShaderType.Overlay : ShaderType.GameObject) : ShaderType.None);
			GraphImg.material = new Material(Shader);
			GraphImg.material.SetColor("_GoodColor", _fusionStats.GraphColorGood);
			GraphImg.material.SetColor("_WarnColor", _fusionStats.GraphColorWarn);
			GraphImg.material.SetColor("_BadColor", _fusionStats.GraphColorBad);
			GraphImg.material.SetColor("_FlagColor", _fusionStats.GraphColorFlag);
			GraphImg.material.SetInt("_ZWrite", (shaderType == ShaderType.GameObject) ? 1 : 0);
		}
	}

	public override void CyclePer()
	{
		if (_graphVisualization != FusionGraphVisualization.CountHistogram && _graphVisualization != FusionGraphVisualization.ValueHistogram)
		{
			base.CyclePer();
			SetPerText();
		}
	}

	private void SetPerText()
	{
		if (LabelPer == null)
		{
			RectTransform rt = LabelAvg.rectTransform.parent.CreateRectTransform("Per").SetAnchors(0.3f, 0.7f, 0f, 0.125f).SetOffsets(6f, -6f, 6f, 0f);
			LabelPer = rt.AddText("per sample", TextAnchor.LowerCenter, _fusionStats.FontColor);
		}
		LabelPer.text = (((_graphVisualization == FusionGraphVisualization.ValueHistogram) | (_graphVisualization == FusionGraphVisualization.CountHistogram)) ? "avg per Sample" : ((CurrentPer == Simulation.Statistics.StatsPer.Second) ? "avg per Second" : ((CurrentPer == Simulation.Statistics.StatsPer.Tick) ? "avg per Tick" : "avg per Sample")));
	}

	public override void Refresh()
	{
		if (_layoutDirty)
		{
			CalculateLayout();
		}
		IStatsBuffer data = base.StatsBuffer;
		if (data != null && data.Count >= 1)
		{
			FusionGraphVisualization fusionGraphVisualization = ((_graphVisualization == FusionGraphVisualization.Auto) ? _statsBuffer.DefaultVisualization : _graphVisualization);
			if (_values == null)
			{
				int num = fusionGraphVisualization switch
				{
					FusionGraphVisualization.ValueHistogram => StatSourceInfo.HistoBucketCount + 3, 
					FusionGraphVisualization.ContinuousTick => data.Capacity, 
					_ => 128, 
				};
				_values = new float[num];
				_histogram = new float[num];
				_intensity = new float[num];
			}
			switch (fusionGraphVisualization)
			{
			default:
				_ = 16;
				break;
			case FusionGraphVisualization.ContinuousTick:
				UpdateContinuousTick(ref data);
				break;
			case FusionGraphVisualization.IntermittentTick:
				UpdateIntermittentTick(ref data);
				break;
			case FusionGraphVisualization.IntermittentTime:
				UpdateIntermittentTime(ref data);
				break;
			case FusionGraphVisualization.ValueHistogram:
				UpdateTickValueHistogram(ref data);
				break;
			case FusionGraphVisualization.ContinuousTick | FusionGraphVisualization.IntermittentTick:
				break;
			}
		}
	}

	private void UpdateContinuousTick(ref IStatsBuffer data)
	{
		float min = float.MaxValue;
		float max = float.MinValue;
		float num = 0f;
		float last = 0f;
		for (int i = 0; i < data.Count; i++)
		{
			float num2 = (float)(StatSourceInfo.Multiplier * (double)data.GetSampleAtIndex(i).FloatValue);
			min = Math.Min(num2, min);
			max = Math.Max(num2, max);
			if (i >= _values.Length)
			{
				Debug.LogWarning(base.name + " Out of range " + i + " " + _values.Length + " " + data.Count);
			}
			last = (_values[i] = num2);
			num += num2;
		}
		num /= (float)data.Count;
		ApplyScaling(ref min, ref max);
		UpdateUiText(min, max, num, last);
	}

	private void UpdateIntermittentTick(ref IStatsBuffer data)
	{
		if (_cachedValues == null)
		{
			_cachedValues = new (int, float)[128];
		}
		int num = _fusionStats.Runner.Simulation.LatestServerState.Tick;
		float min = float.MaxValue;
		float max = float.MinValue;
		float num2 = 0f;
		float last = 0f;
		int num3 = num - 128 + 1;
		int num4 = (num % 128 + 1) % 128;
		int num5 = _lastCachedTick;
		for (int i = 0; i < data.Count; i++)
		{
			ISampleData sampleAtIndex = data.GetSampleAtIndex(i);
			int tickValue = sampleAtIndex.TickValue;
			if (tickValue < num3)
			{
				num5 = tickValue;
				continue;
			}
			if (tickValue <= _lastCachedTick)
			{
				num5 = tickValue;
				continue;
			}
			for (int j = num5 + 1; j < tickValue; j++)
			{
				_cachedValues[j % 128] = (tick: j, value: 0f);
			}
			_lastCachedTick = tickValue;
			_cachedValues[tickValue % 128] = (tick: tickValue, value: (float)(StatSourceInfo.Multiplier * (double)sampleAtIndex.FloatValue));
			num5 = tickValue;
		}
		for (int k = 0; k < 128; k++)
		{
			(int, float) tuple = _cachedValues[(k + num4) % 128];
			float num6 = tuple.Item2;
			if (tuple.Item1 < num3)
			{
				tuple.Item1 = num3 + k;
				num6 = (tuple.Item2 = 0f);
			}
			min = Math.Min(num6, min);
			max = Math.Max(num6, max);
			last = (_values[k] = num6);
			num2 += num6;
		}
		float intermittentAverageInfo = GetIntermittentAverageInfo(ref data, num2);
		ApplyScaling(ref min, ref max);
		UpdateUiText(min, max, intermittentAverageInfo, last);
	}

	private void UpdateIntermittentTime(ref IStatsBuffer data)
	{
		float min = float.MaxValue;
		float max = float.MinValue;
		float num = 0f;
		float last = 0f;
		for (int i = 0; i < data.Count; i++)
		{
			float num2 = (float)(StatSourceInfo.Multiplier * (double)data.GetSampleAtIndex(i).FloatValue);
			min = Math.Min(num2, min);
			max = Math.Max(num2, max);
			last = (_values[i] = num2);
			num += num2;
		}
		float intermittentAverageInfo = GetIntermittentAverageInfo(ref data, num);
		ApplyScaling(ref min, ref max);
		UpdateUiText(min, max, intermittentAverageInfo, last);
	}

	private void ApplyScaling(ref float min, ref float max)
	{
		if (min > 0f)
		{
			min = 0f;
		}
		if (max > _max)
		{
			_max = max;
		}
		if (min < _min)
		{
			_min = min;
		}
		float num = _max - _min;
		int i = 0;
		for (int num2 = _values.Length; i < num2; i++)
		{
			float num3 = _values[i];
			float num4 = ((num3 < 0f) ? (-1f) : ((num3 >= ErrorThreshold) ? 1f : ((num3 >= WarnThreshold) ? Mathf.Lerp(0.5f, 1f, (num3 - WarnThreshold) / (ErrorThreshold - WarnThreshold)) : 0f)));
			_intensity[i] = num4;
			_values[i] = Mathf.Clamp01((num3 - _min) / num);
		}
	}

	private void UpdateUiText(float min, float max, float avg, float last)
	{
		int decimals = StatSourceInfo.Decimals;
		if ((bool)LabelMin)
		{
			LabelMin.text = Math.Round(min, decimals).ToString();
		}
		if ((bool)LabelMax)
		{
			LabelMax.text = Math.Round(max, decimals).ToString();
		}
		if ((bool)LabelAvg)
		{
			LabelAvg.text = Math.Round(avg, decimals).ToString();
		}
		if ((bool)LabelLast)
		{
			LabelLast.text = Math.Round(last, decimals).ToString();
		}
		if ((bool)GraphImg && GraphImg.enabled)
		{
			GraphImg.material.SetFloatArray("_Data", _values);
			GraphImg.material.SetFloatArray("_Intensity", _intensity);
			GraphImg.material.SetFloat("_Count", _values.Length);
			GraphImg.material.SetFloat("_Height", Height);
			GraphImg.material.SetFloat("_ZeroCenter", (min < 0f) ? (min / (min - max)) : 0f);
		}
		_min = Mathf.Lerp(_min, 0f, Time.deltaTime);
		_max = Mathf.Lerp(_max, 1f, Time.deltaTime);
	}

	private float GetIntermittentAverageInfo(ref IStatsBuffer data, float sum)
	{
		switch (CurrentPer)
		{
		case Simulation.Statistics.StatsPer.Second:
		{
			float timeValue = data.GetSampleAtIndex(0).TimeValue;
			float timeValue2 = data.GetSampleAtIndex(data.Count - 1).TimeValue;
			return sum / (timeValue2 - timeValue);
		}
		case Simulation.Statistics.StatsPer.Tick:
		{
			int tickValue = data.GetSampleAtIndex(0).TickValue;
			int tickValue2 = data.GetSampleAtIndex(data.Count - 1).TickValue;
			return sum / (float)(tickValue2 - tickValue);
		}
		default:
			return sum / (float)_values.Length;
		}
	}

	private void UpdateTickValueHistogram(ref IStatsBuffer data)
	{
		int histoBucketCount = StatSourceInfo.HistoBucketCount;
		double histogMaxValue = StatSourceInfo.HistogMaxValue;
		if (_histoStepInverse == 0.0)
		{
			_histoStepInverse = (double)histoBucketCount / StatSourceInfo.HistogMaxValue;
		}
		int tickValue = data.GetSampleAtIndex(data.Count - 1).TickValue;
		SimulationSnapshot latestServerState = _fusionStats.Runner.Simulation.LatestServerState;
		bool flag = tickValue > 0;
		double num;
		if (flag)
		{
			num = (int)latestServerState.Tick;
			double num2 = tickValue;
			if (num2 < num)
			{
				int num3 = Math.Max((int)num2, (int)_lastCachedTickTime);
				int num4 = (int)num - num3;
				float num5 = _histogram[0] + (float)num4;
				_histogram[0] = num5;
				if (num5 > _max)
				{
					_max = num5;
				}
			}
		}
		else
		{
			num = latestServerState.Time;
		}
		Simulation.Statistics.StatSourceInfo statSourceInfo = StatSourceInfo;
		double multiplier = statSourceInfo.Multiplier;
		for (int num6 = data.Count - 1; num6 >= 0; num6--)
		{
			_ = data.GetSampleAtIndex(num6).FloatValue;
			ISampleData sampleAtIndex = data.GetSampleAtIndex(num6);
			if ((double)(flag ? ((float)sampleAtIndex.TickValue) : sampleAtIndex.TimeValue) <= _lastCachedTickTime)
			{
				break;
			}
			double num7 = (double)sampleAtIndex.FloatValue * multiplier;
			int num8 = ((num7 != 0.0) ? ((num7 == histogMaxValue) ? histoBucketCount : ((!(num7 > histogMaxValue)) ? ((int)(num7 * _histoStepInverse) + 1) : (histoBucketCount + 1))) : 0);
			_histoAvg = (_histoAvg * (double)_histoAvgSampleCount + num7) / (double)(++_histoAvgSampleCount);
			float num9 = _histogram[num8] + 1f;
			if (num9 > _max)
			{
				_max = num9;
			}
			_histogram[num8] = num9;
			if (num8 > _histoHighestUsedBucketIndex)
			{
				_histoHighestUsedBucketIndex = num8;
			}
		}
		int num10 = 0;
		float num11 = 0f;
		float num12 = (_max - _min) * 1.1f;
		int i = 0;
		for (int num13 = _histogram.Length; i < num13; i++)
		{
			float num14 = _histogram[i];
			_intensity[i] = 0f;
			if (i != 0 && num14 > num11)
			{
				num11 = num14;
				num10 = i;
			}
			_values[i] = Mathf.Clamp01((_histogram[i] - _min) / num12);
		}
		_intensity[num10] = 2f;
		_lastCachedTickTime = num;
		if ((bool)GraphImg && GraphImg.enabled)
		{
			GraphImg.material.SetFloatArray("_Data", _values);
			GraphImg.material.SetFloatArray("_Intensity", _intensity);
			GraphImg.material.SetFloat("_Count", _histoHighestUsedBucketIndex + 1);
			GraphImg.material.SetFloat("_Height", Height);
		}
		_min = 0f;
		int decimals = statSourceInfo.Decimals;
		LabelMax.text = $"<color=yellow>{Math.Ceiling((double)(num10 + 1) / _histoStepInverse)}</color>";
		LabelAvg.text = Math.Round(_histoAvg, decimals).ToString();
		LabelMin.text = Math.Floor(_min).ToString();
		LabelLast.text = Math.Round((double)(_histoHighestUsedBucketIndex + 1) / _histoStepInverse, decimals).ToString();
	}

	public static FusionGraph Create(FusionStats iFusionStats, Simulation.Statistics.StatSourceTypes statSourceType, int statId, RectTransform parentRT)
	{
		RectTransform rectTransform = parentRT.CreateRectTransform(Simulation.Statistics.GetDescription(statSourceType, statId).LongName);
		FusionGraph fusionGraph = rectTransform.gameObject.AddComponent<FusionGraph>();
		fusionGraph._fusionStats = iFusionStats;
		fusionGraph.Generate(statSourceType, statId, rectTransform);
		return fusionGraph;
	}

	public void Generate(Simulation.Statistics.StatSourceTypes type, int statId, RectTransform root)
	{
		_statSourceType = type;
		GetComponent<RectTransform>();
		_statId = statId;
		root.anchorMin = new Vector2(0.5f, 0.5f);
		root.anchorMax = new Vector2(0.5f, 0.5f);
		root.anchoredPosition3D = default;
		RectTransform rectTransform = root.CreateRectTransform("Background").ExpandAnchor();
		BackImage = rectTransform.gameObject.AddComponent<Image>();
		BackImage.color = BackColor;
		BackImage.raycastTarget = false;
		RectTransform rectTransform2 = rectTransform.CreateRectTransform("Graph").SetAnchors(0f, 1f, 0.2f, 0.8f).SetOffsets(0f, 0f, 0f, 0f);
		GraphImg = rectTransform2.gameObject.AddComponent<Image>();
		GraphImg.raycastTarget = false;
		ResetGraphShader();
		Color fontColor = _fusionStats.FontColor;
		Color fontColor2 = _fusionStats.FontColor * new Color(1f, 1f, 1f, 0.5f);
		RectTransform rectTransform3 = root.CreateRectTransform("Title").ExpandAnchor().SetOffsets(10f, -10f, 0f, -2f);
		rectTransform3.anchoredPosition = new Vector2(0f, 0f);
		LabelTitle = rectTransform3.AddText(base.name, TextAnchor.UpperRight, fontColor);
		LabelTitle.resizeTextMaxSize = 24;
		LabelTitle.raycastTarget = true;
		RectTransform rt = root.CreateRectTransform("Max").SetAnchors(0f, 0.3f, 0.85f, 1f).SetOffsets(6f, 0f, 0f, -2f);
		LabelMax = rt.AddText("-", TextAnchor.UpperLeft, fontColor2);
		RectTransform rt2 = root.CreateRectTransform("Min").SetAnchors(0f, 0.3f, 0f, 0.15f).SetOffsets(6f, 0f, 0f, -2f);
		LabelMin = rt2.AddText("-", TextAnchor.LowerLeft, fontColor2);
		RectTransform rectTransform4 = root.CreateRectTransform("Avg").SetOffsets(0f, 0f, 0f, 0f);
		rectTransform4.anchoredPosition = new Vector2(0f, 0f);
		LabelAvg = rectTransform4.AddText("-", TextAnchor.LowerCenter, fontColor);
		LabelAvg.raycastTarget = true;
		_avgBttn = rectTransform4.gameObject.AddComponent<Button>();
		RectTransform rt3 = root.CreateRectTransform("Per").SetAnchors(0.3f, 0.7f, 0f, 0.125f).SetOffsets(6f, -6f, 6f, 0f);
		LabelPer = rt3.AddText("avg per Sample", TextAnchor.LowerCenter, fontColor);
		RectTransform rt4 = root.CreateRectTransform("Last").SetAnchors(0.7f, 1f, 0f, 0.15f).SetOffsets(10f, -10f, 0f, -2f);
		LabelLast = rt4.AddText("-", TextAnchor.LowerRight, fontColor2);
		_viewDropdown = rectTransform3.CreateDropdown(10f, fontColor);
		_layoutDirty = true;
	}

	[BehaviourButtonAction("Update Layout", null, null)]
	public override void CalculateLayout()
	{
		try
		{
			if (base.gameObject == null)
			{
				return;
			}
		}
		catch
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		_layoutDirty = false;
		RectTransform component = GetComponent<RectTransform>();
		if (_statsBuffer == null)
		{
			TryConnect();
		}
		ApplyTitleText();
		bool flag = StatSourceInfo.InvalidReason == null;
		LabelMin.gameObject.SetActive(flag);
		LabelMax.gameObject.SetActive(flag);
		LabelAvg.gameObject.SetActive(flag);
		LabelPer.gameObject.SetActive(flag);
		if (!flag)
		{
			LabelTitle.rectTransform.ExpandAnchor(10f);
			LabelTitle.alignment = TextAnchor.MiddleCenter;
			LabelTitle.raycastTarget = false;
			_viewDropdown.gameObject.SetActive(value: false);
			return;
		}
		GraphImg.material.SetInt("_ZWrite", (_fusionStats.CanvasType == FusionStats.StatCanvasTypes.GameObject) ? 1 : 0);
		bool flag2 = _fusionStats.CanvasType == FusionStats.StatCanvasTypes.GameObject;
		bool noGraphShader = _fusionStats.NoGraphShader;
		float height = component.rect.height;
		float width = component.rect.width;
		Layouts layouts = ((_layout != Layouts.Auto) ? _layout : ((_fusionStats.DefaultLayout != Layouts.Auto) ? _fusionStats.DefaultLayout : ((height < 52f) ? Layouts.CompactAuto : ((!(width < 200f)) ? Layouts.FullAuto : Layouts.CenteredAuto))));
		bool num = noGraphShader || layouts == Layouts.CompactNoGraph || layouts == Layouts.CenteredNoGraph || (_fusionStats.NoTextOverlap && layouts == Layouts.CompactAuto);
		bool flag3 = _fusionStats.NoTextOverlap || layouts == Layouts.FullNoOverlap || layouts == Layouts.CenteredNoOverlap;
		bool flag4 = !num && (ShowGraph == ShowGraphOptions.Always || ((ShowGraph == ShowGraphOptions.OverlayOnly) & flag2));
		bool flag5 = !flag3 && (_alwaysExpandGraph || !flag4 || layouts == Layouts.CompactAuto || (!flag3 && height < 112f));
		bool flag6 = height < 18f;
		RectTransform rectTransform = GraphImg.rectTransform;
		if ((bool)rectTransform)
		{
			rectTransform.gameObject.SetActive(flag4);
			if (flag5)
			{
				rectTransform.SetAnchors(0f, 1f, 0f, 1f);
			}
			else
			{
				rectTransform.SetAnchors(0f, 1f, 0.25f, 0.8f);
			}
		}
		bool flag7 = layouts == Layouts.FullAuto || layouts == Layouts.FullNoOverlap;
		RectTransform rectTransform2 = LabelTitle.rectTransform;
		RectTransform rectTransform3 = LabelAvg.rectTransform;
		if (LabelPer == null)
		{
			RectTransform rt = rectTransform3.parent.CreateRectTransform("Per").SetAnchors(0.3f, 0.7f, 0f, 0.125f).SetOffsets(6f, -6f, 6f, 0f);
			LabelPer = rt.AddText("per sample", TextAnchor.LowerCenter, _fusionStats.FontColor);
		}
		RectTransform rectTransform4 = LabelPer.rectTransform;
		switch (layouts)
		{
		case Layouts.FullAuto:
		case Layouts.FullNoOverlap:
			rectTransform2.anchorMin = new Vector2(flag7 ? 0.3f : 0f, flag5 ? 0.5f : 0.8f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(6f, 0f);
			rectTransform2.offsetMax = new Vector2(-6f, -6f);
			LabelTitle.alignment = ((!flag7) ? TextAnchor.UpperCenter : TextAnchor.UpperRight);
			rectTransform3.anchorMin = new Vector2(flag7 ? 0.3f : 0f, flag5 ? 0.15f : 0.1f);
			rectTransform3.anchorMax = new Vector2(flag7 ? 0.7f : 1f, flag5 ? 0.5f : 0.25f);
			rectTransform3.SetOffsets(0f, 0f, 0f, 0f);
			LabelAvg.alignment = TextAnchor.LowerCenter;
			rectTransform4.SetAnchors(0.3f, 0.7f, 0f, flag5 ? 0.2f : 0.1f);
			LabelPer.alignment = TextAnchor.LowerCenter;
			break;
		case Layouts.CenteredAuto:
		case Layouts.CenteredNoGraph:
		case Layouts.CenteredNoOverlap:
			rectTransform2.anchorMin = new Vector2(0f, flag5 ? 0.5f : 0.8f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(6f, 0f);
			rectTransform2.offsetMax = new Vector2(-6f, -6f);
			LabelTitle.alignment = TextAnchor.UpperCenter;
			rectTransform3.anchorMin = new Vector2(0f, flag5 ? 0.15f : 0.1f);
			rectTransform3.anchorMax = new Vector2(1f, flag5 ? 0.5f : 0.25f);
			rectTransform3.SetOffsets(6f, -6f, 0f, 0f);
			rectTransform4.SetAnchors(0f, 1f, 0f, flag5 ? 0.2f : 0.1f);
			LabelPer.alignment = TextAnchor.LowerCenter;
			LabelAvg.alignment = TextAnchor.LowerCenter;
			break;
		case Layouts.CompactAuto:
		case Layouts.CompactNoGraph:
			rectTransform2.anchorMin = new Vector2(0.05f, 0f);
			rectTransform2.anchorMax = new Vector2(0.5f, 1f);
			if (flag6)
			{
				rectTransform2.SetOffsets(0f, 0f, 0f, 0f);
				rectTransform3.SetOffsets(6f, 0f, 0f, 0f);
			}
			else
			{
				rectTransform2.SetOffsets(0f, 0f, 6f, -6f);
				rectTransform3.SetOffsets(6f, 0f, 6f, -6f);
			}
			LabelTitle.alignment = TextAnchor.MiddleLeft;
			rectTransform3.SetAnchors(0.5f, 0.95f, 0f, 1f);
			rectTransform4.SetAnchors(0.5f, 0.95f, 0f, 0.15f).SetOffsets(6f, -12f, 6f, 0f);
			LabelPer.alignment = TextAnchor.LowerRight;
			LabelAvg.alignment = TextAnchor.MiddleRight;
			break;
		}
		LabelMin.enabled = flag7;
		LabelMax.enabled = flag7;
		LabelLast.enabled = flag7;
	}
}
