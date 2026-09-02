using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Sirenix.Utilities;

public static class DeepReflection
{
	private enum PathStepType
	{
		Member = 0,
		WeakListElement = 1,
		StrongListElement = 2,
		ArrayElement = 3
	}

	private struct PathStep
	{
		public readonly PathStepType StepType;

		public readonly MemberInfo Member;

		public readonly int ElementIndex;

		public readonly Type ElementType;

		public readonly MethodInfo StrongListGetItemMethod;

		public PathStep(MemberInfo member)
		{
			StepType = PathStepType.Member;
			Member = member;
			ElementIndex = -1;
			ElementType = null;
			StrongListGetItemMethod = null;
		}

		public PathStep(int elementIndex)
		{
			StepType = PathStepType.WeakListElement;
			Member = null;
			ElementIndex = elementIndex;
			ElementType = null;
			StrongListGetItemMethod = null;
		}

		public PathStep(int elementIndex, Type strongListElementType, bool isArray)
		{
			StepType = (isArray ? PathStepType.ArrayElement : PathStepType.StrongListElement);
			Member = null;
			ElementIndex = elementIndex;
			ElementType = strongListElementType;
			StrongListGetItemMethod = typeof(IList<>).MakeGenericType(strongListElementType).GetMethod("get_Item");
		}
	}

	private static MethodInfo WeakListGetItem = typeof(IList).GetMethod("get_Item");

	private static MethodInfo WeakListSetItem = typeof(IList).GetMethod("set_Item");

	private static MethodInfo CreateWeakAliasForInstanceGetDelegate1MethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForInstanceGetDelegate1", BindingFlags.Static | BindingFlags.NonPublic);

	private static MethodInfo CreateWeakAliasForInstanceGetDelegate2MethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForInstanceGetDelegate2", BindingFlags.Static | BindingFlags.NonPublic);

	private static MethodInfo CreateWeakAliasForStaticGetDelegateMethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForStaticGetDelegate", BindingFlags.Static | BindingFlags.NonPublic);

	private static MethodInfo CreateWeakAliasForInstanceSetDelegate1MethodInfo = typeof(DeepReflection).GetMethod("CreateWeakAliasForInstanceSetDelegate1", BindingFlags.Static | BindingFlags.NonPublic);

