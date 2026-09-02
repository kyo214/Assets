using System;
using UnityEngine;

namespace Toked.Weapon;

[Serializable]
public class WeaponData
{
	[SerializeField]
	private int _weaponId;

	[SerializeField]
	private string _weaponName;

	[SerializeField]
	private string _weaponType;

	[SerializeField]
	private string _type;

	[SerializeField]
	private string _attackType;

	[SerializeField]
	private float _range;

	[SerializeField]
	private float _minRangeAccuracy;

	[SerializeField]
	private float _maxRangeAccuracy;

	[SerializeField]
	private float _durability;

	[SerializeField]
	private float _damage;

	[SerializeField]
	private float _needStamina;

	[SerializeField]
	private int _ammoTypeId;

	[SerializeField]
	private int _magazineSize;

	[SerializeField]
	private float _reloadTime;

	[SerializeField]
	private bool _reloadPerAmmo;

	[SerializeField]
	private int _maxItemInSlot;

	[SerializeField]
	private bool _isAutoFire;

	[SerializeField]
	private float _attackInterval;

	[SerializeField]
	private int _shotsPerAttack;

	[SerializeField]
	private int _shellsPerShot;

	[SerializeField]
	private int _bulletPerShell;

	[SerializeField]
	private int _radiusBulletSpread;

	[SerializeField]
	private int _bulletSize;

	[SerializeField]
	private float _impactAoESize;

	[SerializeField]
	private bool _knockbackAnimTrigger;

	[SerializeField]
	private bool _hitAnimTrigger;

	[SerializeField]
	private int _deadEnemyType;

	[SerializeField]
	private bool _isPumpAction;

	[SerializeField]
	private float _stuntTime;

	[SerializeField]
	private string _craftingStation;

	[SerializeField]
	private string _craftingMaterial0;

	[SerializeField]
	private string _craftingMaterial1;

	[SerializeField]
	private string _craftingMaterial2;

	[SerializeField]
	private float _durShake;

	[SerializeField]
	private float _amplitudeShake;

	[SerializeField]
	private float _baseWeaponID;

	public int WeaponId
	{
		get
		{
			return _weaponId;
		}
		set
		{
			_weaponId = value;
		}
	}

	public string WeaponName
	{
		get
		{
			return _weaponName;
		}
		set
		{
			_weaponName = value;
		}
	}

	public string WeaponType
	{
		get
		{
			return _weaponType;
		}
		set
		{
			_weaponType = value;
		}
	}

