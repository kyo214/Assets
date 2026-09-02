using System;
using System.Linq;
using I2.Loc;
using UnityEngine;

namespace Toked.StatusEffect;

[Serializable]
public class StatusEffectData
{
	[SerializeField]
	private string _name;

	[SerializeField]
	[TermsPopup("")]
	private string _localizationName;

	[SerializeField]
	private Color _nameColor = new Color(0.5f, 0f, 0.5f, 1f);

	private string _additionalName = string.Empty;

	[SerializeField]
	private float _duration = 5f;

	[SerializeField]
	private GameObject _effectParticlePrefab;

	public string LocalizationName
	{
		get
		{
			return _localizationName;
		}
		set
		{
			_localizationName = value;
		}
	}

	public Color NameColor
	{
		get
		{
			return _nameColor;
		}
		set
		{
			_nameColor = value;
		}
	}

	public string BaseName
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public string AdditionalName => _additionalName;

	public string Name
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(_additionalName))
			{
				return _name + "_" + _additionalName;
			}
			return _name;
		}
	}

	public float Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			_duration = value;
		}
	}

	public GameObject EffectParticlePrefab
	{
		get
		{
			return _effectParticlePrefab;
		}
		set
		{
			_effectParticlePrefab = value;
		}
	}

	public void SetAdditionalName(params string[] additionalNames)
	{
		if (additionalNames != null && additionalNames.Length != 0)
		{
			_additionalName = string.Join("_", from name in additionalNames
				where !string.IsNullOrWhiteSpace(name)
				select name.Trim());
		}
	}
}
