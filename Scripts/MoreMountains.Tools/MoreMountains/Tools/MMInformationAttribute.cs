using UnityEngine;

namespace MoreMountains.Tools;

public class MMInformationAttribute : PropertyAttribute
{
	public enum InformationType
	{
		Error = 0,
		Info = 1,
		None = 2,
		Warning = 3
	}

	public MMInformationAttribute(string message, InformationType type, bool messageAfterProperty)
	{
	}
}
