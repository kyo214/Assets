using System.Collections.Generic;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Runtime.Reactor.Extensions;

public static class VisualElementExtensions
{
	public static ColorReaction GetColorReaction(this VisualElement target, PropertySetter<Color> setter)
	{
		return Reaction.Get<ColorReaction>().SetTargetObject(target).SetSetter(setter);
	}

	public static FloatReaction GetFloatReaction(this VisualElement target, PropertySetter<float> setter)
	{
		return Reaction.Get<FloatReaction>().SetTargetObject(target).SetSetter(setter);
	}

	public static Texture2DReaction GetTexture2DReaction(this VisualElement target, IEnumerable<Texture2D> textures = null)
	{
		Texture2DReaction texture2DReaction = Reaction.Get<Texture2DReaction>().SetTargetObject(target).SetSetter((Texture2D value) =>
		{
			target.SetStyleBackgroundImage(value);
		});
		if (textures != null)
		{
			texture2DReaction.SetTextures(textures);
		}
		return texture2DReaction;
	}

	public static IntReaction GetIntReaction(this VisualElement target, PropertySetter<int> setter)
	{
		return Reaction.Get<IntReaction>().SetTargetObject(target).SetSetter(setter);
	}

	public static Vector2Reaction GetVector2Reaction(this VisualElement target, PropertySetter<Vector2> setter)
	{
		return Reaction.Get<Vector2Reaction>().SetTargetObject(target).SetSetter(setter);
	}

	public static Vector3Reaction GetVector3Reaction(this VisualElement target, PropertySetter<Vector3> setter)
	{
		return Reaction.Get<Vector3Reaction>().SetTargetObject(target).SetSetter(setter);
	}
}
