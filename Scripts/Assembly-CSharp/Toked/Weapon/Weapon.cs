using UnityEngine;

namespace Toked.Weapon;

public abstract class Weapon : ScriptableObject, IWeapon
{
	[SerializeField]
	protected WeaponData _weaponData;

	public WeaponData WeaponData
	{
		get
		{
			return _weaponData;
		}
		set
		{
			_weaponData = value;
		}
	}

	public abstract void Attack(PlayerController playerController);

	public abstract void Damage(PlayerController playerController);
}
