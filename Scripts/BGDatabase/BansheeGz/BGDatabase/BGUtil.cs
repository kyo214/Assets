using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGUtil
{
	public interface SkipMeInEditor
	{
	}

	public const string NoDatabaseFoundError = "Can not load database from all possible locations. More info: http://www.bansheegz.com/BGDatabase/Setup/";

	public static bool IsAboutToStartInEditor;

	public static bool TestIsRunning { get; private set; }

	public static T Create<T>(string typeName, bool includePrivateConstructors, params object[] parameters)
	{
		Type type = GetType(typeName);
		if (type != null)
		{
			return Create<T>(type, includePrivateConstructors, parameters);
		}
		if (string.IsNullOrEmpty(typeName))
		{
			throw new BGException("Type name is not defined");
		}
		throw new BGException("Can not find type ($)", typeName);
	}

	public static T Create<T>(Type type, bool includePrivateConstructors, params object[] parameters)
	{
		if (!includePrivateConstructors)
		{
			return (T)Activator.CreateInstance(type, parameters);
		}
		Type[] array = ((parameters == null) ? Type.EmptyTypes : new Type[parameters.Length]);
		if (parameters != null)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].GetType();
			}
		}
		ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, array, null);
		return (T)constructor.Invoke(parameters);
	}

	public static Type GetType(string typeName)
	{
		return GetType(typeName, null);
	}

	public static Type GetType(string typeName, bool publicOnly)
	{
		return GetType(typeName, publicOnly ? ((Predicate<Type>)((Type t) => t.IsPublic || t.IsNestedPublic)) : null);
	}

	public static Type GetType(string typeName, Predicate<Type> filter)
	{
		Type type = Type.GetType(typeName);
		if (type != null && filter != null && !filter(type))
		{
			type = null;
		}
		if (type == null)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw new BGException("Type name is not defined");
			}
			int num = typeName.IndexOf(',', 0, typeName.Length);
			if (num >= 0)
			{
				TryToExtractFullTypeName(ref typeName);
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				try
				{
					type = assemblies[i].GetType(typeName, throwOnError: false, ignoreCase: false);
				}
				catch (Exception)
				{
				}
				if (type != null && filter != null && !filter(type))
				{
					type = null;
				}
				if (type != null)
				{
					break;
				}
			}
		}
		return type;
	}

	private static void TryToExtractFullTypeName(ref string typeNameCandidate)
	{
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < typeNameCandidate.Length; i++)
		{
			switch (typeNameCandidate[i])
			{
			case '[':
				num2++;
				continue;
			case ']':
				num2--;
				continue;
			case ',':
				if (num2 != 0)
				{
					continue;
				}
				break;
			default:
				continue;
			}
			num = i;
			break;
		}
		if (num >= 0)
		{
			typeNameCandidate = typeNameCandidate.Substring(0, num).Trim();
		}
	}

	public static bool IsEmpty<T>(ICollection<T> list)
	{
		if (list != null)
		{
			return list.Count == 0;
		}
		return true;
	}

	public static bool IsEmpty<T>(T[] list)
	{
		if (list != null)
		{
			return list.Length == 0;
		}
		return true;
	}

	public static List<TV> EnsureList<TK, TV>(IDictionary<TK, List<TV>> key2Value, TK key)
	{
		key2Value.TryGetValue(key, out var value);
		if (value != null)
		{
			return value;
		}
		return key2Value[key] = new List<TV>();
	}

	public static TV Ensure<TK, TV>(Dictionary<TK, TV> key2Value, TK key, Func<TV> newValue) where TV : class
	{
		key2Value.TryGetValue(key, out var value);
		if (value != null)
		{
			return value;
		}
		return key2Value[key] = newValue();
	}

	public static TV Get<TK, TV>(IDictionary<TK, TV> key2Value, TK key)
	{
		key2Value.TryGetValue(key, out var value);
		return value;
	}

	public static TV Get<TK, TV>(Dictionary<TK, TV> key2Value, TK key, TV defaultValue)
	{
		if (!key2Value.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public static TV GetNullable<TK, TV>(Dictionary<TK, TV> key2Value, TK key)
	{
		if (key2Value != null)
		{
			return Get(key2Value, key);
		}
		return default;
	}

	public static void ForEach<T>(List<T> list, Action<T> action)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				action(list[i]);
			}
		}
	}

	public static bool ListsValuesEqual<T>(List<T> value1, List<T> value2)
	{
		if (value1 == null && value2 == null)
		{
			return true;
		}
		if (value1 == value2 || value1 == null || value2 == null)
		{
			return false;
		}
		if (value1.Count != value2.Count)
		{
			return false;
		}
		if (value1.Count == 0)
		{
			return true;
		}
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		bool result = true;
		for (int i = 0; i < value1.Count; i++)
		{
			T y = value1[i];
			if (!equalityComparer.Equals(value2[i], y))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public static bool ArraysValuesEqual<T>(T[] value1, T[] value2)
	{
		if (value1 == null && value2 == null)
		{
			return true;
		}
		if (value1 == value2 || value1 == null || value2 == null)
		{
			return false;
		}
		if (value1.Length != value2.Length)
		{
			return false;
		}
		if (value1.Length == 0)
		{
			return true;
		}
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		bool result = true;
		for (int i = 0; i < value1.Length; i++)
		{
			T y = value1[i];
			if (!equalityComparer.Equals(value2[i], y))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public static bool IsList(Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(List<>);
		}
		return false;
	}

	public static byte[] ToArray(ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			return Array.Empty<byte>();
		}
		byte[] array = new byte[segment.Count];
		Buffer.BlockCopy(segment.Array, segment.Offset, array, 0, segment.Count);
		return array;
	}

	public static int ToInt(string value, int @default = 0, bool throwException = false)
	{
		try
		{
			return int.Parse(value);
		}
		catch (Exception)
		{
			if (throwException)
			{
				throw new BGException("Can not convert to int, value=$", value);
			}
			return @default;
		}
	}

	public static T ToEnum<T>(string data) where T : struct
	{
		return (T)Enum.ToObject(typeof(T), ToInt(data));
	}

	public static string Format(string message, params object[] args)
	{
		if (args == null || args.Length == 0)
		{
			return message;
		}
		try
		{
			int num = message.IndexOf('$');
			if (num == -1)
			{
				return message;
			}
			for (int i = 0; i < 100; i++)
			{
				if (num < 0)
				{
					break;
				}
				string text = "{" + i + "}";
				message = message.Substring(0, num) + text + message.Substring(num + 1);
				num += text.Length;
				num = ((num < message.Length) ? message.IndexOf('$', num) : (-1));
			}
			return string.Format(message, args);
		}
		catch (Exception)
		{
			return message;
		}
	}

	public static T GetAttribute<T>(Type type, bool inherit = false) where T : Attribute
	{
		Type typeFromHandle = typeof(T);
		if (!type.IsDefined(typeFromHandle, inherit))
		{
			return null;
		}
		return (T)Attribute.GetCustomAttribute(type, typeFromHandle);
	}

	public static bool HasAttribute<T>(Type type, bool inherit)
	{
		return type.IsDefined(typeof(T), inherit);
	}

	public static List<Type> GetTypes(Predicate<Type> filter = null)
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		Assembly[] array = assemblies;
		foreach (Assembly assembly in array)
		{
			if (IsSystem(assembly))
			{
				continue;
			}
			Type[] types;
			try
			{
				types = assembly.GetTypes();
			}
			catch
			{
				continue;
			}
			if (filter == null)
			{
				for (int j = 0; j < types.Length; j++)
				{
					list.Add(types[j]);
				}
				continue;
			}
			foreach (Type type in types)
			{
				if (filter(type))
				{
					list.Add(type);
				}
			}
		}
		return list;
	}

	private static bool IsSystem(Assembly assembly)
	{
		string fullName = assembly.FullName;
		if (fullName.StartsWith("Unity", StringComparison.Ordinal) || fullName.StartsWith("System", StringComparison.Ordinal) || fullName.StartsWith("Mono", StringComparison.Ordinal) || fullName.StartsWith("Accessibility", StringComparison.Ordinal) || fullName.StartsWith("mscorlib", StringComparison.Ordinal))
		{
			return true;
		}
		return false;
	}

	public static List<Type> GetAllSubTypes(Type targetType, Predicate<Type> filter = null)
	{
		if (filter != null)
		{
			return GetTypes((Type type) => SubclassFilter(type) && filter(type));
		}
		return GetTypes(SubclassFilter);
		bool SubclassFilter(Type type)
		{
			if (type.IsClass && !type.IsAbstract)
			{
				return type.IsSubclassOf(targetType);
			}
			return false;
		}
	}

	public static T Clone<T>(T @object) where T : class
	{
		if (@object == null)
		{
			return null;
		}
		MethodInfo method = @object.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
		if (method == null)
		{
			return null;
		}
		return (T)method.Invoke(@object, null);
	}

	public static long Measure(string operation, Action action, bool printResult = true)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		action();
		stopwatch.Stop();
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		if (printResult)
		{
			UnityEngine.Debug.Log(operation + ": " + elapsedMilliseconds);
		}
		return elapsedMilliseconds;
	}

	public static List<Type> GetAllImplementations(Type interfaceType, Predicate<Type> filter = null)
	{
		Predicate<Type> subclassFilter = (Type type) => type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type);
		if (filter != null)
		{
			return GetTypes((Type type) => subclassFilter(type) && filter(type));
		}
		return GetTypes(subclassFilter);
	}

	public static void Catch(ref Exception exception, Action action, Action finallyAction = null)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			if (exception == null)
			{
				exception = ex;
			}
		}
		finally
		{
			finallyAction?.Invoke();
		}
	}

	public static void Catch(Action action, Action<Exception> exceptionAction = null, Action finallyAction = null)
	{
		try
		{
			action();
		}
		catch (Exception obj)
		{
			exceptionAction?.Invoke(obj);
		}
		finally
		{
			finallyAction?.Invoke();
		}
	}

	public static void FromString(BGField field, int entityIndex, string value)
	{
		if (field.CustomStringFormatSupported)
		{
			field.FromCustomString(entityIndex, value);
		}
		else
		{
			field.FromString(entityIndex, value);
		}
	}

	public static string ToString(BGField field, int entityIndex)
	{
		if (!field.CustomStringFormatSupported)
		{
			return field.ToString(entityIndex);
		}
		return field.ToCustomString(entityIndex);
	}

	public static string DuplicateMetaName(BGMetaEntity meta, Func<string, bool> isValidName = null)
	{
		int num = 2;
		string baseName = GetBaseName(meta.Name, num);
		while (meta.Repo.HasMeta(baseName) || (isValidName != null && !isValidName(baseName)))
		{
			baseName = GetBaseName(meta.Name, ++num);
			if (num > 100000)
			{
				throw new Exception("Can not generate new name");
			}
		}
		return baseName;
	}

	private static string GetBaseName(string metaName, int counter)
	{
		string text = counter.ToString() ?? "";
		string text2 = metaName + text;
		if (text2.Length > 31)
		{
			text2 = metaName.Substring(0, 31 - text.Length) + text;
		}
		return text2;
	}

	public static void RunTest(Action action)
	{
		TestIsRunning = true;
		try
		{
			action();
		}
		finally
		{
			TestIsRunning = false;
		}
	}

	public static T[] Concat<T>(params T[][] arrays)
	{
		int num = 0;
		for (int i = 0; i < arrays.Length; i++)
		{
			num += arrays[i].Length;
		}
		T[] array = new T[num];
		int num2 = 0;
		foreach (T[] array2 in arrays)
		{
			int num3 = array2.Length;
			Array.Copy(array2, 0, array, num2, num3);
			num2 += num3;
		}
		return array;
	}

	public static bool IsAssignable(Type fieldType, Type targetType)
	{
		return targetType.IsAssignableFrom(fieldType);
	}

	public static bool AreEqual(string value1, string value2)
	{
		bool flag = string.IsNullOrEmpty(value1);
		bool flag2 = string.IsNullOrEmpty(value2);
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		return value1.Equals(value2);
	}

	public static bool IsPrefab(GameObject go)
	{
		return go.scene.rootCount == 0;
	}

	public static void SaveDatabaseInUnityEditor()
	{
		try
		{
			string text = "BansheeGz.BGDatabase.Editor.BGRepoSaver";
			Type type = GetType(text);
			if (type == null)
			{
				throw new Exception("Can not save database: " + text + " type is not found!");
			}
			string text2 = "SaveAndMarkAsSaved";
			MethodInfo method = type.GetMethod(text2);
			if (method == null)
			{
				throw new Exception("Can not save database: method " + text2 + " method is not found at type " + text + "!");
			}
			method.Invoke(null, null);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	public static string CheckNameForNewMetaObject(string newName)
	{
		if (newName == null)
		{
			return "Name can not be empty";
		}
		if (BGMetaObject.ReservedWordsForNewObjects.Contains(newName))
		{
			return "This name [" + newName + "] is reserved for system needs. Please, choose another name.";
		}
		return null;
	}

	public static void CheckNameForNewMetaObjectWithException(string newName)
	{
		string text = CheckNameForNewMetaObject(newName);
		if (!string.IsNullOrEmpty(text))
		{
			throw new Exception(text);
		}
	}
}
