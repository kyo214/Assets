using System.Collections;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Achievements/MMAchievementDisplayer")]
public class MMAchievementDisplayer : MonoBehaviour, MMEventListener<MMAchievementUnlockedEvent>, MMEventListenerBase
{
	[Header("Achievements")]
	public MMAchievementDisplayItem AchievementDisplayPrefab;

	public float AchievementDisplayDuration = 5f;

	public float AchievementFadeDuration = 0.2f;

	protected WaitForSeconds _achievementFadeOutWFS;

	public virtual IEnumerator DisplayAchievement(MMAchievement achievement)
	{
		if (base.transform == null || AchievementDisplayPrefab == null)
		{
			yield break;
		}
		GameObject gameObject = Object.Instantiate(AchievementDisplayPrefab.gameObject);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		MMAchievementDisplayItem component = gameObject.GetComponent<MMAchievementDisplayItem>();
		if (!(component == null))
		{
			component.Title.text = achievement.Title;
			component.Description.text = achievement.Description;
			component.Icon.sprite = achievement.UnlockedImage;
			if (achievement.AchievementType == AchievementTypes.Progress)
			{
				component.ProgressBarDisplay.gameObject.SetActive(value: true);
			}
			else
			{
				component.ProgressBarDisplay.gameObject.SetActive(value: false);
			}
			if (achievement.UnlockedSound != null)
			{
				MMSfxEvent.Trigger(achievement.UnlockedSound);
			}
			CanvasGroup achievementCanvasGroup = gameObject.GetComponent<CanvasGroup>();
			if (achievementCanvasGroup != null)
			{
				achievementCanvasGroup.alpha = 0f;
				StartCoroutine(MMFade.FadeCanvasGroup(achievementCanvasGroup, AchievementFadeDuration, 1f));
				yield return _achievementFadeOutWFS;
				StartCoroutine(MMFade.FadeCanvasGroup(achievementCanvasGroup, AchievementFadeDuration, 0f));
			}
		}
	}

	public virtual void OnMMEvent(MMAchievementUnlockedEvent achievementUnlockedEvent)
	{
		StartCoroutine(DisplayAchievement(achievementUnlockedEvent.Achievement));
	}

	protected virtual void OnEnable()
	{
		this.MMEventStartListening();
		_achievementFadeOutWFS = new WaitForSeconds(AchievementFadeDuration + AchievementDisplayDuration);
	}

	protected virtual void OnDisable()
	{
		this.MMEventStopListening();
	}
}
