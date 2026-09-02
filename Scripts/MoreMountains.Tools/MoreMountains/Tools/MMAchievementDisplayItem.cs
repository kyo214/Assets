using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Achievements/MMAchievementDisplayItem")]
public class MMAchievementDisplayItem : MonoBehaviour
{
	public Image BackgroundLocked;

	public Image BackgroundUnlocked;

	public Image Icon;

	public Text Title;

	public Text Description;

	public MMProgressBar ProgressBarDisplay;
}
