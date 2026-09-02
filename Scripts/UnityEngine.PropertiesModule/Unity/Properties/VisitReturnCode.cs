namespace Unity.Properties;

public enum VisitReturnCode
{
	Ok = 0,
	NullContainer = 1,
	InvalidContainerType = 2,
	MissingPropertyBag = 3,
	InvalidPath = 4,
	InvalidCast = 5,
	AccessViolation = 6
}
