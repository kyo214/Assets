using System;

namespace UnityEngine;

public enum RenderingPath
{
	UsePlayerSettings = -1,
	VertexLit = 0,
	Forward = 1,
	[Obsolete("DeferredLighting has been removed. Use DeferredShading, Forward or HDRP/URP instead.", false)]
	DeferredLighting = 2,
	DeferredShading = 3
}
