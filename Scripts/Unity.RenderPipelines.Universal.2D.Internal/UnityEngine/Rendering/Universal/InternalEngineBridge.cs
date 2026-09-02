using System;

namespace UnityEngine.Rendering.Universal;

internal static class InternalEngineBridge
{
	internal static void AddOnLayerchangedCallback(Action callback)
	{
		SortingLayer.onLayerChanged = (Action)Delegate.Combine(SortingLayer.onLayerChanged, callback);
	}

	internal static void RemoveOnLayerchangedCallback(Action callback)
	{
		SortingLayer.onLayerChanged = (Action)Delegate.Remove(SortingLayer.onLayerChanged, callback);
	}
}
