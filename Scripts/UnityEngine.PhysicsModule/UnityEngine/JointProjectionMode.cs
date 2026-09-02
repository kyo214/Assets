using System;
using System.ComponentModel;

namespace UnityEngine;

public enum JointProjectionMode
{
	None = 0,
	PositionAndRotation = 1,
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("JointProjectionMode.PositionOnly is no longer supported", true)]
	PositionOnly = 2
}
