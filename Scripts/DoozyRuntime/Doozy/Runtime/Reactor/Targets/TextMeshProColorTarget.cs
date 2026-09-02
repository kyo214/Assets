using System;
using TMPro;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
[AddComponentMenu("Reactor/Targets/TextMeshPro Color Target")]
public class TextMeshProColorTarget : ReactorMetaColorTarget<TMP_Text>
{
	public override Type targetType => typeof(TMP_Text);

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
