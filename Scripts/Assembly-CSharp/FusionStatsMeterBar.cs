using System;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.UI;

public class FusionStatsMeterBar : FusionGraphBase
{
	public float HoldPeakTime = 0.1f;

	public float DecayTime = 0.25f;

	[InlineHelp]
	public int MeterMax;

	[InlineHelp]
	[SerializeField]
	private bool _showUITargets;

	[DrawIf("_showUITargets", Hide = true)]
	public Text ValueLabel;

	[DrawIf("_showUITargets", Hide = true)]
	public Image Bar;

	private double _currentDisplayValue;

	private double _currentBarValue;

	private Color CurrentColor;

	private double _lastImportedSampleTickTime;

	private double _max;

	private double _total;

	private float _lastPeakSetTime;

	protected override Color BackColor => base.BackColor * new Color(0.5f, 0.5f, 0.5f, 1f);

	public override void Initialize()
	{
		base.Initialize();
		_max = MeterMax;
		if (BackImage.sprite == null)
		{
			BackImage.sprite = FusionStatsUtilities.MeterSprite;
			Bar.sprite = BackImage.sprite;
		}
		BackImage.type = Image.Type.Simple;
		if (Bar.rectTransform.parent != BackImage.rectTransform.parent)
		{
			_ = Bar.transform.parent;
			Bar.rectTransform.SetParent(BackImage.rectTransform.parent);
			Bar.transform.SetSiblingIndex(BackImage.transform.GetSiblingIndex() + 1);
		}
		Bar.type = Image.Type.Filled;
		Bar.fillMethod = Image.FillMethod.Horizontal;
		Bar.fillAmount = 0f;
	}

	public override void Refresh()
	{
		if (_layoutDirty)
		{
			CalculateLayout();
		}
		IStatsBuffer statsBuffer = base.StatsBuffer;
		if (statsBuffer == null || statsBuffer.Count < 1)
		{
			return;
		}
		if (statsBuffer.DefaultVisualization == FusionGraphVisualization.CountHistogram)
		{
			if (statsBuffer.Count <= 0)
			{
				return;
			}
			int num = 0;
			float num2 = statsBuffer.GetSampleAtIndex(statsBuffer.Count - 1).TickValue;
			float num3 = num2;
			if ((double)num2 > _lastImportedSampleTickTime)
			{
				int num4 = 0;
				for (int num5 = statsBuffer.Count - 1; num5 >= 0; num5--)
				{
					int tickValue = statsBuffer.GetSampleAtIndex(num5).TickValue;
					if (!((double)tickValue > _lastImportedSampleTickTime))
					{
						break;
					}
					if ((float)tickValue != num3)
					{
						num3 = tickValue;
						if (num4 > num)
						{
							num = num4;
						}
						num4 = 0;
					}
					num4++;
					_total++;
				}
				_lastImportedSampleTickTime = num2;
			}
			SetValue(num);
		}
		else if (statsBuffer.Count > 0)
		{
			ISampleData sampleAtIndex = statsBuffer.GetSampleAtIndex(statsBuffer.Count - 1);
			if (sampleAtIndex.TickValue == _fusionStats.Runner.Simulation.LatestServerState.Tick)
			{
				SetValue(sampleAtIndex.FloatValue);
			}
			else
			{
				SetValue(0.0);
			}
		}
	}

	public void LateUpdate()
	{
		if (!(DecayTime <= 0f) && !(_currentBarValue <= 0.0) && !(Time.time < _lastPeakSetTime + HoldPeakTime))
		{
			double bar = Math.Max(_currentBarValue - (double)(Time.deltaTime / DecayTime) * _max, 0.0);
			SetBar(bar);
		}
	}

	public void SetValue(double rawvalue)
	{
		Simulation.Statistics.StatSourceInfo statSourceInfo = StatSourceInfo;
		double num = rawvalue * statSourceInfo.Multiplier;
		if (MeterMax == 0 && num > _max)
		{
			_max = num;
		}
		double num2 = Math.Max(Math.Min(num, _max), 0.0);
		double num3 = Math.Round(num2, statSourceInfo.Decimals);
		double num4 = ((_total > 0.0) ? _total : num3);
		if (num2 >= _currentBarValue)
		{
			_lastPeakSetTime = Time.time;
		}
		if (num4 != _currentDisplayValue)
		{
			ValueLabel.text = ((_total > 0.0) ? _total.ToString() : num2.ToString());
			_currentDisplayValue = num4;
		}
		if ((!(DecayTime >= 0f) || !(num2 <= _currentBarValue)) && num2 != _currentBarValue)
		{
			SetBar(num2);
		}
	}

