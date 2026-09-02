using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
public abstract class ReactorColorTarget : MonoBehaviour
{
	public abstract Type targetType { get; }

	public abstract bool hasTarget { get; }

	public Color color
	{
		get
		{
			return GetColor();
		}
		set
		{
			SetColor(value);
		}
	}

	public abstract Color GetColor();

	public abstract void SetColor(Color value);

	public static ReactorColorTarget FindTarget(GameObject gameObject)
	{
		ReactorColorTarget[] components = gameObject.GetComponents<ReactorColorTarget>();
		ReactorColorTarget reactorColorTarget = ((components != null && components.Length != 0) ? components[0] : null);
		if (reactorColorTarget != null)
		{
			return reactorColorTarget;
		}
		Image component = gameObject.GetComponent<Image>();
		TMP_Text component2 = gameObject.GetComponent<TMP_Text>();
		Text component3 = gameObject.GetComponent<Text>();
		if ((bool)component)
		{
			return gameObject.AddComponent<ImageColorTarget>();
		}
		if ((bool)component2)
		{
			return gameObject.AddComponent<TextMeshProColorTarget>();
		}
		if ((bool)component3)
		{
			return gameObject.AddComponent<TextColorTarget>();
		}
		return gameObject.GetComponent<ReactorColorTarget>();
	}
}
