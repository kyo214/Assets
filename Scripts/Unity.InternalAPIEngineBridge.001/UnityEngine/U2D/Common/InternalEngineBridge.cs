using System;
using Unity.Collections;
using UnityEngine.Animations;

namespace UnityEngine.U2D.Common;

internal static class InternalEngineBridge
{
	public static void SetLocalAABB(SpriteRenderer spriteRenderer, Bounds aabb)
	{
		spriteRenderer.SetLocalAABB(aabb);
	}

	public static void SetDeformableBuffer(SpriteRenderer spriteRenderer, NativeArray<byte> src)
	{
		spriteRenderer.SetDeformableBuffer(src);
	}

	public static bool IsUsingDeformableBuffer(SpriteRenderer spriteRenderer, IntPtr buffer)
	{
		return spriteRenderer.IsUsingDeformableBuffer(buffer);
	}

	public static Vector2 GUIUnclip(Vector2 v)
	{
		return GUIClip.Unclip(v);
	}

	public static Rect GetGUIClipTopMostRect()
	{
		return GUIClip.topmostRect;
	}

	public static Rect GetGUIClipTopRect()
	{
		return GUIClip.GetTopRect();
	}

	public static Rect GetGUIClipVisibleRect()
	{
		return GUIClip.visibleRect;
	}

	public static void SetBatchDeformableBufferAndLocalAABBArray(SpriteRenderer[] spriteRenderers, NativeArray<IntPtr> buffers, NativeArray<int> bufferSizes, NativeArray<Bounds> bounds)
	{
		SpriteRendererDataAccessExtensions.SetBatchDeformableBufferAndLocalAABBArray(spriteRenderers, buffers, bufferSizes, bounds);
	}

	public static int ConvertFloatToInt(float f)
	{
		return DiscreteEvaluationAttributeUtilities.ConvertFloatToDiscreteInt(f);
	}

	public static float ConvertIntToFloat(int i)
	{
		return DiscreteEvaluationAttributeUtilities.ConvertDiscreteIntToFloat(i);
	}
}