	private void SetBar(double value)
	{
		FusionStats fusionStats = _fusionStats;
		Bar.fillAmount = (float)(value / _max);
		_currentBarValue = value;
		if (value < (double)WarnThreshold)
		{
			Color graphColorGood = fusionStats.GraphColorGood;
			if (CurrentColor != graphColorGood)
			{
				CurrentColor = graphColorGood;
				Bar.color = graphColorGood;
			}
		}
		else if (value < (double)ErrorThreshold)
		{
			Color graphColorWarn = fusionStats.GraphColorWarn;
			if (CurrentColor != graphColorWarn)
			{
				Bar.color = graphColorWarn;
				CurrentColor = graphColorWarn;
			}
		}
		else
		{
			Color graphColorBad = fusionStats.GraphColorBad;
			if (CurrentColor != graphColorBad)
			{
				Bar.color = graphColorBad;
				CurrentColor = graphColorBad;
			}
		}
	}

	public override void CalculateLayout()
	{
		_layoutDirty = false;
		float num = LabelTitle.transform.parent.GetComponent<RectTransform>().rect.height * 0.2f;
		LabelTitle.rectTransform.offsetMax = new Vector2(0f, 0f - num);
		LabelTitle.rectTransform.offsetMin = new Vector2(10f, num * 1.2f);
		ValueLabel.rectTransform.offsetMax = new Vector2(-10f, 0f - num);
		ValueLabel.rectTransform.offsetMin = new Vector2(0f, num * 1.2f);
		ApplyTitleText();
	}

	public static FusionStatsMeterBar Create(RectTransform parent, FusionStats fusionStats, Simulation.Statistics.StatSourceTypes statSourceType, int statId, float warnThreshold, float alertThreshold)
	{
		Simulation.Statistics.StatSourceInfo description = Simulation.Statistics.GetDescription(statSourceType, statId);
		FusionStatsMeterBar fusionStatsMeterBar = parent.CreateRectTransform(description.LongName, expand: true).gameObject.AddComponent<FusionStatsMeterBar>();
		fusionStatsMeterBar.StatSourceInfo = description;
		fusionStatsMeterBar._fusionStats = fusionStats;
		fusionStatsMeterBar._statSourceType = statSourceType;
		fusionStatsMeterBar._statId = statId;
		fusionStatsMeterBar.GenerateMeter();
		return fusionStatsMeterBar;
	}

	public void GenerateMeter()
	{
		Simulation.Statistics.StatSourceInfo description = Simulation.Statistics.GetDescription(_statSourceType, _statId);
		RectTransform rectTransform = base.transform.CreateRectTransform("Back", expand: true);
		BackImage = rectTransform.gameObject.AddComponent<Image>();
		BackImage.raycastTarget = false;
		BackImage.sprite = FusionStatsUtilities.MeterSprite;
		BackImage.color = BackColor;
		BackImage.type = Image.Type.Simple;
		RectTransform rectTransform2 = base.transform.CreateRectTransform("Bar", expand: true);
		Bar = rectTransform2.gameObject.AddComponent<Image>();
		Bar.raycastTarget = false;
		Bar.sprite = BackImage.sprite;
		Bar.color = _fusionStats.GraphColorGood;
		Bar.type = Image.Type.Filled;
		Bar.fillMethod = Image.FillMethod.Horizontal;
		Bar.fillAmount = 0f;
		RectTransform rt = base.transform.CreateRectTransform("Label", expand: true).ExpandAnchor().SetAnchors(0f, 0.5f, 0f, 1f)
			.SetOffsets(6f, -6f, 6f, -6f);
		LabelTitle = rt.AddText(description.LongName, TextAnchor.MiddleLeft, _fusionStats.FontColor);
		LabelTitle.alignByGeometry = false;
		RectTransform rt2 = base.transform.CreateRectTransform("Value", expand: true).ExpandAnchor().SetAnchors(0.5f, 1f, 0f, 1f)
			.SetOffsets(6f, -6f, 6f, -6f);
		ValueLabel = rt2.AddText("0.0", TextAnchor.MiddleRight, _fusionStats.FontColor);
		ValueLabel.alignByGeometry = false;
	}
}
