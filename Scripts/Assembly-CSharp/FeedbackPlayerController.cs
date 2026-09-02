using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;

public class FeedbackPlayerController : MonoBehaviour
{
	[SerializeField]
	private PlayerController player;

	[SerializeField]
	private bool isSfxPlayerGruntPlaying;

	private bool _isSicknessVfxPlaying;

	private bool _isHurtVfxPlaying;

	public async UniTask Hurt(bool IsCloseInventory = true, bool isGreenBloodScreen = false)
	{
		_isHurtVfxPlaying = true;
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		if (IsCloseInventory)
		{
			if (player.network.isLocalPlayer)
			{
				if (!UIGameManager.Instance.UIMenuMap.isHidden)
				{
					player.CloseMap();
				}
				else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden)
				{
					player.ClosePuzzle();
				}
				else if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf)
				{
					player.CloseNote();
				}
				else if (!UIGameManager.Instance.uiInventory.isHidden)
				{
					UIGameManager.Instance.HideInventory();
				}
			}
			if (NetworkGameManager.Instance.isServer && player.network.GetHealth() > 0f)
			{
				player.network.SetEnableControl(value: true);
			}
		}
		player.isHurt = true;
		if (player.network.isLocalPlayer)
		{
			UIGameManager.Instance.flashRed.image.gameObject.SetActive(value: true);
			UIGameManager.Instance.flashRed.RandomizeSprite();
			UIGameManager.Instance.flashRed.image.color = new Color(0.8f, 0.06f, 0.06f);
			UIGameManager.Instance.flashRed.image.DOKill();
			UIGameManager.Instance.flashRed.image.DOFade(0.4f, 0f);
			UIGameManager.Instance.flashRed.image.DOFade(0f, 1f).SetDelay(0.5f).OnComplete(() =>
			{
				UIGameManager.Instance.flashRed.gameObject.SetActive(value: false);
			});
			CameraGame.Instance.FeedbackHurt();
			AudioManager.PlaySFX("impact_flesh");
			if (!isSfxPlayerGruntPlaying)
			{
				if (player.IsMale)
				{
					AudioManager.PlaySFX("male_grunt2");
				}
				else
				{
					AudioManager.PlaySFX("female_grunt2");
				}
				isSfxPlayerGruntPlaying = true;
				UniTaskUtil.DelayedCall(this, 2.5f, () =>
				{
					isSfxPlayerGruntPlaying = false;
				}).Forget();
			}
		}
		try
		{
			foreach (SpriteRenderer allSpritePart in player.allSpriteParts)
			{
				allSpritePart.material.SetFloat("_Brightness", 1f);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.029999999329447746), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			foreach (SpriteRenderer allSpritePart2 in player.allSpriteParts)
			{
				allSpritePart2.material.SetFloat("_Brightness", 0f);
			}
			foreach (SpriteRenderer allSpritePart3 in player.allSpriteParts)
			{
				allSpritePart3.material.SetColor("_Tint", new Color(1.6f, 0f, 0f));
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			foreach (SpriteRenderer allSpritePart4 in player.allSpriteParts)
			{
				allSpritePart4.material.DOKill();
				allSpritePart4.material.DOColor(new Color(0f, 0f, 0f), "_Tint", 0.15f).SetEase(Ease.Linear);
			}
		}
		finally
		{
			_isHurtVfxPlaying = false;
		}
		player.isHurt = false;
	}

	public async UniTask SicknessVfx()
	{
		if (_isSicknessVfxPlaying || _isHurtVfxPlaying)
		{
			return;
		}
		_isSicknessVfxPlaying = true;
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		try
		{
			foreach (SpriteRenderer allSpritePart in player.allSpriteParts)
			{
				allSpritePart.material.SetColor("_Tint", new Color(0.627f, 0.125f, 0.941f));
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			foreach (SpriteRenderer allSpritePart2 in player.allSpriteParts)
			{
				allSpritePart2.material.DOKill();
				allSpritePart2.material.DOColor(new Color(0f, 0f, 0f), "_Tint", 0.15f).SetEase(Ease.Linear);
			}
			await UniTask.Delay(TimeSpan.FromSeconds(1.2000000476837158), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		}
		finally
		{
			_isSicknessVfxPlaying = false;
		}
	}
}
