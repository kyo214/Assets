using System.Collections;
using System.IO;
using I2.Loc;
using RichTextSubstringHelper;
using TMPro;
using UnityEngine;

namespace EasySubtitles;

public class SubtitlePlayer : MonoBehaviour
{
	public enum Mode
	{
		Instant = 0,
		CharacterByCharacter = 1
	}

	public enum TextMode
	{
		Text = 0,
		I2Loc = 1
	}

	public enum SubtitleMode
	{
		TextAsset = 0,
		Path = 1
	}

	public enum RootPath
	{
		StreamingAssets = 0,
		ApplicationData = 1
	}

	[Header("Subtitles")]
	[Tooltip("The subtitles Mode")]
	[SerializeField]
	private SubtitleMode _subtitlesMode;

	[Tooltip("The subtitles path")]
	[SerializeField]
	private RootPath _rootPath;

	[Tooltip("The subtitles path")]
	[SerializeField]
	private string _subtitlesPath;

	[Tooltip("The subtitles file to play")]
	[SerializeField]
	private TextAsset _subtitles;

	[Tooltip("The mode of display")]
	[SerializeField]
	private Mode _mode;

	[Tooltip("The mode of text subtitles")]
	[SerializeField]
	private TextMode _textMode;

	[Tooltip("The text component to display the subtitles")]
	[SerializeField]
	private TMP_Text _text;

	[Header("Audio")]
	[Tooltip("The audio source that will be played with the subtitles")]
	[SerializeField]
	private AudioSource _audioSource;

	private Subtitles parsedSubtitles;

	private float timer;

	private Subtitle currentSubtitle;

	public TextAsset Subtitles
	{
		get
		{
			return _subtitles;
		}
		set
		{
			_subtitles = value;
		}
	}

	public bool IsPlaying { get; private set; }

	private void Update()
	{
		if (IsPlaying && parsedSubtitles != null)
		{
			float time;
			float num;
			if (_audioSource != null && _audioSource.isPlaying)
			{
				time = _audioSource.time;
				num = _audioSource.clip.length;
			}
			else
			{
				timer += Time.deltaTime;
				time = timer;
				num = parsedSubtitles.Duration;
			}
			if (num - time < 0.1f)
			{
				Stop();
			}
			else
			{
				UpdateText(parsedSubtitles.GetSubtitleAt(time));
			}
		}
	}

	private void UpdateText(Subtitle subtitle)
	{
		if (subtitle != currentSubtitle)
		{
			currentSubtitle = subtitle;
			_text.margin = new Vector4(subtitle.X1, subtitle.Y1, subtitle.X2, subtitle.Y2);
			if (_mode == Mode.Instant)
			{
				_text.text = GetTextSubtitle(subtitle.Text);
			}
			else if (_mode == Mode.CharacterByCharacter)
			{
				StartCoroutine(PlayCharacterByCharacter(subtitle));
			}
		}
	}

	private IEnumerator PlayCharacterByCharacter(Subtitle subtitle)
	{
		_text.text = string.Empty;
		string textSubtitle = GetTextSubtitle(subtitle.Text);
		int length = textSubtitle.Length;
		float num = subtitle.Duration * 0.5f;
		WaitForSeconds characterDelay = new WaitForSeconds(num / (float)length);
		RichTextSubStringMaker maker = new RichTextSubStringMaker(textSubtitle);
		while (maker.IsConsumable() && IsPlaying)
		{
			maker.Consume();
			_text.text = maker.GetRichText();
			yield return characterDelay;
		}
	}

	public void Play(Mode mode)
	{
		if (_subtitlesMode == SubtitleMode.Path)
		{
			string pathSubtitle = GetPathSubtitle();
			if (File.Exists(pathSubtitle))
			{
				_subtitles = new TextAsset(File.ReadAllText(pathSubtitle));
			}
		}
		if (_subtitles == null)
		{
			Debug.LogError("No subtitles assigned");
		}
		else
		{
			Play(_subtitles, mode);
		}
	}

	public void Play()
	{
		Play(_mode);
	}

	public void Play(TextAsset subtitles, Mode mode)
	{
		Play(subtitles, mode, _audioSource);
	}

	public void Play(TextAsset subtitles, Mode mode, AudioSource audioSource)
	{
		if (subtitles != _subtitles)
		{
			_subtitles = subtitles;
			parsedSubtitles = new Subtitles(subtitles);
		}
		else if (parsedSubtitles == null)
		{
			parsedSubtitles = new Subtitles(subtitles);
		}
		_mode = mode;
		timer = 0f;
		_audioSource = audioSource;
		if (_audioSource != null)
		{
			_audioSource.Play();
		}
		IsPlaying = true;
	}

	public void Stop()
	{
		if (_audioSource != null)
		{
			_audioSource.Stop();
		}
		IsPlaying = false;
		_text.text = string.Empty;
	}

	private string GetTextSubtitle(string subtitle)
	{
		if (_textMode == TextMode.I2Loc)
		{
			return LocalizationManager.GetTranslation(subtitle) ?? subtitle;
		}
		return subtitle;
	}

	private string GetPathSubtitle()
	{
		return _rootPath switch
		{
			RootPath.StreamingAssets => Path.Join(Application.streamingAssetsPath, _subtitlesPath), 
			RootPath.ApplicationData => _subtitlesPath, 
			_ => _subtitlesPath, 
		};
	}
}
