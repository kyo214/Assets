using System;
using System.Linq;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class FeedbackHelpAttribute : Attribute
{
	public string HelpText;

	public FeedbackHelpAttribute(string helpText)
	{
		HelpText = helpText;
	}

	public static string GetFeedbackHelpText(Type type)
	{
		FeedbackHelpAttribute feedbackHelpAttribute = type.GetCustomAttributes(inherit: false).OfType<FeedbackHelpAttribute>().FirstOrDefault();
		if (feedbackHelpAttribute == null)
		{
			return "";
		}
		return feedbackHelpAttribute.HelpText;
	}
}
