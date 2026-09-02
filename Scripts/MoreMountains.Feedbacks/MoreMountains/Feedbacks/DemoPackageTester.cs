using UnityEngine;

namespace MoreMountains.Feedbacks;

public class DemoPackageTester : MonoBehaviour
{
	[MMFInformation("This component is only used to display an error in the console in case dependencies for this demo haven't been installed. You can safely remove it if you want, and typically you wouldn't want to keep that in your own game.", MMFInformationAttribute.InformationType.Warning, false)]
	public bool RequiresPostProcessing;

	public bool RequiresTMP;

	public bool RequiresCinemachine;

	protected virtual void Awake()
	{
	}

	protected virtual void TestForDependencies()
	{
		_ = 0;
		string text = "";
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		flag = true;
		flag2 = true;
		if (RequiresCinemachine && !flag)
		{
			text += "Cinemachine";
		}
		if (RequiresTMP && !flag2)
		{
			if (text != "")
			{
				text += ", ";
			}
			text += "TextMeshPro";
		}
		if (RequiresPostProcessing && !flag3)
		{
			if (text != "")
			{
				text += ", ";
			}
			text += "PostProcessing";
		}
	}
}
