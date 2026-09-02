using System;
using System.Collections;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators;

public abstract class BaseUIContainerAnimator : BaseTargetComponentAnimator<UIContainer>
{
	private Coroutine executeCommandCoroutine { get; set; }

	protected bool reversingShow { get; set; }

	protected bool reversingHide { get; set; }

	protected override void ConnectToController()
	{
		if (!(base.controller == null))
		{
			UIContainer uIContainer = base.controller;
			uIContainer.showHideExecute = (UnityAction<ShowHideExecute>)Delegate.Remove(uIContainer.showHideExecute, new UnityAction<ShowHideExecute>(Execute));
			UIContainer uIContainer2 = base.controller;
			uIContainer2.showHideExecute = (UnityAction<ShowHideExecute>)Delegate.Combine(uIContainer2.showHideExecute, new UnityAction<ShowHideExecute>(Execute));
			if (base.controller.executedFirstCommand)
			{
				Execute(base.controller.previouslyExecutedCommand);
			}
		}
	}

	protected override void DisconnectFromController()
	{
		if (!(base.controller == null))
		{
			UIContainer uIContainer = base.controller;
			uIContainer.showHideExecute = (UnityAction<ShowHideExecute>)Delegate.Remove(uIContainer.showHideExecute, new UnityAction<ShowHideExecute>(Execute));
		}
	}

	protected virtual void Execute(ShowHideExecute execute)
	{
		if (executeCommandCoroutine != null)
		{
			StopCoroutine(executeCommandCoroutine);
			executeCommandCoroutine = null;
		}
		if (!base.animatorInitialized)
		{
			executeCommandCoroutine = StartCoroutine(ExecuteCommandAfterAnimatorInitialized(execute));
			return;
		}
		switch (execute)
		{
		case ShowHideExecute.Show:
			Show();
			break;
		case ShowHideExecute.Hide:
			Hide();
			break;
		case ShowHideExecute.InstantShow:
			InstantShow();
			break;
		case ShowHideExecute.InstantHide:
			InstantHide();
			break;
		case ShowHideExecute.ReverseShow:
			ReverseShow();
			break;
		case ShowHideExecute.ReverseHide:
			ReverseHide();
			break;
		default:
			throw new ArgumentOutOfRangeException("execute", execute, null);
		}
	}

	private IEnumerator ExecuteCommandAfterAnimatorInitialized(ShowHideExecute execute)
	{
		yield return new WaitUntil(() => base.animatorInitialized);
		Execute(execute);
		executeCommandCoroutine = null;
	}

	public abstract void Show();

	public abstract void ReverseShow();

	public abstract void Hide();

	public abstract void ReverseHide();

	public abstract void InstantShow();

	public abstract void InstantHide();
}
