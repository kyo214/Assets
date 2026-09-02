using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
public class MMF_PlayerEnabler : MonoBehaviour
{
	public MMF_Player TargetMmfPlayer { get; set; }

	protected virtual void OnEnable()
	{
		if (TargetMmfPlayer != null && !TargetMmfPlayer.enabled && TargetMmfPlayer.AutoPlayOnEnable)
		{
			TargetMmfPlayer.enabled = true;
		}
	}
}
