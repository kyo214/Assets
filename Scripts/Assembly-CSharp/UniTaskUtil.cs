using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskUtil
{
	public static async UniTask DelayedCall(MonoBehaviour owner, float delay, Action callback, bool ignoreTimeScale = true)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale, PlayerLoopTiming.Update, owner.GetCancellationTokenOnDestroy());
		callback?.Invoke();
	}

	public static async UniTask DelayedCall(float delay, Action callback, bool ignoreTimeScale = true)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale);
		callback?.Invoke();
	}
}
