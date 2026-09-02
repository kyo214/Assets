using System.Collections;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Global;

public class Coroutiner : SingletonBehaviour<Coroutiner>
{
	public Coroutine StartLocalCoroutine(IEnumerator enumerator)
	{
		return StartCoroutine(enumerator);
	}

	public void StopLocalCoroutine(Coroutine coroutine)
	{
		StopCoroutine(coroutine);
	}

	public void StopLocalCoroutine(IEnumerator enumerator)
	{
		StopCoroutine(enumerator);
	}

	public void StopAllLocalCoroutines()
	{
		StopAllCoroutines();
	}

	public static Coroutine Start(IEnumerator enumerator)
	{
		if (!(SingletonBehaviour<Coroutiner>.instance != null) || enumerator == null)
		{
			return null;
		}
		return SingletonBehaviour<Coroutiner>.instance.StartLocalCoroutine(enumerator);
	}

	public static void Stop(IEnumerator enumerator)
	{
		if (!(SingletonBehaviour<Coroutiner>.instance == null) && enumerator != null)
		{
			SingletonBehaviour<Coroutiner>.instance.StopLocalCoroutine(enumerator);
		}
	}

	public static void Stop(Coroutine coroutine)
	{
		if (!(SingletonBehaviour<Coroutiner>.instance == null) && coroutine != null)
		{
			SingletonBehaviour<Coroutiner>.instance.StopLocalCoroutine(coroutine);
		}
	}

	public static void StopAll()
	{
		if (!(SingletonBehaviour<Coroutiner>.instance == null))
		{
			SingletonBehaviour<Coroutiner>.instance.StopAllLocalCoroutines();
		}
	}

	public static Coroutine ExecuteLater(UnityAction callback, float delay)
	{
		return Start(DelayExecution(callback, delay));
	}

	public static Coroutine ExecuteLater(UnityAction callback, int numberOfFrames)
	{
		return Start(DelayExecution(callback, numberOfFrames));
	}

	public static Coroutine ExecuteAtEndOfFrame(UnityAction callback)
	{
		return Start(DelayExecutionToTheEndOfFrame(callback));
	}

	public static Coroutine ExecuteNextFrame(UnityAction callback)
	{
		return Start(DelayExecutionToTheNextFrame(callback));
	}

	public static IEnumerator DelayExecution(UnityAction callback, float delay)
	{
		delay = ((delay < 0f) ? 0f : delay);
		yield return new WaitForSecondsRealtime(delay);
		callback?.Invoke();
	}

	public static IEnumerator DelayExecution(UnityAction callback, int numberOfFrames)
	{
		for (numberOfFrames = ((numberOfFrames >= 0) ? numberOfFrames : 0); numberOfFrames > 0; numberOfFrames--)
		{
			yield return null;
		}
		callback?.Invoke();
	}

	public static IEnumerator DelayExecutionToTheEndOfFrame(UnityAction callback)
	{
		yield return new WaitForEndOfFrame();
		callback?.Invoke();
	}

	public static IEnumerator DelayExecutionToTheNextFrame(UnityAction callback)
	{
		yield return null;
		callback?.Invoke();
	}
}
