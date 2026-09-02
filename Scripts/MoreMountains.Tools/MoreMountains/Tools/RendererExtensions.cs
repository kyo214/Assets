using UnityEngine;

namespace MoreMountains.Tools;

public static class RendererExtensions
{
	public static bool MMIsVisibleFrom(this Renderer renderer, Camera camera)
	{
		return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
	}
}