	public static Func<object> CreateWeakStaticValueGetter(Type rootType, Type resultType, string path, bool allowEmit = true)
	{
		if (rootType == null)
		{
			throw new ArgumentNullException("rootType");
		}
		List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out var rootIsStatic, isSet: false);
		if (!rootIsStatic)
		{
			throw new ArgumentException("Given path root is not static.");
		}
		if (!allowEmit)
		{
			return CreateSlowDeepStaticValueGetterDelegate(memberPath);
		}
		Delegate obj = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
		MethodInfo methodInfo = CreateWeakAliasForStaticGetDelegateMethodInfo.MakeGenericMethod(resultType);
		return (Func<object>)methodInfo.Invoke(null, new object[1] { obj });
	}

	public static Func<object, object> CreateWeakInstanceValueGetter(Type rootType, Type resultType, string path, bool allowEmit = true)
	{
		if (rootType == null)
		{
			throw new ArgumentNullException("rootType");
		}
		List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out var rootIsStatic, isSet: false);
		if (rootIsStatic)
		{
			throw new ArgumentException("Given path root is static.");
		}
		if (!allowEmit)
		{
			return CreateSlowDeepInstanceValueGetterDelegate(memberPath);
		}
		Delegate obj = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
		MethodInfo methodInfo = CreateWeakAliasForInstanceGetDelegate1MethodInfo.MakeGenericMethod(rootType, resultType);
		return (Func<object, object>)methodInfo.Invoke(null, new object[1] { obj });
	}

	public static Action<object, object> CreateWeakInstanceValueSetter(Type rootType, Type argType, string path, bool allowEmit = true)
	{
		if (rootType == null)
		{
			throw new ArgumentNullException("rootType");
		}
		List<PathStep> memberPath = GetMemberPath(rootType, ref argType, path, out var rootIsStatic, isSet: true);
		if (rootIsStatic)
		{
			throw new ArgumentException("Given path root is static.");
		}
		allowEmit = false;
		if (!allowEmit)
		{
			return CreateSlowDeepInstanceValueSetterDelegate(memberPath);
		}
		Delegate obj = null;
		MethodInfo methodInfo = CreateWeakAliasForInstanceSetDelegate1MethodInfo.MakeGenericMethod(rootType, argType);
		return (Action<object, object>)methodInfo.Invoke(null, new object[1] { obj });
	}

	public static Func<object, TResult> CreateWeakInstanceValueGetter<TResult>(Type rootType, string path, bool allowEmit = true)
	{
		if (rootType == null)
		{
			throw new ArgumentNullException("rootType");
		}
		Type resultType = typeof(TResult);
		List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out var rootIsStatic, isSet: false);
		if (rootIsStatic)
		{
			throw new ArgumentException("Given path root is static.");
		}
		if (!allowEmit)
		{
			Func<object, object> del = CreateSlowDeepInstanceValueGetterDelegate(memberPath);
			return (object arg) => (TResult)del(arg);
		}
		Delegate obj = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
		MethodInfo methodInfo = CreateWeakAliasForInstanceGetDelegate2MethodInfo.MakeGenericMethod(rootType, resultType);
		return (Func<object, TResult>)methodInfo.Invoke(null, new object[1] { obj });
	}

	public static Func<TResult> CreateValueGetter<TResult>(Type rootType, string path, bool allowEmit = true)
	{
		if (rootType == null)
		{
			throw new ArgumentNullException("rootType");
		}
		Type resultType = typeof(TResult);
		List<PathStep> memberPath = GetMemberPath(rootType, ref resultType, path, out var rootIsStatic, isSet: false);
		if (!rootIsStatic)
		{
			throw new ArgumentException("Given path root is not static; use the generic overload with a target type.");
		}
		if (!allowEmit)
		{
			Func<object> slowDelegate = CreateSlowDeepStaticValueGetterDelegate(memberPath);
			return () => (TResult)slowDelegate();
		}
		Delegate obj = CreateEmittedDeepValueGetterDelegate(path, rootType, resultType, memberPath, rootIsStatic);
		return (Func<TResult>)obj;
	}

	public static Func<TTarget, TResult> CreateValueGetter<TTarget, TResult>(string path, bool allowEmit = true)
	{
		Type resultType = typeof(TResult);
		List<PathStep> memberPath = GetMemberPath(typeof(TTarget), ref resultType, path, out var rootIsStatic, isSet: false);
		if (rootIsStatic)
		{
			throw new ArgumentException("Given path root is static; use the generic overload without a target type.");
		}
		if (!allowEmit)
		{
			Func<object, object> slowDelegate = CreateSlowDeepInstanceValueGetterDelegate(memberPath);
			return (TTarget target) => (TResult)slowDelegate(target);
		}
		Delegate obj = CreateEmittedDeepValueGetterDelegate(path, typeof(TTarget), resultType, memberPath, rootIsStatic);
		return (Func<TTarget, TResult>)obj;
	}

	private static Func<object, object> CreateWeakAliasForInstanceGetDelegate1<TTarget, TResult>(Func<TTarget, TResult> func)
	{
		return (object obj) => func((TTarget)obj);
	}

	private static Func<object, TResult> CreateWeakAliasForInstanceGetDelegate2<TTarget, TResult>(Func<TTarget, TResult> func)
	{
		return (object obj) => func((TTarget)obj);
	}

	private static Func<object> CreateWeakAliasForStaticGetDelegate<TResult>(Func<TResult> func)
	{
		return () => func();
	}

	private static Action<object, object> CreateWeakAliasForInstanceSetDelegate1<TTarget, TArg1>(Action<TTarget, TArg1> func)
	{
		return (object obj, object arg) =>
		{
			func((TTarget)obj, (TArg1)arg);
		};
	}

	private static Action<object, TArg1> CreateWeakAliasForInstanceSetDelegate2<TTarget, TArg1>(Action<TTarget, TArg1> func)
	{
		return (object obj, TArg1 arg) =>
		{
			func((TTarget)obj, arg);
		};
	}

	private static Action<object> CreateWeakAliasForStaticSetDelegate<TArg1>(Action<TArg1> func)
	{
		return (object arg) =>
		{
			func((TArg1)arg);
		};
	}

	private static Delegate CreateEmittedDeepValueGetterDelegate(string path, Type rootType, Type resultType, List<PathStep> memberPath, bool rootIsStatic)
	{
		DynamicMethod dynamicMethod = ((!rootIsStatic) ? new DynamicMethod(rootType.FullName + "_getter<" + path + ">", resultType, new Type[1] { rootType }, restrictedSkipVisibility: true) : new DynamicMethod(rootType.FullName + "_getter<" + path + ">", resultType, new Type[0], restrictedSkipVisibility: true));
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		if (!rootIsStatic)
		{
			iLGenerator.Emit(OpCodes.Ldarg_0);
		}
		for (int i = 0; i < memberPath.Count; i++)
		{
			PathStep pathStep = memberPath[i];
			switch (pathStep.StepType)
			{
			case PathStepType.Member:
			{
				MemberInfo member = pathStep.Member;
				FieldInfo fieldInfo = member as FieldInfo;
				if (fieldInfo != null)
				{
					if (fieldInfo.IsLiteral)
					{
						EmitConstant(iLGenerator, fieldInfo.GetRawConstantValue());
					}
					else if (fieldInfo.IsStatic)
					{
						iLGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
					}
				}
				PropertyInfo propertyInfo = member as PropertyInfo;
				if (propertyInfo != null)
				{
					MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
					if (getMethod.IsStatic)
					{
						iLGenerator.Emit(OpCodes.Call, getMethod);
					}
					else if (getMethod.DeclaringType.IsValueType)
					{
						LocalBuilder local = iLGenerator.DeclareLocal(getMethod.DeclaringType);
						iLGenerator.Emit(OpCodes.Stloc, local);
						iLGenerator.Emit(OpCodes.Ldloca, local);
						iLGenerator.Emit(OpCodes.Call, getMethod);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Callvirt, getMethod);
					}
				}
				MethodInfo methodInfo = member as MethodInfo;
				if (methodInfo != null)
				{
					if (methodInfo.IsStatic)
					{
						iLGenerator.Emit(OpCodes.Call, methodInfo);
					}
					else if (methodInfo.DeclaringType.IsValueType)
					{
						LocalBuilder local2 = iLGenerator.DeclareLocal(methodInfo.DeclaringType);
						iLGenerator.Emit(OpCodes.Stloc, local2);
						iLGenerator.Emit(OpCodes.Ldloca, local2);
						iLGenerator.Emit(OpCodes.Call, methodInfo);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
					}
				}
				Type returnType = member.GetReturnType();
				if ((resultType == typeof(object) || returnType.IsInterface) && returnType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, returnType);
				}
				break;
			}
			case PathStepType.ArrayElement:
				iLGenerator.Emit(OpCodes.Ldc_I4, pathStep.ElementIndex);
				iLGenerator.Emit(OpCodes.Ldelem, pathStep.ElementType);
				break;
			case PathStepType.WeakListElement:
				iLGenerator.Emit(OpCodes.Ldc_I4, pathStep.ElementIndex);
				iLGenerator.Emit(OpCodes.Callvirt, WeakListGetItem);
				break;
			case PathStepType.StrongListElement:
			{
				Type type = typeof(IList<>).MakeGenericType(pathStep.ElementType);
				MethodInfo method = type.GetMethod("get_Item");
				iLGenerator.Emit(OpCodes.Ldc_I4, pathStep.ElementIndex);
				iLGenerator.Emit(OpCodes.Callvirt, method);
				break;
			}
			}
		}
		iLGenerator.Emit(OpCodes.Ret);
		if (rootIsStatic)
		{
			return dynamicMethod.CreateDelegate(typeof(Func<>).MakeGenericType(resultType));
		}
		return dynamicMethod.CreateDelegate(typeof(Func<, >).MakeGenericType(rootType, resultType));
	}

	private static Func<object> CreateSlowDeepStaticValueGetterDelegate(List<PathStep> memberPath)
	{
		return () =>
		{
			object obj = null;
			for (int i = 0; i < memberPath.Count; i++)
			{
				obj = SlowGetMemberValue(memberPath[i], obj);
			}
			return obj;
		};
	}

	private static Func<object, object> CreateSlowDeepInstanceValueGetterDelegate(List<PathStep> memberPath)
	{
		return (object instance) =>
		{
			object obj = instance;
			for (int i = 0; i < memberPath.Count; i++)
			{
				obj = SlowGetMemberValue(memberPath[i], obj);
			}
			return obj;
		};
	}

	private static Action<object, object> CreateSlowDeepInstanceValueSetterDelegate(List<PathStep> memberPath)
	{
		return (object instance, object arg) =>
		{
			object instance2 = instance;
			int num = memberPath.Count - 1;
			for (int i = 0; i < num; i++)
			{
				instance2 = SlowGetMemberValue(memberPath[i], instance2);
			}
			SlowSetMemberValue(memberPath[memberPath.Count - 1], instance2, arg);
		};
	}

	private static object SlowGetMemberValue(PathStep step, object instance)
	{
		switch (step.StepType)
		{
		case PathStepType.Member:
		{
			FieldInfo fieldInfo = step.Member as FieldInfo;
			if (fieldInfo != null)
			{
				if (fieldInfo.IsLiteral)
				{
					return fieldInfo.GetRawConstantValue();
				}
				return fieldInfo.GetValue(instance);
			}
			PropertyInfo propertyInfo = step.Member as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.GetValue(instance, null);
			}
			MethodInfo methodInfo = step.Member as MethodInfo;
			if (methodInfo != null)
			{
				return methodInfo.Invoke(instance, null);
			}
			throw new NotSupportedException(step.Member.GetType().GetNiceName());
		}
		case PathStepType.WeakListElement:
			return WeakListGetItem.Invoke(instance, new object[1] { step.ElementIndex });
		case PathStepType.ArrayElement:
			return (instance as Array).GetValue(step.ElementIndex);
		case PathStepType.StrongListElement:
			return step.StrongListGetItemMethod.Invoke(instance, new object[1] { step.ElementIndex });
		default:
			throw new NotImplementedException(step.StepType.ToString());
		}
	}

	private static void SlowSetMemberValue(PathStep step, object instance, object value)
	{
		switch (step.StepType)
		{
		case PathStepType.Member:
		{
			FieldInfo fieldInfo = step.Member as FieldInfo;
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(instance, value);
				break;
			}
			PropertyInfo propertyInfo = step.Member as PropertyInfo;
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(instance, value, null);
				break;
			}
			throw new NotSupportedException(step.Member.GetType().GetNiceName());
		}
		case PathStepType.WeakListElement:
			WeakListSetItem.Invoke(instance, new object[2] { step.ElementIndex, value });
			break;
		case PathStepType.ArrayElement:
			(instance as Array).SetValue(value, step.ElementIndex);
			break;
		case PathStepType.StrongListElement:
		{
			MethodInfo method = typeof(IList<>).MakeGenericType(step.ElementType).GetMethod("set_Item");
			method.Invoke(instance, new object[2] { step.ElementIndex, value });
			break;
		}
		default:
			throw new NotImplementedException(step.StepType.ToString());
		}
	}

	private static List<PathStep> GetMemberPath(Type rootType, ref Type resultType, string path, out bool rootIsStatic, bool isSet)
	{
		if (path.IsNullOrWhitespace())
		{
			throw new ArgumentException("Invalid path; is null or whitespace.");
		}
		rootIsStatic = false;
		List<PathStep> list = new List<PathStep>();
		string[] array = path.Split(new char[1] { '.' });
		Type type = rootType;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			bool flag = false;
			if (text.StartsWith("[", StringComparison.InvariantCulture) && text.EndsWith("]", StringComparison.InvariantCulture))
			{
				string s = text.Substring(1, text.Length - 2);
				if (!int.TryParse(s, out var result))
				{
					throw new ArgumentException("Couldn't parse an index from the path step '" + text + "'.");
				}
				if (type.IsArray)
				{
					Type elementType = type.GetElementType();
					list.Add(new PathStep(result, elementType, isArray: true));
					type = elementType;
					continue;
				}
				if (type.ImplementsOpenGenericInterface(typeof(IList<>)))
				{
					Type type2 = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
					list.Add(new PathStep(result, type2, isArray: false));
					type = type2;
					continue;
				}
				if (typeof(IList).IsAssignableFrom(type))
				{
					list.Add(new PathStep(result));
					type = typeof(object);
					continue;
				}
				throw new ArgumentException("Cannot get elements by index from the type '" + type.Name + "'.");
			}
			if (text.EndsWith("()", StringComparison.InvariantCulture))
			{
				flag = true;
				text = text.Substring(0, text.Length - 2);
			}
			MemberInfo stepMember = GetStepMember(type, text, flag);
			if (stepMember.IsStatic())
			{
				if (!(type == rootType))
				{
					throw new ArgumentException("The non-root member '" + text + "' is static; use that member as the path root instead.");
				}
				rootIsStatic = true;
			}
			type = stepMember.GetReturnType();
			if (flag && (type == null || type == typeof(void)))
			{
				throw new ArgumentException("The method '" + stepMember.Name + "' has no return type and cannot be part of a deep reflection path.");
			}
			list.Add(new PathStep(stepMember));
		}
		if (resultType == null)
		{
			resultType = type;
		}
		else if (type != typeof(object) && !resultType.IsAssignableFrom(type))
		{
			throw new ArgumentException("Last member '" + list[list.Count - 1].Member.Name + "' of path '" + path + "' contains type '" + type.AssemblyQualifiedName + "', which is not assignable to expected type '" + resultType.AssemblyQualifiedName + "'.");
		}
		return list;
	}

	private static MemberInfo GetStepMember(Type owningType, string name, bool expectMethod)
	{
		MemberInfo memberInfo = null;
		MemberInfo[] array = owningType.GetAllMembers(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).ToArray();
		int num = int.MaxValue;
		foreach (MemberInfo memberInfo2 in array)
		{
			if (expectMethod)
			{
				MethodInfo methodInfo = memberInfo2 as MethodInfo;
				if (methodInfo != null)
				{
					int num2 = methodInfo.GetParameters().Length;
					if (memberInfo == null || num2 < num)
					{
						memberInfo = methodInfo;
						num = num2;
					}
				}
				continue;
			}
			if (memberInfo2 is MethodInfo)
			{
				throw new ArgumentException("Found method member for name '" + name + "', but expected a field or property.");
			}
			memberInfo = memberInfo2;
			break;
		}
		if (memberInfo == null)
		{
			throw new ArgumentException("Could not find expected " + (expectMethod ? "method" : "field or property") + " '" + name + "' on type '" + owningType.GetNiceName() + "' while parsing reflection path.");
		}
		if (expectMethod && num > 0)
		{
			throw new NotSupportedException("Method '" + memberInfo.GetNiceName() + "' has " + num + " parameters, but method parameters are currently not supported.");
		}
		if (!(memberInfo is FieldInfo) && !(memberInfo is PropertyInfo) && !(memberInfo is MethodInfo))
		{
			throw new NotSupportedException("Members of type " + memberInfo.GetType().GetNiceName() + " are not support; only fields, properties and methods are supported.");
		}
		return memberInfo;
	}

	private static void EmitConstant(ILGenerator il, object constant, Type type = null)
	{
		if (constant == null)
		{
			il.Emit(OpCodes.Ldnull);
			return;
		}
		if (type == null)
		{
			type = constant.GetType();
		}
		if (type == typeof(int) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort))
		{
			il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(constant));
		}
		else if (type == typeof(uint))
		{
			il.Emit(OpCodes.Ldc_I4, (int)(uint)constant);
		}
		else if (type == typeof(long))
		{
			il.Emit(OpCodes.Ldc_I8, (long)constant);
		}
		else if (type == typeof(ulong))
		{
			il.Emit(OpCodes.Ldc_I8, (long)(ulong)constant);
		}
		else if (type == typeof(float))
		{
			il.Emit(OpCodes.Ldc_R4, (float)constant);
		}
		else if (type == typeof(double))
		{
			il.Emit(OpCodes.Ldc_R8, (double)constant);
		}
		else if (type == typeof(string))
		{
			il.Emit(OpCodes.Ldstr, (string)constant);
		}
		else if (type == typeof(char))
		{
			il.Emit(OpCodes.Ldc_I4, (char)constant);
		}
		else if (type == typeof(decimal))
		{
			int[] bits = decimal.GetBits((decimal)constant);
			ConstructorInfo constructor = typeof(decimal).GetConstructor(new Type[1] { typeof(int[]) });
			LocalBuilder local = il.DeclareLocal(typeof(int[]));
			il.Emit(OpCodes.Ldc_I4, bits.Length);
			il.Emit(OpCodes.Newarr, typeof(int));
			il.Emit(OpCodes.Stloc, local);
			for (int i = 0; i < bits.Length; i++)
			{
				il.Emit(OpCodes.Ldloc, local);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldc_I4, bits[i]);
				il.Emit(OpCodes.Stelem_I4);
			}
			il.Emit(OpCodes.Ldloc, local);
			il.Emit(OpCodes.Newobj, constructor);
		}
		else if (type == typeof(bool))
		{
			il.Emit(((bool)constant) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
		}
		else
		{
			if (!type.IsEnum)
			{
				throw new NotSupportedException("Type " + type.GetNiceFullName() + " is not supported as a constant.");
			}
			EmitConstant(il, constant, Enum.GetUnderlyingType(type));
		}
	}
}
