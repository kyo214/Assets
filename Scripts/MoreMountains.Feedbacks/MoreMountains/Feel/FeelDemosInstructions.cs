using System.Collections;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feel;

public class FeelDemosInstructions : MonoBehaviour
{
	[Header("Bindings")]
	public Text TargetText;

	public float DisappearDelay = 3f;

	public float DisappearDuration = 0.3f;

	[Header("Texts")]
	public string DesktopText = "Press space to...";

	public string MobileText = "Tap anywhere to...";

	protected CanvasGroup _canvasGroup;

	protected virtual void Awake()
	{
		TargetText.text = DesktopText;
		_canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		StartCoroutine(DisappearCo());
	}

	protected virtual IEnumerator DisappearCo()
	{
		yield return MMCoroutine.WaitFor(DisappearDelay);
		StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, DisappearDuration, 0f));
		yield return MMCoroutine.WaitFor(DisappearDuration + 0.1f);
		base.gameObject.SetActive(value: false);
	}
}
