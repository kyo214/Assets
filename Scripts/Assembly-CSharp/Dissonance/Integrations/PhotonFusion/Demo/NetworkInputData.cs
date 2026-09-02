using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

namespace Dissonance.Integrations.PhotonFusion.Demo;

[StructLayout(LayoutKind.Explicit, Size = 12)]
[NetworkInputWeaved(3)]
public struct NetworkInputData : INetworkInput
{
	[FieldOffset(0)]
	public Vector3 direction;
}
