using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
[RequireComponent(typeof(SpriteMask))]
[AddComponentMenu("Reactor/Targets/SpriteMask Sprite Target")]
public class SpriteMaskSpriteTarget : ReactorMetaSpriteTarget<SpriteMask>
{
	public override Type targetType => typeof(SpriteMask);

	public override Sprite GetSprite()
	{
		if (!(Target == null))
		{
			return Target.sprite;
		}
		return null;
	}

	public override void SetSprite(Sprite value)
	{
		if (!(Target == null))
		{
			Target.sprite = value;
		}
	}
}