	public string Type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
		}
	}

	public string AttackType
	{
		get
		{
			return _attackType;
		}
		set
		{
			_attackType = value;
		}
	}

	public float Range
	{
		get
		{
			return _range;
		}
		set
		{
			_range = value;
		}
	}

	public float MinRangeAccuracy
	{
		get
		{
			return _minRangeAccuracy;
		}
		set
		{
			_minRangeAccuracy = value;
		}
	}

	public float MaxRangeAccuracy
	{
		get
		{
			return _maxRangeAccuracy;
		}
		set
		{
			_maxRangeAccuracy = value;
		}
	}

	public float Durability
	{
		get
		{
			return _durability;
		}
		set
		{
			_durability = value;
		}
	}

	public float Damage
	{
		get
		{
			return _damage;
		}
		set
		{
			_damage = value;
		}
	}

	public float NeedStamina
	{
		get
		{
			return _needStamina;
		}
		set
		{
			_needStamina = value;
		}
	}

	public int AmmoTypeId
	{
		get
		{
			return _ammoTypeId;
		}
		set
		{
			_ammoTypeId = value;
		}
	}

	public int MagazineSize
	{
		get
		{
			return _magazineSize;
		}
		set
		{
			_magazineSize = value;
		}
	}

	public float ReloadTime
	{
		get
		{
			return _reloadTime;
		}
		set
		{
			_reloadTime = value;
		}
	}

	public bool ReloadPerAmmo
	{
		get
		{
			return _reloadPerAmmo;
		}
		set
		{
			_reloadPerAmmo = value;
		}
	}

	public int MaxItemInSlot
	{
		get
		{
			return _maxItemInSlot;
		}
		set
		{
			_maxItemInSlot = value;
		}
	}

	public bool IsAutoFire
	{
		get
		{
			return _isAutoFire;
		}
		set
		{
			_isAutoFire = value;
		}
	}

	public float AttackInterval
	{
		get
		{
			return _attackInterval;
		}
		set
		{
			_attackInterval = value;
		}
	}

	public int ShotsPerAttack
	{
		get
		{
			return _shotsPerAttack;
		}
		set
		{
			_shotsPerAttack = value;
		}
	}

	public int ShellsPerShot
	{
		get
		{
			return _shellsPerShot;
		}
		set
		{
			_shellsPerShot = value;
		}
	}

	public int BulletPerShell
	{
		get
		{
			return _bulletPerShell;
		}
		set
		{
			_bulletPerShell = value;
		}
	}

	public int RadiusBulletSpread
	{
		get
		{
			return _radiusBulletSpread;
		}
		set
		{
			_radiusBulletSpread = value;
		}
	}

	public int BulletSize
	{
		get
		{
			return _bulletSize;
		}
		set
		{
			_bulletSize = value;
		}
	}

	public float ImpactAoESize
	{
		get
		{
			return _impactAoESize;
		}
		set
		{
			_impactAoESize = value;
		}
	}

	public bool KnockbackAnimTrigger
	{
		get
		{
			return _knockbackAnimTrigger;
		}
		set
		{
			_knockbackAnimTrigger = value;
		}
	}

	public bool HitAnimTrigger
	{
		get
		{
			return _hitAnimTrigger;
		}
		set
		{
			_hitAnimTrigger = value;
		}
	}

	public int DeadEnemyType
	{
		get
		{
			return _deadEnemyType;
		}
		set
		{
			_deadEnemyType = value;
		}
	}

	public bool IsPumpAction
	{
		get
		{
			return _isPumpAction;
		}
		set
		{
			_isPumpAction = value;
		}
	}

	public float StuntTime
	{
		get
		{
			return _stuntTime;
		}
		set
		{
			_stuntTime = value;
		}
	}

	public float DurShake
	{
		get
		{
			return _durShake;
		}
		set
		{
			_durShake = value;
		}
	}

	public float AmplitudeShake
	{
		get
		{
			return _amplitudeShake;
		}
		set
		{
			_amplitudeShake = value;
		}
	}

	public float BaseWeaponID
	{
		get
		{
			return _baseWeaponID;
		}
		set
		{
			_baseWeaponID = value;
		}
	}

	public void Set(BGDatabase_Weapon bgDatabaseWeapon)
	{
		Damage = bgDatabaseWeapon.Damage;
		WeaponName = bgDatabaseWeapon.Name;
		WeaponType = bgDatabaseWeapon.WeaponType;
		Type = bgDatabaseWeapon.Type;
		AttackType = bgDatabaseWeapon.AttackType;
		Range = bgDatabaseWeapon.floatRange;
		MinRangeAccuracy = bgDatabaseWeapon.MinRangeAccuracy;
		MaxRangeAccuracy = bgDatabaseWeapon.MaxRangeAccuracy;
		Durability = bgDatabaseWeapon.Durability;
		Damage = bgDatabaseWeapon.Damage;
		AmmoTypeId = bgDatabaseWeapon.AmmoTypeID;
		MagazineSize = bgDatabaseWeapon.MagazineSize;
		ReloadTime = bgDatabaseWeapon.ReloadTime;
		ReloadPerAmmo = bgDatabaseWeapon.ReloadPerAmmo;
		MaxItemInSlot = bgDatabaseWeapon.MaxItemInSlot;
		IsAutoFire = bgDatabaseWeapon.IsAutoFire;
		AttackInterval = bgDatabaseWeapon.AttackInterval;
		ShotsPerAttack = bgDatabaseWeapon.ShotsPerAttack;
		ShellsPerShot = bgDatabaseWeapon.ShellsPerShot;
		BulletPerShell = bgDatabaseWeapon.BulletPerShell;
		RadiusBulletSpread = (int)bgDatabaseWeapon.RadiusBulletSpread;
		BulletSize = (int)bgDatabaseWeapon.BulletSize;
		ImpactAoESize = bgDatabaseWeapon.ImpactAoESize;
		KnockbackAnimTrigger = bgDatabaseWeapon.KnockbackAnimTrigger;
		HitAnimTrigger = bgDatabaseWeapon.HitAnimTrigger;
		DeadEnemyType = bgDatabaseWeapon.DeadEnemyType;
		IsPumpAction = bgDatabaseWeapon.isPumpAction;
		StuntTime = bgDatabaseWeapon.StuntTime;
		DurShake = bgDatabaseWeapon.DurShake;
		AmplitudeShake = bgDatabaseWeapon.AmplitudeShake;
		BaseWeaponID = bgDatabaseWeapon.BaseWeaponID;
	}
}
