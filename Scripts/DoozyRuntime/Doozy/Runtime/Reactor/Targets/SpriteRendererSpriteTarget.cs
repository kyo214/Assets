using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu("Reactor/Targets/SpriteRenderer Sprite Target")]
public class SpriteRendererSpriteTarget : ReactorMetaSpriteTarget<SpriteRenderer>
{
	public override Type targetType => typeof(SpriteRenderer);

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
