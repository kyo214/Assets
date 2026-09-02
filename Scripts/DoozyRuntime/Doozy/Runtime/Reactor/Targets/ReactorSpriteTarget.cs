using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
public abstract class ReactorSpriteTarget : MonoBehaviour
{
	public abstract Type targetType { get; }

	public abstract bool hasTarget { get; }

	public int currentFrame { get; set; }

	public Sprite current { get; set; }

	public Sprite sprite
	{
		get
		{
			return GetSprite();
		}
		set
		{
			SetSprite(value);
		}
	}

	public abstract Sprite GetSprite();

	public abstract void SetSprite(Sprite value);

	public static ReactorSpriteTarget FindTarget(GameObject gameObject)
	{
		ReactorSpriteTarget[] components = gameObject.GetComponents<ReactorSpriteTarget>();
		ReactorSpriteTarget reactorSpriteTarget = ((components != null && components.Length != 0) ? components[0] : null);
		if (reactorSpriteTarget != null)
		{
			return reactorSpriteTarget;
		}
		Image component = gameObject.GetComponent<Image>();
		SpriteMask component2 = gameObject.GetComponent<SpriteMask>();
		SpriteRenderer component3 = gameObject.GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			return gameObject.AddComponent<ImageSpriteTarget>();
		}
		if ((bool)component2)
		{
			return gameObject.AddComponent<SpriteMaskSpriteTarget>();
		}
		if ((bool)component3)
		{
			return gameObject.AddComponent<SpriteRendererSpriteTarget>();
		}
		return gameObject.GetComponent<ReactorSpriteTarget>();
	}
}
