using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Modules;

[AddComponentMenu("Mody/Animator Module")]
public class AnimatorModule : ModyModule
{
	public const string k_DefaultModuleName = "Animator";

	public List<ReactorAnimator> Animators = new List<ReactorAnimator>();

	public SimpleModyAction PlayForward;

	public SimpleModyAction PlayReverse;

	public SimpleModyAction Stop;

	public SimpleModyAction Finish;

	public SimpleModyAction Reverse;

	public SimpleModyAction Rewind;

	public SimpleModyAction Pause;

	public SimpleModyAction Resume;

	public FloatModyAction SetProgressAt;

	public SimpleModyAction SetProgressAtZero;

	public SimpleModyAction SetProgressAtOne;

	public FloatModyAction PlayToProgress;

	public FloatModyAction PlayFromProgress;

	public SimpleModyAction UpdateValues;

	public AnimatorModule()
		: this("Animator")
	{
	}

	public AnimatorModule(string moduleName)
		: base(moduleName.IsNullOrEmpty() ? "Animator" : moduleName)
	{
	}

	protected override void SetupActions()
	{
		this.AddAction(PlayForward ?? (PlayForward = new SimpleModyAction(this, "PlayForward", ExecutePlayForward)));
		this.AddAction(PlayReverse ?? (PlayReverse = new SimpleModyAction(this, "PlayReverse", ExecutePlayReverse)));
		this.AddAction(Stop ?? (Stop = new SimpleModyAction(this, "Stop", ExecuteStop)));
		this.AddAction(Finish ?? (Finish = new SimpleModyAction(this, "Finish", ExecuteFinish)));
		this.AddAction(Reverse ?? (Reverse = new SimpleModyAction(this, "Reverse", ExecuteReverse)));
		this.AddAction(Rewind ?? (Rewind = new SimpleModyAction(this, "Rewind", ExecuteRewind)));
		this.AddAction(Pause ?? (Pause = new SimpleModyAction(this, "Pause", ExecutePause)));
		this.AddAction(Resume ?? (Resume = new SimpleModyAction(this, "Resume", ExecuteResume)));
		this.AddAction(SetProgressAt ?? (SetProgressAt = new FloatModyAction(this, "SetProgressAt", ExecuteSetProgressAt)));
		this.AddAction(SetProgressAtZero ?? (SetProgressAtZero = new SimpleModyAction(this, "SetProgressAtZero", ExecuteSetProgressAtZero)));
		this.AddAction(SetProgressAtOne ?? (SetProgressAtOne = new SimpleModyAction(this, "SetProgressAtOne", ExecuteSetProgressAtOne)));
		this.AddAction(PlayToProgress ?? (PlayToProgress = new FloatModyAction(this, "PlayToProgress", ExecutePlayToProgress)));
		this.AddAction(PlayFromProgress ?? (PlayFromProgress = new FloatModyAction(this, "PlayFromProgress", ExecutePlayFromProgress)));
		this.AddAction(UpdateValues ?? (UpdateValues = new SimpleModyAction(this, "UpdateValues", ExecuteUpdateValues)));
	}

	public void CleanAnimatorsList()
	{
		for (int num = Animators.Count - 1; num >= 0; num--)
		{
			if (Animators[num] == null)
			{
				Animators.RemoveAt(num);
			}
		}
	}

	public void ExecutePlayForward()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Play(PlayDirection.Forward);
		}
	}

	public void ExecutePlayReverse()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Play(PlayDirection.Reverse);
		}
	}

	public void ExecuteStop()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Stop();
		}
	}

	public void ExecuteFinish()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Finish();
		}
	}

	public void ExecuteReverse()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Reverse();
		}
	}

	public void ExecuteRewind()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Rewind();
		}
	}

	public void ExecutePause()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Pause();
		}
	}

	public void ExecuteResume()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.Resume();
		}
	}

	public void ExecuteSetProgressAt(float value)
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.SetProgressAt(value);
		}
	}

	public void ExecuteSetProgressAtZero()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.SetProgressAtZero();
		}
	}

	public void ExecuteSetProgressAtOne()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.SetProgressAtOne();
		}
	}

	public void ExecutePlayToProgress(float value)
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.PlayToProgress(value);
		}
	}

	public void ExecutePlayFromProgress(float value)
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.PlayFromProgress(value);
		}
	}

	public void ExecuteUpdateValues()
	{
		CleanAnimatorsList();
		foreach (ReactorAnimator animator in Animators)
		{
			animator.UpdateValues();
		}
	}
}
