using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
[RequireComponent(typeof(Image))]
[AddComponentMenu("Reactor/Targets/Image Sprite Target")]
public class ImageSpriteTarget : ReactorMetaSpriteTarget<Image>
{
	public override Type targetType => typeof(Image);

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
