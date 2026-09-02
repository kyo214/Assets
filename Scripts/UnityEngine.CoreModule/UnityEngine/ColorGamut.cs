using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/ColorGamut.h")]
public enum ColorGamut
{
	sRGB = 0,
	Rec709 = 1,
	Rec2020 = 2,
	DisplayP3 = 3,
	HDR10 = 4,
	DolbyHDR = 5,
	P3D65G22 = 6
}
