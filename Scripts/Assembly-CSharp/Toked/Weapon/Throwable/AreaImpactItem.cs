using System.Collections.Generic;
using Toked.StatusEffect;
using UnityEngine;

namespace Toked.Weapon.Throwable;

public class AreaImpactItem : AreaImpactItemBase
{
	[SerializeField]
	private new List<IEffectable> _effectableList = new List<IEffectable>();

	[SerializeField]
	private ParticleSystem _groundParticleSystem;

	private float _initStartSize;

	private void Awake()
	{
		_initStartSize = _groundParticleSystem.main.startSize.constantMax;
	}

	public override void Init(PlayerController playerController, float impactDuration = -1f, float impactDps = -1f)
	{
		base.Init(playerController, impactDuration, impactDps);
		ParticleSystem.MainModule main = _groundParticleSystem.main;
		main.startSize = new ParticleSystem.MinMaxCurve(0f, _initStartSize);
	}

	protected override void Release()
	{
		base.Release();
		FireSpawner.Instance.Release(this);
	}

	protected override void Reset()
	{
		ParticleSystem.MainModule main = _groundParticleSystem.main;
		main.startSize = new ParticleSystem.MinMaxCurve(0f, 0f);
		base.Reset();
	}
}
