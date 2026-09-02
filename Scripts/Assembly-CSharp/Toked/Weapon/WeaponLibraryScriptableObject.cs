using UnityEngine;

namespace Toked.Weapon;

[CreateAssetMenu(fileName = "WeaponLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Weapons/Weapon Library", order = 0)]
public class WeaponLibraryScriptableObject : ScriptableObjectLibraryBase<Weapon>
{
	public Weapon GetData(int key)
	{
		foreach (Weapon data in base.DataList)
		{
			if (data.WeaponData.WeaponId == key)
			{
				return data;
			}
		}
		return null;
	}

	public override void SortData()
	{
		base.DataList.Sort((Weapon x, Weapon y) => x.WeaponData.WeaponId.CompareTo(y.WeaponData.WeaponId));
	}

	protected override void UpdateData(Weapon data)
	{
		data.WeaponData.Set(BGDatabase_Weapon.GetEntity(data.WeaponData.WeaponId.ToString()));
	}
}
