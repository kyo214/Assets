using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMDebug
{
	public struct DebugLogItem(object message, string color, int framecount, float time, int timePrecision, bool displayFrameCount)
	{
		public object Message = message;

		public string Color = color;

		public int Framecount = framecount;

		public float Time = time;

		public int TimePrecision = timePrecision;

		public bool DisplayFrameCount = displayFrameCount;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct MMDebugLogEvent
	{
		public delegate void Delegate(DebugLogItem item);

		private static event Delegate OnEvent;

		public static void Register(Delegate callback)
		{
			OnEvent += callback;
		}

		public static void Unregister(Delegate callback)
		{
			OnEvent -= callback;
		}

		public static void Trigger(DebugLogItem item)
		{
			OnEvent?.Invoke(item);
		}
	}

	private static MethodInfo[] _commands;

	private static readonly int _logHistoryMaxLength = 256;

	private static bool _debugDrawEnabled = false;

	private static bool _debugLogEnabled = false;

	private static bool _debugLogEnabledSet = false;

	public static List<DebugLogItem> LogHistory = new List<DebugLogItem>(_logHistoryMaxLength);

	private const string _editorPrefsDebugLogs = "DebugLogsEnabled";

	private const string _editorPrefsDebugDraws = "DebugDrawsEnabled";

	public static MMDebugOnScreenConsole _console;

	private const string _debugConsolePrefabPath = "MMDebugOnScreenConsole";

	public static MethodInfo[] Commands
	{
		get
		{
			if (_commands == null)
			{
				_commands = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly m) => m.GetTypes().SelectMany((Type n) => from o in n.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					where o.GetCustomAttribute<MMDebugLogCommandAttribute>() != null
					select o)).ToArray();
			}
			return _commands;
		}
	}

	public static string LogHistoryText
	{
		get
		{
			string text = "";
			string text2 = "";
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < LogHistory.Count; i++)
			{
				if (!string.IsNullOrEmpty(LogHistory[i].Color))
				{
					text = "<color=" + LogHistory[i].Color + ">";
					text2 = "</color>";
				}
				if (LogHistory[i].DisplayFrameCount)
				{
					stringBuilder.Append("<color=#82d3f9>[" + LogHistory[i].Framecount + "]</color> ");
				}
				stringBuilder.Append("<color=#f9a682>[" + MMTime.FloatToTimeString(LogHistory[i].Time, displayHours: false, displayMinutes: true, displaySeconds: true, displayMilliseconds: true) + "]</color> ");
				stringBuilder.Append(text + LogHistory[i].Message?.ToString() + text2);
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}

	public static bool DebugLogsEnabled
	{
		get
		{
			if (_debugLogEnabledSet)
			{
				return _debugLogEnabled;
			}
			if (PlayerPrefs.HasKey("DebugLogsEnabled"))
			{
				_debugLogEnabled = ((PlayerPrefs.GetInt("DebugLogsEnabled") != 0) ? true : false);
			}
			else
			{
				_debugLogEnabled = true;
			}
			_debugLogEnabledSet = true;
			return _debugLogEnabled;
		}
		private set
		{
			_debugLogEnabledSet = true;
			_debugLogEnabled = value;
		}
	}

	public static bool DebugDrawEnabled
	{
		get
		{
			return false;
		}
		private set
		{
		}
	}

	public static void DebugLogCommand(string command)
	{
		if (command == string.Empty || command == null)
		{
			LogCommand("", "#ff2a00");
			return;
		}
		string[] array = command.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (array == null || array.Length == 0)
		{
			LogCommand("Empty command", "#ff2a00");
			return;
		}
		string commandFirst = MMString.UppercaseFirst(array[0]);
		MethodInfo[] array2 = Commands.Where((MethodInfo m) => m.Name == commandFirst).ToArray();
		if (array2.Length == 0)
		{
			LogCommand("Command " + commandFirst + " not found.", "#ff2a00");
			return;
		}
		object[] parameters = null;
		if (array.Length > 1)
		{
			MethodInfo methodInfo = array2.Where((MethodInfo m) => m.GetParameters().Length != 0).FirstOrDefault();
			if (methodInfo == null)
			{
				LogCommand("A version of command " + commandFirst + " with arguments could not be found. Maybe try without arguments.", "#ff2a00");
				return;
			}
			MMDebugLogCommandArgumentCountAttribute mMDebugLogCommandArgumentCountAttribute = methodInfo.GetCustomAttributes<MMDebugLogCommandArgumentCountAttribute>(inherit: true).FirstOrDefault();
			if (mMDebugLogCommandArgumentCountAttribute != null && mMDebugLogCommandArgumentCountAttribute.ArgumentCount > array.Length - 1)
			{
				string[] obj = new string[5] { "A version of command ", commandFirst, " needs at least ", null, null };
				int argumentCount = mMDebugLogCommandArgumentCountAttribute.ArgumentCount;
				obj[3] = argumentCount.ToString();
				obj[4] = " arguments.";
				LogCommand(string.Concat(obj), "#ff2a00");
				return;
			}
			parameters = new object[1] { array };
		}
		else
		{
			MethodInfo methodInfo = array2.Where((MethodInfo m) => m.GetParameters().Length == 0).FirstOrDefault();
			if (methodInfo == null)
			{
				LogCommand("A version of command " + commandFirst + " without arguments could not be found.", "#ff2a00");
				return;
			}
		}
		LogCommand(command, "#FFC400");
		array2[0].Invoke(null, parameters);
	}

	private static void LogCommand(string command, string color)
	{
		DebugLogItem item = new DebugLogItem(command, color, Time.frameCount, Time.time, 3, displayFrameCount: true);
		LogHistory.Add(item);
		MMDebugLogEvent.Trigger(new DebugLogItem(null, "", Time.frameCount, Time.time, 0, displayFrameCount: false));
	}

	public static void DebugLogClear()
	{
		LogHistory.Clear();
		MMDebugLogEvent.Trigger(new DebugLogItem(null, "", Time.frameCount, Time.time, 0, displayFrameCount: false));
	}

	public static void DebugLogTime(object message, string color = "", int timePrecision = 3, bool displayFrameCount = true)
	{
		if (DebugLogsEnabled)
		{
			string name = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name;
			color = ((color == "") ? "#00FFFF" : color);
			string text = "";
			string text2 = "";
			if (!string.IsNullOrEmpty(color))
			{
				text = "<color=" + color + ">";
				text2 = "</color>";
			}
			string text3 = "";
			if (displayFrameCount)
			{
				text3 = text3 + "<color=#82d3f9>[f" + Time.frameCount + "]</color> ";
			}
			text3 = text3 + "<color=#f9a682>[" + MMTime.FloatToTimeString(Time.time, displayHours: false, displayMinutes: true, displaySeconds: true, displayMilliseconds: true) + "]</color> ";
			text3 = text3 + name + " : ";
			text3 = text3 + text + message?.ToString() + text2;
			UnityEngine.Debug.Log(text3);
			LogDebugToConsole(message, color, timePrecision, displayFrameCount);
		}
	}

	public static DebugLogItem LogDebugToConsole(object message, string color, int timePrecision, bool displayFrameCount)
	{
		DebugLogItem debugLogItem = new DebugLogItem(message, color, Time.frameCount, Time.time, timePrecision, displayFrameCount);
		if (LogHistory.Count > _logHistoryMaxLength)
		{
			LogHistory.RemoveAt(0);
		}
		LogHistory.Add(debugLogItem);
		MMDebugLogEvent.Trigger(debugLogItem);
		return debugLogItem;
	}

	public static void SetDebugLogsEnabled(bool status)
	{
		DebugLogsEnabled = status;
		_debugLogEnabled = status;
	}

	public static void SetDebugDrawEnabled(bool status)
	{
		DebugDrawEnabled = status;
		_debugDrawEnabled = status;
	}

	public static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
	{
		if (drawGizmo && DebugDrawEnabled)
		{
			UnityEngine.Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
		}
		return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
	}

	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float length, LayerMask mask, Color color, bool drawGizmo = false)
	{
		if (drawGizmo && DebugDrawEnabled)
		{
			Quaternion quaternion = Quaternion.Euler(0f, 0f, angle);
			Vector3[] array = new Vector3[8];
			float num = size.x / 2f;
			float num2 = size.y / 2f;
			array[0] = quaternion * (origin + Vector2.left * num + Vector2.up * num2);
			array[1] = quaternion * (origin + Vector2.right * num + Vector2.up * num2);
			array[2] = quaternion * (origin + Vector2.right * num - Vector2.up * num2);
			array[3] = quaternion * (origin + Vector2.left * num - Vector2.up * num2);
			array[4] = quaternion * (origin + Vector2.left * num + Vector2.up * num2 + length * direction);
			array[5] = quaternion * (origin + Vector2.right * num + Vector2.up * num2 + length * direction);
			array[6] = quaternion * (origin + Vector2.right * num - Vector2.up * num2 + length * direction);
			array[7] = quaternion * (origin + Vector2.left * num - Vector2.up * num2 + length * direction);
			UnityEngine.Debug.DrawLine(array[0], array[1], color);
			UnityEngine.Debug.DrawLine(array[1], array[2], color);
			UnityEngine.Debug.DrawLine(array[2], array[3], color);
			UnityEngine.Debug.DrawLine(array[3], array[0], color);
			UnityEngine.Debug.DrawLine(array[4], array[5], color);
			UnityEngine.Debug.DrawLine(array[5], array[6], color);
			UnityEngine.Debug.DrawLine(array[6], array[7], color);
			UnityEngine.Debug.DrawLine(array[7], array[4], color);
			UnityEngine.Debug.DrawLine(array[0], array[4], color);
			UnityEngine.Debug.DrawLine(array[1], array[5], color);
			UnityEngine.Debug.DrawLine(array[2], array[6], color);
			UnityEngine.Debug.DrawLine(array[3], array[7], color);
		}
		return Physics2D.BoxCast(origin, size, angle, direction, length, mask);
	}

	public static RaycastHit2D MonoRayCastNonAlloc(RaycastHit2D[] array, Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
	{
		if (drawGizmo && DebugDrawEnabled)
		{
			UnityEngine.Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
		}
		if (Physics2D.RaycastNonAlloc(rayOriginPoint, rayDirection, array, rayDistance, mask) > 0)
		{
			return array[0];
		}
		return default;
	}

	public static RaycastHit Raycast3D(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		if (drawGizmo && DebugDrawEnabled)
		{
			UnityEngine.Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
		}
		Physics.Raycast(rayOriginPoint, rayDirection, out var hitInfo, rayDistance, mask, queryTriggerInteraction);
		return hitInfo;
	}

	public static void DebugOnScreen(string message)
	{
		if (DebugLogsEnabled)
		{
			InstantiateOnScreenConsole();
			_console.AddMessage(message, "", 30);
		}
	}

	public static void DebugOnScreen(string label, object value, int fontSize = 25)
	{
		if (DebugLogsEnabled)
		{
			InstantiateOnScreenConsole(fontSize);
			_console.AddMessage(label, value, fontSize);
		}
	}

	public static void InstantiateOnScreenConsole(int fontSize = 25)
	{
		if (DebugLogsEnabled)
		{
			if (_console == null)
			{
				_console = (MMDebugOnScreenConsole)UnityEngine.Object.FindObjectOfType(typeof(MMDebugOnScreenConsole));
			}
			if (_console == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("MMDebugOnScreenConsole") as GameObject);
				gameObject.name = "MMDebugOnScreenConsole";
				_console = gameObject.GetComponent<MMDebugOnScreenConsole>();
			}
		}
	}

	public static void SetOnScreenConsole(MMDebugOnScreenConsole newConsole)
	{
		_console = newConsole;
	}

	public static void DrawGizmoArrow(Vector3 origin, Vector3 direction, Color color, float arrowHeadLength = 3f, float arrowHeadAngle = 25f)
	{
		if (DebugDrawEnabled)
		{
			Gizmos.color = color;
			Gizmos.DrawRay(origin, direction);
			DrawArrowEnd(drawGizmos: true, origin, direction, color, arrowHeadLength, arrowHeadAngle);
		}
	}

	public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowHeadLength = 0.2f, float arrowHeadAngle = 35f)
	{
		if (DebugDrawEnabled)
		{
			UnityEngine.Debug.DrawRay(origin, direction, color);
			DrawArrowEnd(drawGizmos: false, origin, direction, color, arrowHeadLength, arrowHeadAngle);
		}
	}

	public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowLength, float arrowHeadLength = 0.2f, float arrowHeadAngle = 35f)
	{
		if (DebugDrawEnabled)
		{
			UnityEngine.Debug.DrawRay(origin, direction * arrowLength, color);
			DrawArrowEnd(drawGizmos: false, origin, direction * arrowLength, color, arrowHeadLength, arrowHeadAngle);
		}
	}

	public static void DebugDrawCross(Vector3 spot, float crossSize, Color color)
	{
		if (DebugDrawEnabled)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			zero.x = spot.x - crossSize / 2f;
			zero.y = spot.y - crossSize / 2f;
			zero.z = spot.z;
			zero2.x = 1f;
			zero2.y = 1f;
			zero2.z = 0f;
			UnityEngine.Debug.DrawRay(zero, zero2 * crossSize, color);
			zero.x = spot.x - crossSize / 2f;
			zero.y = spot.y + crossSize / 2f;
			zero.z = spot.z;
			zero2.x = 1f;
			zero2.y = -1f;
			zero2.z = 0f;
			UnityEngine.Debug.DrawRay(zero, zero2 * crossSize, color);
		}
	}

	private static void DrawArrowEnd(bool drawGizmos, Vector3 arrowEndPosition, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 40f)
	{
		if (DebugDrawEnabled && !(direction == Vector3.zero))
		{
			Vector3 vector = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0f, 0f) * Vector3.back;
			Vector3 vector2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f - arrowHeadAngle, 0f, 0f) * Vector3.back;
			Vector3 vector3 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, arrowHeadAngle, 0f) * Vector3.back;
			Vector3 vector4 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 0f - arrowHeadAngle, 0f) * Vector3.back;
			if (drawGizmos)
			{
				Gizmos.color = color;
				Gizmos.DrawRay(arrowEndPosition + direction, vector * arrowHeadLength);
				Gizmos.DrawRay(arrowEndPosition + direction, vector2 * arrowHeadLength);
				Gizmos.DrawRay(arrowEndPosition + direction, vector3 * arrowHeadLength);
				Gizmos.DrawRay(arrowEndPosition + direction, vector4 * arrowHeadLength);
			}
			else
			{
				UnityEngine.Debug.DrawRay(arrowEndPosition + direction, vector * arrowHeadLength, color);
				UnityEngine.Debug.DrawRay(arrowEndPosition + direction, vector2 * arrowHeadLength, color);
				UnityEngine.Debug.DrawRay(arrowEndPosition + direction, vector3 * arrowHeadLength, color);
				UnityEngine.Debug.DrawRay(arrowEndPosition + direction, vector4 * arrowHeadLength, color);
			}
		}
	}

	public static void DrawHandlesBounds(Bounds bounds, Color color)
	{
		_ = DebugDrawEnabled;
	}

	public static void DrawSolidRectangle(Vector3 position, Vector3 size, Color borderColor, Color solidColor)
	{
		_ = DebugDrawEnabled;
	}

	public static void DrawGizmoPoint(Vector3 position, float size, Color color)
	{
		if (DebugDrawEnabled)
		{
			Gizmos.color = color;
			Gizmos.DrawWireSphere(position, size);
		}
	}

	public static void DrawCube(Vector3 position, Color color, Vector3 size)
	{
		if (DebugDrawEnabled)
		{
			Vector3 vector = size / 2f;
			Vector3[] array = new Vector3[8]
			{
				position + new Vector3(vector.x, vector.y, vector.z),
				position + new Vector3(0f - vector.x, vector.y, vector.z),
				position + new Vector3(0f - vector.x, 0f - vector.y, vector.z),
				position + new Vector3(vector.x, 0f - vector.y, vector.z),
				position + new Vector3(vector.x, vector.y, 0f - vector.z),
				position + new Vector3(0f - vector.x, vector.y, 0f - vector.z),
				position + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z),
				position + new Vector3(vector.x, 0f - vector.y, 0f - vector.z)
			};
			UnityEngine.Debug.DrawLine(array[0], array[1], color);
			UnityEngine.Debug.DrawLine(array[1], array[2], color);
			UnityEngine.Debug.DrawLine(array[2], array[3], color);
			UnityEngine.Debug.DrawLine(array[3], array[0], color);
		}
	}

	public static void DrawGizmoCube(Transform transform, Vector3 offset, Vector3 cubeSize, bool wireOnly)
	{
		if (DebugDrawEnabled)
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			if (wireOnly)
			{
				Gizmos.DrawWireCube(offset, cubeSize);
			}
			else
			{
				Gizmos.DrawCube(offset, cubeSize);
			}
		}
	}

	public static void DrawGizmoRectangle(Vector2 center, Vector2 size, Color color)
	{
		if (DebugDrawEnabled)
		{
			Gizmos.color = color;
			Vector3 vector = new Vector3(center.x - size.x / 2f, center.y + size.y / 2f, 0f);
			Vector3 vector2 = new Vector3(center.x + size.x / 2f, center.y + size.y / 2f, 0f);
			Vector3 vector3 = new Vector3(center.x + size.x / 2f, center.y - size.y / 2f, 0f);
			Vector3 vector4 = new Vector3(center.x - size.x / 2f, center.y - size.y / 2f, 0f);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector);
		}
	}

	public static void DrawGizmoRectangle(Vector2 center, Vector2 size, Matrix4x4 rotationMatrix, Color color)
	{
		if (DebugDrawEnabled)
		{
			GL.PushMatrix();
			Gizmos.color = color;
			Vector3 vector = rotationMatrix * new Vector3(center.x - size.x / 2f, center.y + size.y / 2f, 0f);
			Vector3 vector2 = rotationMatrix * new Vector3(center.x + size.x / 2f, center.y + size.y / 2f, 0f);
			Vector3 vector3 = rotationMatrix * new Vector3(center.x + size.x / 2f, center.y - size.y / 2f, 0f);
			Vector3 vector4 = rotationMatrix * new Vector3(center.x - size.x / 2f, center.y - size.y / 2f, 0f);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector);
			GL.PopMatrix();
		}
	}

	public static void DrawRectangle(Rect rectangle, Color color)
	{
		if (DebugDrawEnabled)
		{
			Vector3 position = new Vector3(rectangle.x + rectangle.width / 2f, rectangle.y + rectangle.height / 2f, 0f);
			Vector3 size = new Vector3(rectangle.width, rectangle.height, 0f);
			DrawRectangle(position, color, size);
		}
	}

	public static void DrawRectangle(Vector3 position, Color color, Vector3 size)
	{
		if (DebugDrawEnabled)
		{
			Vector3 vector = size / 2f;
			Vector3[] array = new Vector3[4]
			{
				position + new Vector3(vector.x, vector.y, vector.z),
				position + new Vector3(0f - vector.x, vector.y, vector.z),
				position + new Vector3(0f - vector.x, 0f - vector.y, vector.z),
				position + new Vector3(vector.x, 0f - vector.y, vector.z)
			};
			UnityEngine.Debug.DrawLine(array[0], array[1], color);
			UnityEngine.Debug.DrawLine(array[1], array[2], color);
			UnityEngine.Debug.DrawLine(array[2], array[3], color);
			UnityEngine.Debug.DrawLine(array[3], array[0], color);
		}
	}

	public static void DrawPoint(Vector3 position, Color color, float size)
	{
		if (DebugDrawEnabled)
		{
			Vector3[] array = new Vector3[6]
			{
				position + Vector3.up * size,
				position - Vector3.up * size,
				position + Vector3.right * size,
				position - Vector3.right * size,
				position + Vector3.forward * size,
				position - Vector3.forward * size
			};
			UnityEngine.Debug.DrawLine(array[0], array[1], color);
			UnityEngine.Debug.DrawLine(array[2], array[3], color);
			UnityEngine.Debug.DrawLine(array[4], array[5], color);
			UnityEngine.Debug.DrawLine(array[0], array[2], color);
			UnityEngine.Debug.DrawLine(array[0], array[3], color);
			UnityEngine.Debug.DrawLine(array[0], array[4], color);
			UnityEngine.Debug.DrawLine(array[0], array[5], color);
			UnityEngine.Debug.DrawLine(array[1], array[2], color);
			UnityEngine.Debug.DrawLine(array[1], array[3], color);
			UnityEngine.Debug.DrawLine(array[1], array[4], color);
			UnityEngine.Debug.DrawLine(array[1], array[5], color);
			UnityEngine.Debug.DrawLine(array[4], array[2], color);
			UnityEngine.Debug.DrawLine(array[4], array[3], color);
			UnityEngine.Debug.DrawLine(array[5], array[2], color);
			UnityEngine.Debug.DrawLine(array[5], array[3], color);
		}
	}

	public static void DrawGizmoPoint(Vector3 position, Color color, float size)
	{
		if (DebugDrawEnabled)
		{
			Vector3[] array = new Vector3[6]
			{
				position + Vector3.up * size,
				position - Vector3.up * size,
				position + Vector3.right * size,
				position - Vector3.right * size,
				position + Vector3.forward * size,
				position - Vector3.forward * size
			};
			Gizmos.color = color;
			Gizmos.DrawLine(array[0], array[1]);
			Gizmos.DrawLine(array[2], array[3]);
			Gizmos.DrawLine(array[4], array[5]);
			Gizmos.DrawLine(array[0], array[2]);
			Gizmos.DrawLine(array[0], array[3]);
			Gizmos.DrawLine(array[0], array[4]);
			Gizmos.DrawLine(array[0], array[5]);
			Gizmos.DrawLine(array[1], array[2]);
			Gizmos.DrawLine(array[1], array[3]);
			Gizmos.DrawLine(array[1], array[4]);
			Gizmos.DrawLine(array[1], array[5]);
			Gizmos.DrawLine(array[4], array[2]);
			Gizmos.DrawLine(array[4], array[3]);
			Gizmos.DrawLine(array[5], array[2]);
			Gizmos.DrawLine(array[5], array[3]);
		}
	}

	public static string GetSystemInfo()
	{
		string text = "SYSTEM INFO";
		text = text + "\n<color=#FFFFFF>Device Type :</color> " + SystemInfo.deviceType;
		text = text + "\n<color=#FFFFFF>OS Version :</color> " + SystemInfo.operatingSystem;
		text = text + "\n<color=#FFFFFF>System Memory Size :</color> " + SystemInfo.systemMemorySize;
		text = text + "\n<color=#FFFFFF>Graphic Device Name :</color> " + SystemInfo.graphicsDeviceName + " (version " + SystemInfo.graphicsDeviceVersion + ")";
		text = text + "\n<color=#FFFFFF>Graphic Memory Size :</color> " + SystemInfo.graphicsMemorySize;
		text = text + "\n<color=#FFFFFF>Graphic Max Texture Size :</color> " + SystemInfo.maxTextureSize;
		text = text + "\n<color=#FFFFFF>Graphic Shader Level :</color> " + SystemInfo.graphicsShaderLevel;
		text = text + "\n<color=#FFFFFF>Compute Shader Support :</color> " + SystemInfo.supportsComputeShaders;
		text = text + "\n<color=#FFFFFF>Processor Count :</color> " + SystemInfo.processorCount;
		text = text + "\n<color=#FFFFFF>Processor Type :</color> " + SystemInfo.processorType;
		text = text + "\n<color=#FFFFFF>3D Texture Support :</color> " + SystemInfo.supports3DTextures;
		text = text + "\n<color=#FFFFFF>Shadow Support :</color> " + SystemInfo.supportsShadows;
		text = text + "\n<color=#FFFFFF>Platform :</color> " + Application.platform;
		text = text + "\n<color=#FFFFFF>Screen Size :</color> " + Screen.width + " x " + Screen.height;
		return text + "\n<color=#FFFFFF>DPI :</color> " + Screen.dpi;
	}

	public static void ClearConsole()
	{
		Type type = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
		if (type != null)
		{
			MethodInfo method = type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}
	}
}
