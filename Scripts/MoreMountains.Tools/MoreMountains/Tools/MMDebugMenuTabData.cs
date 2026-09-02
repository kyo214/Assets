using System;

namespace MoreMountains.Tools;

[Serializable]
public class MMDebugMenuTabData
{
	public string Name = "TabName";

	public bool Active = true;

	[MMReorderableAttribute]
	public MMDebugMenuItemList MenuItems;
}
