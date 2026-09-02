using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Spritetrail : MonoBehaviour
{
	public float trailTime = 1f;

	public float trailDelay = 0.1f;

	public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

	private List<SpriteRenderer> trailRenderers = new List<SpriteRenderer>();

	private void Start()
	{
		spriteRenderers.AddRange(GetComponents<SpriteRenderer>());
		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			CreateTrail(spriteRenderer).Forget();
		}
	}

	private async UniTask CreateTrail(SpriteRenderer originalRenderer)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		while (true)
		{
			SpriteRenderer trailRenderer = UnityEngine.Object.Instantiate(originalRenderer, originalRenderer.transform.position, originalRenderer.transform.rotation);
			trailRenderer.transform.localScale = originalRenderer.transform.lossyScale;
			trailRenderers.Add(trailRenderer);
			Color color = trailRenderer.color;
			color.a = 0.7f;
			trailRenderer.color = color;
			trailRenderer.DOFade(0f, trailDelay / 2f).SetDelay(trailTime / 2f);
			await UniTask.Delay(TimeSpan.FromSeconds(trailDelay), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			trailRenderer.DOKill();
			UnityEngine.Object.Destroy(trailRenderer.gameObject, trailTime);
			trailRenderers.Remove(trailRenderer);
		}
	}
}
