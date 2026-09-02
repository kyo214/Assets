using DG.Tweening;
using Toked;
using UnityEngine;

public class EventLightBroken : MonoBehaviour
{
	[SerializeField]
	private Light lightObj;

	[SerializeField]
	private float initRange;

	[SerializeField]
	private float interval;

	[SerializeField]
	private float delay;

	[SerializeField]
	private int ctrFlicker;

	private void Start()
	{
		lightObj = GetComponent<Light>();
		initRange = lightObj.range;
		interval = Random.Range(0.3f, 0.7f);
		ctrFlicker = Random.Range(1, 3);
	}

	private void FixedUpdate()
	{
		interval -= Time.deltaTime;
		if (!(interval < 0f))
		{
			return;
		}
		if (lightObj.range == 0f)
		{
			if (ctrFlicker <= 0)
			{
				interval = Random.Range(0.05f, 0.5f);
				DOTween.To(() => lightObj.range, (float x) =>
				{
					lightObj.range = x;
				}, initRange, 0.05f);
				ctrFlicker = Random.Range(1, 3);
			}
			else
			{
				interval = Random.Range(0.03f, 0.05f);
				DOTween.To(() => lightObj.range, (float x) =>
				{
					lightObj.range = x;
				}, initRange, 0.03f);
				ctrFlicker--;
			}
		}
		else
		{
			AudioManager.PlaySFXTransform("broken-lamp", base.transform, isLocalPlayerTrigger: false);
			interval = Random.Range(0.03f, 0.05f);
			DOTween.To(() => lightObj.range, (float x) =>
			{
				lightObj.range = x;
			}, 0f, 0.03f);
		}
	}
}
