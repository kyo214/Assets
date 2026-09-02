using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public static class MMF_FieldInfo
{
	public static Dictionary<int, List<FieldInfo>> FieldInfoList = new Dictionary<int, List<FieldInfo>>();

	public static int GetFieldInfo(MMF_Feedback target, out List<FieldInfo> fieldInfoList)
	{
		Type type = target.GetType();
		int hashCode = type.GetHashCode();
		if (!FieldInfoList.TryGetValue(hashCode, out fieldInfoList))
		{
			IList<Type> typeTree = type.GetBaseTypes();
			fieldInfoList = (from x in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				orderby typeTree.IndexOf(x.DeclaringType) descending
				select x).ToList();
			FieldInfoList.Add(hashCode, fieldInfoList);
		}
		return fieldInfoList.Count;
	}

	public static int GetFieldInfo(UnityEngine.Object target, out List<FieldInfo> fieldInfoList)
	{
		Type type = target.GetType();
		int hashCode = type.GetHashCode();
		if (!FieldInfoList.TryGetValue(hashCode, out fieldInfoList))
		{
			IList<Type> typeTree = type.GetBaseTypes();
			fieldInfoList = (from x in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				orderby typeTree.IndexOf(x.DeclaringType) descending
				select x).ToList();
			FieldInfoList.Add(hashCode, fieldInfoList);
		}
		return fieldInfoList.Count;
	}

	public static IList<Type> GetBaseTypes(this Type t)
	{
		List<Type> list = new List<Type>();
		while (t.BaseType != null)
		{
			list.Add(t);
			t = t.BaseType;
		}
		return list;
	}
}
