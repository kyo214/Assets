namespace Toked.Weapon.Throwable;

public class RadiationImpactItem : AreaImpactItemBase
{
	protected override void FixedUpdate()
	{
	}

	private void OnDisable()
	{
		Reset();
		Release();
	}
}
