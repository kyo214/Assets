using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
[AddComponentMenu("Reactor/Targets/Text Color Target")]
public class TextColorTarget : ReactorMetaColorTarget<Text>
{
	public override Type targetType => typeof(Text);

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
