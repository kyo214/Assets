using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public static class MMFade
{
	public static IEnumerator FadeImage(Image target, float duration, Color color)
	{
		if (target == null)
		{
			yield break;
		}
		float alpha = target.color.a;
		for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
		{
			if (target == null)
			{
				yield break;
			}
			Color color2 = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
			target.color = color2;
			yield return null;
		}
		target.color = color;
	}

	public static IEnumerator FadeText(Text target, float duration, Color color)
	{
		if (target == null)
		{
			yield break;
		}
		float alpha = target.color.a;
		for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
		{
			if (target == null)
			{
				yield break;
			}
			Color color2 = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
			target.color = color2;
			yield return null;
		}
		target.color = color;
	}

	public static IEnumerator FadeSprite(SpriteRenderer target, float duration, Color color)
	{
		if (target == null)
		{
			yield break;
		}
		float alpha = target.material.color.a;
		float t = 0f;
		while (t < 1f)
		{
			if (target == null)
			{
				yield break;
			}
			Color color2 = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
			target.material.color = color2;
			t += Time.deltaTime / duration;
			yield return null;
		}
		Color color3 = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
		if (target != null)
		{
			target.material.color = color3;
		}
	}

	public static IEnumerator FadeCanvasGroup(CanvasGroup target, float duration, float targetAlpha, bool unscaled = true)
	{
		if (target == null)
		{
			yield break;
		}
		float currentAlpha = target.alpha;
		float t = 0f;
		while (t < 1f)
		{
			if (target == null)
			{
				yield break;
			}
			float alpha = Mathf.SmoothStep(currentAlpha, targetAlpha, t);
			target.alpha = alpha;
			t = ((!unscaled) ? (t + Time.deltaTime / duration) : (t + Time.unscaledDeltaTime / duration));
			yield return null;
		}
		target.alpha = targetAlpha;
	}
}
