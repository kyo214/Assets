using System.Security.Permissions;

namespace System.Reflection.Emit;

internal struct RefEmitPermissionSet(SecurityAction action, string pset)
{
	public SecurityAction action = action;

	public string pset = pset;
}
