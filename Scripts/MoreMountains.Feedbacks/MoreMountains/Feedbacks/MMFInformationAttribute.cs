using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMFInformationAttribute : PropertyAttribute
{
	public enum InformationType
	{
		Error = 0,
		Info = 1,
		None = 2,
		Warning = 3
	}

	public MMFInformationAttribute(string message, InformationType type, bool messageAfterProperty)
	{
	}
}
