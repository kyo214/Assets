using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGMetaObject : BGObject, BGConfigurableI, BGConfigurableBinaryI, BGObjectWithNameI, BGObjectI, BGIndexableI
{
	public static readonly HashSet<string> ReservedWords = new HashSet<string>(new string[86]
	{
		"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
		"class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum",
		"event", "explicit", "extern", "finally", "fixed", "float", "for", "foreach", "goto", "if",
		"implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
		"null", "object", "operator", "out", "override", "params", "private", "public", "readonly", "ref",
		"return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
		"this", "throw", "try", "typeof", "unit", "ulong", "unchecked", "unsafe", "ushort", "using",
		"virtual", "void", "volatile", "while", "FALSE", "TRUE", "yield", "by", "descending", "from",
		"group", "into", "orderby", "select", "var", "where"
	});

	public static readonly HashSet<string> ReservedWordsForNewObjects = new HashSet<string>(new string[5] { "false", "protected", "true", "uint", "Index" });

	private string name;

	private bool system;

	private string addon;

	private string comment;

	private object controller;

	private string controllerType;

	private bool controllerLoadTried;

	public virtual string Name
	{
		get
		{
			return name;
		}
		set
		{
			SetName(value);
		}
	}

	public virtual bool System
	{
		get
		{
			return system;
		}
		set
		{
			system = value;
		}
	}

	public string Addon
	{
		get
		{
			return addon;
		}
		set
		{
			addon = value;
		}
	}

	public abstract int Index { get; }

	public virtual string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			comment = value;
		}
	}

	public virtual string ControllerType
	{
		get
		{
			return controllerType;
		}
		set
		{
			if (!object.Equals(controllerType, value))
			{
				controllerType = value;
				controller = null;
				controllerLoadTried = false;
			}
		}
	}

	public object Controller
	{
		get
		{
			if (controller != null)
			{
				return controller;
			}
			if (controllerLoadTried || string.IsNullOrEmpty(controllerType))
			{
				return null;
			}
			controllerLoadTried = true;
			try
			{
				Type type = BGUtil.GetType(controllerType);
				if (type == null)
				{
					throw new Exception("Can not find a C# type with name " + controllerType);
				}
				controller = Activator.CreateInstance(type);
			}
			catch (Exception exception)
			{
				Debug.Log("BGDatabase: can not load a controller for [" + GetType().FullName + "] with name [" + Name + "], see the next message for details");
				Debug.LogException(exception);
			}
			return controller;
		}
		set
		{
			if (!object.Equals(controllerType, value))
			{
				controller = value;
				if (controller != null)
				{
					controllerLoadTried = true;
					controllerType = controller.GetType().FullName;
				}
			}
		}
	}

	protected BGMetaObject(BGId id, string name)
		: base(id)
	{
		SetName(name);
	}

	private void SetName(string value)
	{
		string text = CheckName(value);
		if (text != null)
		{
			throw new BGException("Error in name ($): $", value, text);
		}
		name = value;
	}

	public static string CheckName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return "Name can not be empty";
		}
		if (name.Length > 31)
		{
			return "Name is not valid (31 characters max, no more)";
		}
		if (!char.IsLetter(name[0]))
		{
			return "Name should start with a letter";
		}
		if (ReservedWords.Contains(name))
		{
			return "This name (" + name + ") is reserved for system needs. Please, choose another name.";
		}
		for (int i = 1; i < name.Length; i++)
		{
			char c = name[i];
			if (!char.IsLetterOrDigit(c) && c != '_')
			{
				return "Name should contain letters, digits or underscore only";
			}
		}
		return null;
	}

	public abstract string ConfigToString();

	public abstract void ConfigFromString(string config);

	public abstract byte[] ConfigToBytes();

	public abstract void ConfigFromBytes(ArraySegment<byte> config);
}
