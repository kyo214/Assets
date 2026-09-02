using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Modules;

[AddComponentMenu("Mody/AudioSource Module")]
public class AudioSourceModule : ModyModule
{
	public const string k_DefaultModuleName = "AudioSource";

	public AudioSource Source;

	public SimpleModyAction Play;

	public SimpleModyAction Stop;

	public SimpleModyAction Mute;

	public SimpleModyAction Unmute;

	public SimpleModyAction Pause;

	public SimpleModyAction Unpause;

	public bool hasSource => Source != null;

	public AudioSourceModule()
		: this("AudioSource")
	{
	}

	public AudioSourceModule(AudioSource audioSource)
		: this("AudioSource", audioSource)
	{
	}

	public AudioSourceModule(string moduleName, AudioSource audioSource)
		: this(moduleName.IsNullOrEmpty() ? "AudioSource" : moduleName)
	{
		Source = audioSource;
	}

	public AudioSourceModule(string moduleName)
		: base(moduleName)
	{
	}

	protected override void SetupActions()
	{
		this.AddAction(Play ?? (Play = new SimpleModyAction(this, "Play", ExecuteSourcePlay)));
		this.AddAction(Stop ?? (Stop = new SimpleModyAction(this, "Stop", ExecuteSourceStop)));
		this.AddAction(Mute ?? (Mute = new SimpleModyAction(this, "Mute", ExecuteSourceMute)));
		this.AddAction(Unmute ?? (Unmute = new SimpleModyAction(this, "Unmute", ExecuteSourceUnmute)));
		this.AddAction(Pause ?? (Pause = new SimpleModyAction(this, "Pause", ExecutePauseSource)));
		this.AddAction(Unpause ?? (Unpause = new SimpleModyAction(this, "Unpause", ExecuteSourceUnpause)));
	}

	public void ExecuteSourcePlay()
	{
		if (hasSource)
		{
			Source.Play();
		}
	}

	public void ExecuteSourceStop()
	{
		if (hasSource)
		{
			Source.Stop();
		}
	}

	public void ExecuteSourceMute()
	{
		if (hasSource)
		{
			Source.mute = true;
		}
	}

	public void ExecuteSourceUnmute()
	{
		if (hasSource)
		{
			Source.mute = false;
		}
	}

	public void ExecutePauseSource()
	{
		if (hasSource)
		{
			Source.Pause();
		}
	}

	public void ExecuteSourceUnpause()
	{
		if (hasSource)
		{
			Source.UnPause();
		}
	}
}
