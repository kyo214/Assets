using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private Image image2;

	[SerializeField]
	private XTimer timer;

	[SerializeField]
	private int ctr;

	private void Start()
	{
		timer.StartDuration(Random.Range(3f, 6f));
	}

	private void FixedUpdate()
	{
		if (timer.isCompleted())
		{
			ctr++;
			image.DOKill();
			image.DOFade(1f, 0f);
			image2.DOKill();
			image2.DOFade(1f, 0f);
			if (Random.Range(0, 2) == 0 && ctr == 1)
			{
				timer.StartDuration(0.25f);
				image.DOFade(0f, 0f).SetDelay(0.15f);
				image2.DOFade(0f, 0f).SetDelay(0.15f);
			}
			else
			{
				AudioManager.PlaySFX("storm");
				timer.StartDuration(Random.Range(6f, 10f));
				image.DOFade(0f, 3f);
				image2.DOFade(0f, 3f);
				ctr = 0;
			}
		}
	}
}
