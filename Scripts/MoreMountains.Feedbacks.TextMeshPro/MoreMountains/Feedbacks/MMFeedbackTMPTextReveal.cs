using System.Collections;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you reveal words, lines, or characters in a target TMP, one at a time")]
[FeedbackPath("TextMesh Pro/TMP Text Reveal")]
public class MMFeedbackTMPTextReveal : MMFeedback
{
	public enum RevealModes
	{
		Character = 0,
		Lines = 1,
		Words = 2
	}

	public enum DurationModes
	{
		Interval = 0,
		TotalDuration = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("TextMesh Pro")]
	[Tooltip("the target TMP_Text component we want to change the text on")]
	public TMP_Text TargetTMPText;

	[Header("Change Text")]
	[Tooltip("whether or not to replace the current TMP target's text on play")]
	public bool ReplaceText;

	[Tooltip("the new text to replace the old one with")]
	[TextArea]
	public string NewText = "Hello World";

	[Header("Reveal")]
	[Tooltip("the selected way to reveal the text (character by character, word by word, or line by line)")]
	public RevealModes RevealMode;

	[Tooltip("whether to define duration by the time interval between two unit reveals, or by the total duration the reveal should take")]
	public DurationModes DurationMode;

	[Tooltip("the interval (in seconds) between two reveals")]
	[MMFEnumCondition("DurationMode", new int[] { 0 })]
	public float IntervalBetweenReveals = 0.05f;

	[Tooltip("the total duration of the text reveal, in seconds")]
	[MMFEnumCondition("DurationMode", new int[] { 1 })]
	public float RevealDuration = 1f;

	protected float _delay;

	protected Coroutine _coroutine;

	protected int _richTextLength;

	public override float FeedbackDuration
	{
		get
		{
			if (DurationMode == DurationModes.TotalDuration)
			{
				return RevealDuration;
			}
			if (TargetTMPText == null || TargetTMPText.textInfo == null)
			{
				return 0f;
			}
			return RevealMode switch
			{
				RevealModes.Character => (float)RichTextLength(TargetTMPText.text) * IntervalBetweenReveals, 
				RevealModes.Lines => (float)TargetTMPText.textInfo.lineCount * IntervalBetweenReveals, 
				RevealModes.Words => (float)TargetTMPText.textInfo.wordCount * IntervalBetweenReveals, 
				_ => 0f, 
			};
		}
		set
		{
			if (DurationMode == DurationModes.TotalDuration)
			{
				RevealDuration = value;
			}
			else if (TargetTMPText != null)
			{
				switch (RevealMode)
				{
				case RevealModes.Character:
					IntervalBetweenReveals = value / (float)RichTextLength(TargetTMPText.text);
					break;
				case RevealModes.Lines:
					IntervalBetweenReveals = value / (float)TargetTMPText.textInfo.lineCount;
					break;
				case RevealModes.Words:
					IntervalBetweenReveals = value / (float)TargetTMPText.textInfo.wordCount;
					break;
				}
			}
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetTMPText == null))
		{
			if (ReplaceText)
			{
				TargetTMPText.text = NewText;
				TargetTMPText.ForceMeshUpdate();
			}
			_richTextLength = RichTextLength(TargetTMPText.text);
			switch (RevealMode)
			{
			case RevealModes.Character:
				_delay = ((DurationMode == DurationModes.Interval) ? IntervalBetweenReveals : (RevealDuration / (float)_richTextLength));
				TargetTMPText.maxVisibleCharacters = 0;
				_coroutine = StartCoroutine(RevealCharacters());
				break;
			case RevealModes.Lines:
				_delay = ((DurationMode == DurationModes.Interval) ? IntervalBetweenReveals : (RevealDuration / (float)TargetTMPText.textInfo.lineCount));
				TargetTMPText.maxVisibleLines = 0;
				_coroutine = StartCoroutine(RevealLines());
				break;
			case RevealModes.Words:
				_delay = ((DurationMode == DurationModes.Interval) ? IntervalBetweenReveals : (RevealDuration / (float)TargetTMPText.textInfo.wordCount));
				TargetTMPText.maxVisibleWords = 0;
				_coroutine = StartCoroutine(RevealWords());
				break;
			}
		}
	}

	protected virtual IEnumerator RevealCharacters()
	{
		float startTime = ((Timing.TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime);
		int totalCharacters = _richTextLength;
		int visibleCharacters = 0;
		float lastCharAt = 0f;
		IsPlaying = true;
		while (visibleCharacters <= totalCharacters)
		{
			float deltaTime = ((Timing.TimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime);
			float time = ((Timing.TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime);
			if (time - lastCharAt < IntervalBetweenReveals)
			{
				yield return null;
			}
			TargetTMPText.maxVisibleCharacters = visibleCharacters;
			visibleCharacters++;
			lastCharAt = time;
			float seconds;
			if (DurationMode == DurationModes.Interval)
			{
				_delay = Mathf.Max(IntervalBetweenReveals, deltaTime);
				seconds = _delay - deltaTime;
			}
			else
			{
				int num = totalCharacters - visibleCharacters;
				float num2 = time - startTime;
				if (num != 0)
				{
					_delay = (RevealDuration - num2) / (float)num;
				}
				seconds = _delay - deltaTime;
			}
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				yield return MMFeedbacksCoroutine.WaitFor(seconds);
			}
			else
			{
				yield return MMFeedbacksCoroutine.WaitForUnscaled(seconds);
			}
		}
		TargetTMPText.maxVisibleCharacters = _richTextLength;
		IsPlaying = false;
	}

	protected virtual IEnumerator RevealLines()
	{
		int totalLines = TargetTMPText.textInfo.lineCount;
		int visibleLines = 0;
		IsPlaying = true;
		while (visibleLines <= totalLines)
		{
			TargetTMPText.maxVisibleLines = visibleLines;
			visibleLines++;
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				yield return MMFeedbacksCoroutine.WaitFor(_delay);
			}
			else
			{
				yield return MMFeedbacksCoroutine.WaitForUnscaled(_delay);
			}
		}
		IsPlaying = false;
	}

	protected virtual IEnumerator RevealWords()
	{
		int totalWords = TargetTMPText.textInfo.wordCount;
		int visibleWords = 0;
		IsPlaying = true;
		while (visibleWords <= totalWords)
		{
			TargetTMPText.maxVisibleWords = visibleWords;
			visibleWords++;
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				yield return MMFeedbacksCoroutine.WaitFor(_delay);
			}
			else
			{
				yield return MMFeedbacksCoroutine.WaitForUnscaled(_delay);
			}
		}
		IsPlaying = false;
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			IsPlaying = false;
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
		}
	}

	protected int RichTextLength(string richText)
	{
		int num = 0;
		bool flag = false;
		richText = richText.Replace("<br>", "-");
		string text = richText;
		for (int i = 0; i < text.Length; i++)
		{
			switch (text[i])
			{
			case '<':
				flag = true;
				continue;
			case '>':
				flag = false;
				continue;
			}
			if (!flag)
			{
				num++;
			}
		}
		return num;
	}
}
