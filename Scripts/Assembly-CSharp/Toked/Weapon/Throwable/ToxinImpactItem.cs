using System.Collections;
using UnityEngine;

namespace Toked.Weapon.Throwable;

public class ToxinImpactItem : AreaImpactItemBase
{
	[SerializeField]
	private SO_MissionModifierStatus modifierToxicSpill;

	[SerializeField]
	private bool IsToxinSpill;

	private IEnumerator Start()
	{
		if (IsToxinSpill)
		{
			while (GameManagerPhoton.Instance == null || GameManagerPhoton.Instance.CurrentMission == null || NetworkGameManager.Instance.isSyncingMissionMap)
			{
				yield return null;
			}
			bool flag = false;
			for (int i = 0; i < GameManagerPhoton.Instance.CurrentMission.ListModifier.Count; i++)
			{
				for (int j = 0; j < GameManagerPhoton.Instance.CurrentMission.ListModifier[i].Modifier.Count; j++)
				{
					if (GameManagerPhoton.Instance.CurrentMission.ListModifier[i].Modifier[j].ModifierStatus == modifierToxicSpill && modifierToxicSpill.CurrentValue >= 1f)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				Reset();
				_isColliderActive = true;
				base.gameObject.SetActive(value: true);
			}
			else
			{
				Reset();
				_isColliderActive = false;
				base.gameObject.SetActive(value: false);
			}
		}
		else
		{
			_isColliderActive = true;
		}
	}

	protected override void FixedUpdate()
	{
		if (!(modifierToxicSpill != null))
		{
			base.FixedUpdate();
		}
	}

	protected override void Release()
	{
		base.Release();
		ToxinSpawner.Instance.Release(this);
	}
}
