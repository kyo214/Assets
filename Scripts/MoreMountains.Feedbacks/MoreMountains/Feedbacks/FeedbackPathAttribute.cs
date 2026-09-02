using System;
using System.Linq;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class FeedbackPathAttribute : Attribute
{
	public string Path;

	public string Name;

	public FeedbackPathAttribute(string path)
	{
		Path = path;
		Name = path.Split('/').Last();
	}

	public static string GetFeedbackDefaultName(Type type)
	{
		FeedbackPathAttribute feedbackPathAttribute = type.GetCustomAttributes(inherit: false).OfType<FeedbackPathAttribute>().FirstOrDefault();
		if (feedbackPathAttribute == null)
		{
			return type.Name;
		}
		return feedbackPathAttribute.Name;
	}

	public static string GetFeedbackDefaultPath(Type type)
	{
		return type.GetCustomAttributes(inherit: false).OfType<FeedbackPathAttribute>().FirstOrDefault()?.Path;
	}
}
