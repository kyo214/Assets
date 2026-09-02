using System;

namespace MoreMountains.Tools;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class MMDebugLogCommandAttribute : Attribute
{
}
