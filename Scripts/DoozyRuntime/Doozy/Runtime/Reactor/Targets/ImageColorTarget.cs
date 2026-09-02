using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
[RequireComponent(typeof(Image))]
[AddComponentMenu("Reactor/Targets/Image Color Target")]
public class ImageColorTarget : ReactorMetaColorTarget<Image>
{
	public override Type targetType => typeof(Image);

	public override Color GetColor()
	{
		if (!(Target == null))
		{
			return Target.color;
		}
		return Color.magenta;
	}

	public override void SetColor(Color value)
	{
		if (!(Target == null))
		{
			Target.color = value;
		}
	}
}
